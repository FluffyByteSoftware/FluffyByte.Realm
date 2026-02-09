/*
 * (Sentinel.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@1:23:57 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Net;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Heartbeats;
using FluffyByte.Realm.Tools.Logger;
using LiteNetLib;

namespace FluffyByte.Realm.Networking.ServerCore;

public static class Sentinel
{
    private static NetManager _netManager = null!;
    private static SentinelListener _listener = null!;
    private static RealmServer? _serverConfig;
    private static bool _initialized;
    private static bool _running;

    private const string NetworkClockName = "NetworkClock";
    private const int NetworkTickIntervalMs = 10; // 100 ticks per second

    public static void Initialize(RealmServer serverConfig)
    {
        if (_initialized)
        {
            Log.Debug($"[Sentinel]: Already initialized.");
            return;
        }

        _serverConfig = serverConfig ?? throw new ArgumentNullException(nameof(serverConfig));

        _listener = new SentinelListener();
        _netManager = new NetManager(_listener)
        {
            AutoRecycle = true,
            IPv6Enabled = false
        };

        ClockManager.RegisterClock(NetworkClockName, NetworkTickIntervalMs);
        
        // Subscribe
        EventManager.Subscribe<TickEvent>(OnNetworkTick);
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);
        
        _initialized = true;

        Log.Debug($"[Sentinel]: Initialized for server, {serverConfig.ServerName}");
    }

    public static void Start()
    {
        if (!_initialized)
        {
            Log.Debug($"[Sentinel]: Cannot start - not initialized.");
            return;
        }

        if (_running)
        {
            return;
        }

        if (_serverConfig == null)
        {
            Log.Debug($"[Sentinel]: Critical Error - NetManager or ServerConfig is null!");
            return;
        }

        var convertedIp = IPAddress.Parse(_serverConfig.IpAddress);
        var started = _netManager.Start(convertedIp, null, _serverConfig.Port);

        if (!started)
        {
            Log.Error($"[Sentinel]: Failed to start on port: {_serverConfig.Port}");
            return;
        }

        ClockManager.StartClock(NetworkClockName);

        _running = true;
    }

    private static void Stop()
    {
        if (!_running)
        {
            return;
        }
        
        ClockManager.StopClock(NetworkClockName);

        _netManager.DisconnectAll();

        _netManager.Stop();

        _running = false;

        Log.Debug($"[Sentinel]: Server Stopped.");
    }

    public static void Shutdown()
    {
        if (!_initialized)
            return;

        Stop();

        EventManager.Unsubscribe<TickEvent>(OnNetworkTick);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);

        _initialized = false;
    }

    public static int GetConnectedPeerCount()
    {
        return _netManager.ConnectedPeersCount;
    }

    private static void OnNetworkTick(TickEvent e)
    {
        if (e.ClockName != NetworkClockName || !_running)
            return;

        try
        {
            _netManager.PollEvents();
        }
        catch (Exception ex)
        {
            Log.Debug(ex);
        }
    }

    private static void OnShutdown(SystemShutdownEvent e)
    {
        Log.Debug($"[Sentinel]: System shutdown detected.");
        Shutdown();
    }
}
/*
 *------------------------------------------------------------
 * (Sentinel.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */