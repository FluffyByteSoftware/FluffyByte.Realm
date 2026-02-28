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

    public virtual void OnWarmLoad(TimeSpan elapsed, long missedFastTicks, long missedNormalTicks, long missedSlowTicks) { }


    public virtual void OnHotLoad() { }

    public virtual void OnWarmUnload() { }

    public virtual void OnColdUnload() { }

    #endregion Lifecycle

    #region Ticks

    public virtual void ActiveTick() { }

    public virtual void WarmTick() { }

    #endregion Ticks
}

/*
 *------------------------------------------------------------
 * (TileComponent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */