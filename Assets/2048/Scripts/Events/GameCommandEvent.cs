using System.Collections.Generic;
using _2048.Scripts.Values;

namespace _2048.Scripts.Events
{
    public class GameCommandEvent : IEvent
    {
        public List<TileMove> TileMoves { get; }
        
        public  GameCommandEvent(List<TileMove> tileMoves)
        {
            TileMoves = tileMoves;
        }
    }
}