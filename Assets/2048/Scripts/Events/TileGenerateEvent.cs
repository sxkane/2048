using UnityEngine;

namespace _2048.Scripts.Events
{
    public class TileGenerateEvent : IEvent
    {
        public Vector2Int Position { get; }
        public int Value { get; }
        
        public TileGenerateEvent(Vector2Int position, int value)
        {
            Position = position;
            Value = value;
        }
    }
}