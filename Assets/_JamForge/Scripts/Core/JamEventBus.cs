using System;
using System.Collections.Generic;

namespace JamForge
{
    public static class JamEventBus
    {
        private static readonly Dictionary<Type, Delegate> Events = new();

        public static void Subscribe<T>(Action<T> listener)
        {
            if (listener == null)
                return;

            Type type = typeof(T);
            Events[type] = Events.TryGetValue(type, out Delegate existing)
                ? Delegate.Combine(existing, listener)
                : listener;
        }

        public static void Unsubscribe<T>(Action<T> listener)
        {
            if (listener == null)
                return;

            Type type = typeof(T);
            if (!Events.TryGetValue(type, out Delegate existing))
                return;

            Delegate current = Delegate.Remove(existing, listener);
            if (current == null)
                Events.Remove(type);
            else
                Events[type] = current;
        }

        public static void Raise<T>(T evt)
        {
            if (Events.TryGetValue(typeof(T), out Delegate existing))
            {
                (existing as Action<T>)?.Invoke(evt);
            }
        }

        public static void Clear()
        {
            Events.Clear();
        }
    }
}
