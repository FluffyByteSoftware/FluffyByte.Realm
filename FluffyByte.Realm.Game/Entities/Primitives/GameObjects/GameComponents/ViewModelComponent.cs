/*
 * (ViewModelComponent.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 22, 2026@8:56:07 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using FluffyByte.Realm.Shared.Misc;

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects.GameComponents;

public class ViewModelComponent(
    PrimitiveModelType primitiveModelType = PrimitiveModelType.Capsule) : GameObjectComponent
{
    public override TickType TickType => TickType.None;

    public PrimitiveModelType ModelType { get; set; } = primitiveModelType;

    public ComplexModelType? RealModelType { get; set; }
    
    public override void Tick()
    {
        
    }

    public override string ToString()
        => $"ViewModelComponent Model = {ModelType}";
}

/*
 *------------------------------------------------------------
 * (ViewModelComponent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */