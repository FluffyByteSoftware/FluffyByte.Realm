/*
 * (GameDirector.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@10:59:34 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Brains.Assistants;
using FluffyByte.Realm.Game.Entities.Actors;
using FluffyByte.Realm.Game.Entities.Primitives;
using FluffyByte.Realm.Game.Entities.World;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Brains;

public static class GameDirector
{
    #region Assistants

    private static WorldComposer _composer = null!;
    private static Metronome _metronome = null!;
    private static ActorRegistrar _actorReg = null!;
    #endregion Assistants
    
    #region Config

    public static GameDirectorConfig Config { get; private set; } = null!;
    private static GameDirectorEditor _editor = null!;

    #endregion Config
    
    #region World

    public static RealmWorld World { get; private set; } = null!;
    
    #endregion World
    
    #region Initialization

    private static bool _isInitialized;
    
    public static void Initialize()
    {

        if (_isInitialized) return;
        
        _editor = new GameDirectorEditor();
        Config = _editor.Load();
        World = new RealmWorld();
        _composer = new WorldComposer(World);
        _metronome = new Metronome();
        _actorReg = new ActorRegistrar(_composer);

        _isInitialized = true;
        
        EventManager.Subscribe<SystemStartupEvent>(OnSystemStartup);
        EventManager.Subscribe<SystemShutdownEvent>(OnSystemShutdown);
    }
    #endregion Initialization
    
    #region Lifecycle Events

    private static void OnSystemStartup(SystemStartupEvent e)
    {
        if (!_isInitialized) return;
        
        World.OnLoad();
        _metronome.Start();
        
        Log.Debug($"[GameDirector]: Initialized.");
    }

    private static void OnSystemShutdown(SystemShutdownEvent e)
    {
        if(!_isInitialized) return;

        _metronome.Stop();
        _composer.OnUnload();
        World.OnUnload();
        _editor.Save(Config);
        
        _editor = null!;
        Config = null!;
        World = null!;
        _composer = null!;
        _metronome = null!;
        _actorReg = null!;
     
        _isInitialized = false;
        
        EventManager.Unsubscribe<SystemStartupEvent>(OnSystemStartup);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnSystemShutdown);
    }
    #endregion Lifecycle Events
    
    #region Actor Registration
    public static void RegisterActor(IUniqueActor actor, RealmTile startingTile)
        => _actorReg.RegisterUnique(actor, startingTile);

    public static void UnregisterActor(IUniqueActor actor)
        => _actorReg.UnregisterUnique(actor);

    public static void OnAgentMoved(IUniqueActor actor, RealmTile newTile)
        => _actorReg.OnUniqueActorMoved(actor, newTile);
    #endregion Actor Registration
    
    #region Config Management

    public static void UpdateConfig(Action<GameDirectorConfig> mutation)
    {
        mutation(Config);
        _editor.Save(Config);
    }
    #endregion Config Management
    
    #region Tick Handlers

    
    public static void ActiveTick(TickType tickType)
    {
        _actorReg.UpdateIfNeeded();
        _composer.ActiveTick(tickType);
    }

    public static void WarmTick(TickType tickType)
        => _composer.WarmTick(tickType);
    
    public static long FastTickCount => _metronome.FastTickCount;
    public static long NormalTickCount => _metronome.NormalTickCount;
    public static long SlowTickCount => _metronome.SlowTickCount;
    
    #endregion Tick Handlers
    
    #region Diagnostics

    public static int ActorCount => _actorReg.UniqueActorCount;
    public static int HotCount => _composer.HotCount;
    public static int WarmCount => _composer.WarmCount;

    public static string Status()
        => $"GameDirector Agents={ActorCount} Hot={HotCount} Warm={WarmCount}";
    #endregion Diagnostics
}


/*
 *------------------------------------------------------------
 * (GameDirector.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */