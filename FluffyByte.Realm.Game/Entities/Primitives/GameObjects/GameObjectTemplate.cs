/*
 * (GameObjectTemplate.cs)
 *------------------------------------------------------------
 * Created - Tuesday, February 24, 2026@9:40:55 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Shared.Misc;

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

public class GameObjectTemplate
{
    public required string Name { get; set; } = "Unnamed";
    
    public required Guid Id { get; set; } = Guid.NewGuid();
    
    public List<string> Tags { get; set; } = [];
    public required PrimitiveModelType ModelType { get; set; } = PrimitiveModelType.Capsule;
    public required ComplexModelType? ComplexModelType { get; set; }
    public required int FootprintRadius { get; set; } = 1;
}

/*
 *------------------------------------------------------------
 * (GameObjectTemplate.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */