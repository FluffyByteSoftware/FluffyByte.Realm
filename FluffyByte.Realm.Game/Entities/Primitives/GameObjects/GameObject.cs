/*
 * (GameObject.cs)
 *------------------------------------------------------------
 * Created - Thursday, February 19, 2026@10:47:50 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;
using FluffyByte.Realm.Tools.Logger;

namespace FluffyByte.Realm.Game.Entities.Primitives.GameObjects;

public class GameObject
{
    public Guid Id { get; }
    public string Name { get; }

    public IGameObjectOwner? Owner { get; private set; }

    public List<string> Tags { get; set; } = [];
    
    private readonly Dictionary<Type, GameObjectComponent> _components = [];

    private readonly Dictionary<TickType, List<GameObjectComponent>> _tickBuckets = new()
    {
        { TickType.Fast, [] },
        { TickType.Normal, [] },
        { TickType.Slow, []}
    };

    private readonly ConcurrentQueue<IGameObjectCommand> _commandQueue = [];

    public GameObject(string name, IGameObjectOwner? owner = null)
    {
        Name = name;
        Owner = owner;

        if(Owner == null)
        {
            Log.Debug($"New game object: {name} is missing an owner.");
        }

        Tags.Add("GameObject");
    }

    /// <summary>
    /// Called when the game object is spawned into the world.
    /// </summary>
    public void OnSpawn()
    {
        foreach (var component in _components.Values)
            component.OnSpawn();
    }

    /// <summary>
    /// Called when the game object is destroyed.
    /// </summary>
    public void OnDestroy()
    {
        foreach (var component in _components.Values)
            component.OnDestroy();

        _components.Clear();

        foreach (var bucket in _tickBuckets.Values)
            bucket.Clear();

        _commandQueue.Clear();
    }

    public void AddComponent<T>(T component) where T : GameObjectComponent
    {
        var type = typeof(T);
        
        if (_components.ContainsKey(type))
            throw new InvalidOperationException(
                $"[GameObject]: {Name} already has a component of type {type.Name}.");

        component.Owner = this;
        _components[type] = component;
        _tickBuckets[component.TickType].Add(component);
    }

    public void RemoveComponent<T>() where T : GameObjectComponent
    {
        var type = typeof(T);

        if (!_components.TryGetValue(type, out var component))
            return;

        _tickBuckets[component.TickType].Remove(component);
        _components.Remove(type);

        component.OnDestroy();
        component.Owner = null;
    }

    public T? GetComponent<T>() where T : GameObjectComponent
    {
        _components.TryGetValue(typeof(T), out var component);
        return component as T;
    }

    public bool HasComponent<T>() where T : GameObjectComponent
    {
        return _components.ContainsKey(typeof(T));
    }

    public void EnqueueCommand(IGameObjectCommand command)
    {
        _commandQueue.Enqueue(command);
    }

    private void DrainCommands()
    {
        while (_commandQueue.TryDequeue(out var command))
            command.Execute(this);
    }

    public void Tick(TickType tickType)
    {
        if (tickType == TickType.Fast)
            DrainCommands();
        
        foreach (var component in _tickBuckets[tickType])
            component.Tick();
    }

    public void TransferOwnership(IGameObjectOwner newOwner)
    {
        Owner?.RemoveGameObject(this);
        Owner = newOwner;
        newOwner.AddGameObject(this);
    }

    public override string ToString()
    {
        return $"GameObject: {Name}[{Id}] ({_components.Count} components)";
    }
}

/*
 *------------------------------------------------------------
 * (GameObject.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */