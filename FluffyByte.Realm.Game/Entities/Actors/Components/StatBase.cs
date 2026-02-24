/*
 * (StatBase.cs)
 *------------------------------------------------------------
 * Created - Monday, February 23, 2026@4:26:35 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.Actors.Components;

public class StatBase
{
    public int Base { get; set; } = 10;
    public int Equipment { get; set; } = 0;
    public int Temporary { get; set; } = 0;

    public int Effective => Math.Clamp(Base + Equipment + Temporary, 1, 30);
    public int Modifier => (Effective - 10) / 2;
}

/*
 *------------------------------------------------------------
 * (StatBase.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */