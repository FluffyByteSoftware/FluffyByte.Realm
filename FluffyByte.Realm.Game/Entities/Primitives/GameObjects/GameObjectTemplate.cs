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
    public required string Name { get; init; } = "Unnamed";
    
    public required Guid Id { get; init; } = Guid.NewGuid();
    
    public List<string> Tags { get; init; } = [];
    public required PrimitiveModelType ModelType { get; init; } = PrimitiveModelType.Capsule;
    public required ComplexModelType? ComplexModelType { get; init; }
    public required int FootprintRadius { get; init; } = 1;
}

/*
 *------------------------------------------------------------
 * (GameObjectTemplate.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */