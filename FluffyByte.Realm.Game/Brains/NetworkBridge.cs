/*
 * (NetworkBridge.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 26, 2026@5:38:16 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Actors.Components;
using FluffyByte.Realm.Game.Entities.Actors.Events;
using FluffyByte.Realm.Game.Entities.Actors.Players;
using FluffyByte.Realm.Game.Entities.Events;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects;
using FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;
using FluffyByte.Realm.Game.Entities.World.Zones.Tiles;
using FluffyByte.Realm.Shared.Misc;
using FluffyByte.Realm.Shared.PacketTypes;
using FluffyByte.Realm.Tools.Broadcasting;
using FluffyByte.Realm.Tools.Broadcasting.Events;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Brains;

public static class NetworkBridge
{
    private static bool _isInitialized;

    public static void Initialize()
    {
        if (_isInitialized) return;

        EventManager.Subscribe<ActorTilesChangedEvent>(OnActorTilesChanged);
        EventManager.Subscribe<RealmTileEnterTileEvent>(OnTileEntered);
        EventManager.Subscribe<RealmTileExitTileEvent>(OnTileExited);
        EventManager.Subscribe<ActorDiedEvent>(OnActorDied);
        EventManager.Subscribe<SystemShutdownEvent>(OnShutdown);

        _isInitialized = true;
    }

    private static void OnShutdown(SystemShutdownEvent e)
    {
        if (!_isInitialized) return;
        _isInitialized = false;

        EventManager.Unsubscribe<ActorTilesChangedEvent>(OnActorTilesChanged);
        EventManager.Unsubscribe<RealmTileEnterTileEvent>(OnTileEntered);
        EventManager.Unsubscribe<RealmTileExitTileEvent>(OnTileExited);
        EventManager.Unsubscribe<ActorDiedEvent>(OnActorDied);
        EventManager.Unsubscribe<SystemShutdownEvent>(OnShutdown);
    }

    #region Tiles Changed (Snapshot + Diff)

    private static void OnActorTilesChanged(ActorTilesChangedEvent e)
    {
        var playerComp = e.Actor.GetComponent<PlayerComponent>();
        if (playerComp?.Client == null) return;

        if (e.IsFirstRefresh)
        {
            SendFullSnapshot(playerComp, e.Actor, e.HotTiles, e.WarmTiles);
            return;
        }

        SendTileDiff(playerComp, e);
    }

    private static void SendFullSnapshot(PlayerComponent playerComp, GameObject self,
        IReadOnlySet<RealmTile> hotTiles, IReadOnlySet<RealmTile> warmTiles)
    {
        foreach (var tile in hotTiles)
            SendOccupantsOnTile(playerComp, self, tile);

        foreach (var tile in warmTiles)
            SendOccupantsOnTile(playerComp, self, tile);
    }

    private static void SendTileDiff(PlayerComponent playerComp, ActorTilesChangedEvent e)
    {
        // Tiles we can now see but couldn't before — send spawns
        foreach (var tile in e.HotTiles)
        {
            if (!e.PreviousHotTiles.Contains(tile) && !e.PreviousWarmTiles.Contains(tile))
                SendOccupantsOnTile(playerComp, e.Actor, tile);
        }

        foreach (var tile in e.WarmTiles)
        {
            if (!e.PreviousHotTiles.Contains(tile) && !e.PreviousWarmTiles.Contains(tile))
                SendOccupantsOnTile(playerComp, e.Actor, tile);
        }

        // Tiles we could see but can't anymore — send despawns
        foreach (var tile in e.PreviousHotTiles)
        {
            if (!e.HotTiles.Contains(tile) && !e.WarmTiles.Contains(tile))
                SendDespawnsOnTile(playerComp, tile);
        }

        foreach (var tile in e.PreviousWarmTiles)
        {
            if (!e.HotTiles.Contains(tile) && !e.WarmTiles.Contains(tile))
                SendDespawnsOnTile(playerComp, tile);
        }
    }

    private static void SendOccupantsOnTile(PlayerComponent playerComp, GameObject self, RealmTile tile)
    {
        if (tile.Agent != null && tile.Agent.HasComponent<PlayerComponent>())
            SendSpawnPacket(playerComp, tile.Agent, tile.Agent == self);

        foreach (var item in tile.Items)
            SendSpawnPacket(playerComp, item, false);
    }

    private static void SendDespawnsOnTile(PlayerComponent playerComp, RealmTile tile)
    {
        if (tile.Agent != null)
            SendDespawnPacket(playerComp, tile.Agent);

        foreach (var item in tile.Items)
            SendDespawnPacket(playerComp, item);
    }

    #endregion Tiles Changed

    #region World Events

    private static void OnTileEntered(RealmTileEnterTileEvent e)
    {
        var observers = GameDirector.GetObservers(e.Tile);

        foreach (var observer in observers)
        {
            var playerComp = observer.GetComponent<PlayerComponent>();
            if (playerComp?.Client == null) continue;

            SendSpawnPacket(playerComp, e.ObjectEntering, e.ObjectEntering == observer);
        }
    }

    private static void OnTileExited(RealmTileExitTileEvent e)
    {
        var observers = GameDirector.GetObservers(e.Tile);

        foreach (var observer in observers)
        {
            var playerComp = observer.GetComponent<PlayerComponent>();
            if (playerComp?.Client == null) continue;

            SendDespawnPacket(playerComp, e.ObjectLeaving);
        }
    }

    private static void OnActorDied(ActorDiedEvent e)
    {
        var tile = e.Actor.GetComponent<TransformComponent>()?.Tile;
        if (tile == null) return;

        var observers = GameDirector.GetObservers(tile);

        foreach (var observer in observers)
        {
            var playerComp = observer.GetComponent<PlayerComponent>();
            if (playerComp?.Client == null) continue;

            playerComp.Client.SendPacket(PacketType.ServerActorDied, new ServerActorDiedPacket
            {
                NetworkId = e.Actor.NetworkId
            });
        }
    }

    #endregion World Events

    #region Packet Builders

    private static void SendSpawnPacket(PlayerComponent playerComp, GameObject actor, bool isSelf)
    {
        var transform = actor.GetComponent<TransformComponent>();
        var health = actor.GetComponent<Health>();
        var viewModel = actor.GetComponent<ViewModelComponent>();

        if (viewModel == null)
        {
            Log.Warn($"Attempted to send spawn packet for actor {actor.Name} (NetworkId: {actor.NetworkId}) without a ViewModelComponent. Skipping.");
            return;
        }

        playerComp.Client!.SendPacket(PacketType.ServerSpawnActor, new ServerSpawnActorPacket
        {
            NetworkId = actor.NetworkId,
            Name = actor.Name,
            GlobalX = transform?.Tile?.GlobalX ?? 0,
            GlobalZ = transform?.Tile?.GlobalZ ?? 0,
            Rotation = transform?.Rotation ?? 0f,
            ModelType = viewModel.ModelType,
            ComplexModelType = viewModel.RealModelType ?? 0,
            CurrentHealth = health?.Current ?? 0,
            MaxHealth = health?.Max ?? 0,
            IsSelf = isSelf
        });
    }

    private static void SendDespawnPacket(PlayerComponent playerComp, GameObject actor)
    {
        playerComp.Client!.SendPacket(PacketType.ServerDespawnActor, new ServerDespawnActorPacket
        {
            NetworkId = actor.NetworkId
        });
    }

    #endregion Packet Builders
}
/*
 *------------------------------------------------------------
 * (NetworkBridge.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */