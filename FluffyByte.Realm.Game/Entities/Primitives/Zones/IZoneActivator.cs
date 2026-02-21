/*
 * (IZoneActivator.cs)
 *------------------------------------------------------------
 * Created - Friday, February 20, 2026@4:15:33 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.Primitives;

/// <summary>
/// Any GameObject, which implements this interface, will keep a RealmZone active (and wake its
/// neighbors) when present. Implement on players, significant NPCs, like world bosses, and any other entity that should
/// prevent a zone from sleeping.
/// </summary>
public interface IZoneActivator
{
    
}

/*
 *------------------------------------------------------------
 * (IZoneActivator.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */