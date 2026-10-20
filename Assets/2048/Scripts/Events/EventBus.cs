using System;
using System.Collections.Generic;

namespace _2048.Scripts.Events
{
    public static class EventBus
    {
        private static Dictionary<Type, Delegate> _eventHandlers = new();

        public static void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            var type = typeof(T);
            if (_eventHandlers.TryGetValue(type, out var handlers))
            {
                _eventHandlers[type] = Delegate.Combine(handlers, handler);
            }
            else
            {
                _eventHandlers[type] = handler;
            }
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            var type = typeof(T);
            if (_eventHandlers.TryGetValue(type, out var handlers))
            {
                handlers = Delegate.Remove(handlers, handler);

                if (handlers == null)
                {
                    _eventHandlers.Remove(type);
                }
                else
                {
                    _eventHandlers[type] = handlers;
                }
            }
        }

        public static void Publish<T>(T message) where T : IEvent
        {
            var  type = typeof(T);
            if (_eventHandlers.TryGetValue(type, out var handlers))
            {
                ((Action<T>)handlers).Invoke(message);
            }
        }

        public static void ClearAll()
        {
            _eventHandlers.Clear();
        }
    }
}