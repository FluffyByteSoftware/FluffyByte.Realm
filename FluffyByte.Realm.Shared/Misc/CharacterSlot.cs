/*
 * (CharacterSlot.cs)
 *------------------------------------------------------------
 * Created - Wednesday, February 25, 2026@2:29:09 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System;

namespace FluffyByte.Realm.Shared.Misc;

public class CharacterSlot
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsEmpty => Id == Guid.Empty;

    public static CharacterSlot Empty => new();
}

/*
 *------------------------------------------------------------
 * (CharacterSlot.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */