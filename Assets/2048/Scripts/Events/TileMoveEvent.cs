using System.Collections.Generic;
using _2048.Scripts.Values;

namespace _2048.Scripts.Events
{
    public class TileMoveEvent : IEvent
    {
        public List<TileMove> TileMoves { get; }
        
        public  TileMoveEvent(List<TileMove> tileMoves)
        {
            TileMoves = tileMoves;
        }
    }
}