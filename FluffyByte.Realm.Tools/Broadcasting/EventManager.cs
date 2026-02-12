/*
 * (EventManager.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@7:58:53 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Collections.Concurrent;

namespace FluffyByte.Realm.Tools.Broadcasting;

/// <summary>
/// Provides a centralized event management system that allows objects to subscribe to and publish
/// events without needing direct references to each other.
/// </summary>
public static class EventManager
{
    private static readonly ConcurrentDictionary<Type, List<Delegate>> Subscribers = new();
    private static readonly Lock Lock = new Lock();

    /// <summary>
    /// Subscribes a handler to events of a specific type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to subscribe to.</typeparam>
    /// <param name="handler">The handler method to invoke when the event is published.</param>
    public static void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : EventArgs
    {
        var eventType = typeof(TEvent);

        lock (Lock)
        {
            if (!Subscribers.TryGetValue(eventType, out var handlers))
            {
                handlers = [];
                Subscribers[eventType] = handlers;
            }

            handlers.Add(handler);
        }
    }

    /// <summary>
    /// Unsubscribes a handler from events of a specific type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to unsubscribe from.</typeparam>
    /// <param name="handler">The handler method to remove.</param>
    public static void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : EventArgs
    {
        var eventType = typeof(TEvent);

        lock (Lock)
        {
            if (Subscribers.TryGetValue(eventType, out var handlers))
            {
                handlers.Remove(handler);
         

                if (handlers.Count == 0)
                {
                    Subscribers.TryRemove(eventType, out _);
                }
            }
        }
    }

    /// <summary>
    /// Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event being published.</typeparam>
    /// <param name="eventArgs">The event data to pass to handlers.</param>
    public static void Publish<TEvent>(TEvent eventArgs) where TEvent : EventArgs
    {
        var eventType = typeof(TEvent);

        List<Delegate> handlersCopy;

        lock (Lock)
        {
            if (!Subscribers.TryGetValue(eventType, out var handlers) || handlers.Count == 0)
            {
                return;
            }

            // Create a copy to avoid modification during iteration
            handlersCopy = new List<Delegate>(handlers);
        }

        foreach (var handler in handlersCopy)
        {
            try
            {
                ((Action<TEvent>)handler).Invoke(eventArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex.StackTrace);
            }
        }
    }

    /// <summary>
    /// Gets the number of subscribers for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to check.</typeparam>
    /// <returns>The number of subscribers for the event type.</returns>
    public static int GetSubscriberCount<TEvent>() where TEvent : EventArgs
    {
        var eventType = typeof(TEvent);

        lock (Lock)
        {
            if (Subscribers.TryGetValue(eventType, out var handlers))
            {
                return handlers.Count;
            }
        }

        return 0;
    }

    /// <summary>
    /// Clears all subscribers for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to clear subscribers for.</typeparam>
    public static void ClearSubscribers<TEvent>() where TEvent : EventArgs
    {
        var eventType = typeof(TEvent);

        lock (Lock)
        {
            if (Subscribers.TryRemove(eventType, out _))
            {
                Console.WriteLine($"Cleared all subscribers for event type: {eventType.Name}");
            }
        }
    }

    /// <summary>
    /// Clears all event subscriptions from the EventManager.
    /// </summary>
    public static void ClearAll()
    {
        lock (Lock)
        {
            var count = Subscribers.Count;
            Subscribers.Clear();
            Console.WriteLine($"Cleared all event subscriptions. Removed {count} event type(s)");
        }
    }
}

/*
 *------------------------------------------------------------
 * (EventManager.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */