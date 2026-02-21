/*
 * (GameObjectComponent.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 19, 2026@10:48:01 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Game.Entities.Primitives;

/// <summary>
/// Represents the base class for components that can be attached to a <see cref="GameObject"/>.
/// This class provides functionality for initialization, destruction, and tick-based processing.
/// Subclasses should define specific behaviors and override methods as required to implement custom logic.
/// </summary>
/// <remarks>
/// Components derived from this class are intended to be lightweight and modular entities that encapsulate
/// particular behaviors or properties of a <see cref="GameObject"/>. Each component can dictate the type
/// of game loop step it participates in by specifying a <see cref="TickType"/>.
/// </remarks>
public abstract class GameObjectComponent
{
    /// <summary>
    /// Gets or sets the GameObject to which this component is attached.
    /// This property references the owning GameObject instance that contains the component,
    /// providing access to the parent game object's context and attributes.
    /// The property is managed internally, and it is updated automatically by GameObject
    /// when components are added, removed, or destroyed.
    /// </summary>
    public GameObject? Owner { get; internal set; }

    /// <summary>
    /// Gets the tick rate category associated with this component.
    /// This property determines how frequently the component's <see cref="Tick"/> method is invoked
    /// during the game loop. The tick rate is categorized into predefined intervals such as
    /// <see cref="TickType.Fast"/>, <see cref="TickType.Normal"/>, and <see cref="TickType.Slow"/>.
    /// Components with different tick rates are executed in their respective intervals to optimize
    /// game performance and balance workload.
    /// </summary>
    public abstract TickType TickType { get; }

    /// <summary>
    /// Executes logic specific to the component when the associated game object is spawned into the world.
    /// This method is called automatically for every component attached to a game object during its initialization.
    /// Subclasses can override this method to implement any setup or preparation needed when the game object
    /// becomes active.
    /// </summary>
    public virtual void OnSpawn()
    {
    }

    /// <summary>
    /// Performs cleanup and teardown logic for the component before it is permanently destroyed.
    /// This method is invoked when the associated game object is being destroyed or when the component
    /// is removed from the game object. Subclasses can override this method to release resources
    /// or execute any custom destruction behavior specific to the component.
    /// </summary>
    public virtual void OnDestroy()
    {
        
    }

    /// <summary>
    /// Executes the main functionality of the GameObjectComponent during the game loop.
    /// This method is called based on the associated <see cref="TickType"/> of the component.
    /// Derived classes must implement this method to define the specific behavior of the component
    /// during a tick cycle.
    /// </summary>
    public abstract void Tick();
}

/*
 *------------------------------------------------------------
 * (GameObjectComponent.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */