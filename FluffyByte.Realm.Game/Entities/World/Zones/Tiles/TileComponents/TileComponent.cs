/*
 * (TileComponent.cs)
 *------------------------------------------------------------
 * Created - Saturday, February 21, 2026@12:49:56 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Game.Entities.Primitives;

namespace FluffyByte.Realm.Game.Entities.World.Zones.Tiles.TileComponents;

public abstract class TileComponent
{
    #region Owner

    public RealmTile Owner { get; internal set; } = null!;

    #endregion Owner

    #region Tick Type

    public abstract TickType TickType { get; }

    #endregion Tick Type

    #region Lifecycle

    public abstract void OnWarmLoad(TimeSpan elapsed, long missedFastTicks, long missedNormalTicks, long missedSlowTicks);

    public abstract void OnHotLoad();

    public abstract void OnWarmUnload();

    public abstract void OnColdUnload();

    #endregion Lifecycle

    #region Ticks

    public abstract void ActiveTick();

    public abstract void WarmTick();

    #endregion Ticks
}

/*
 *------------------------------------------------------------
 * (TileComponent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */