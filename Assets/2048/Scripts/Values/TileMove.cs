using UnityEngine;

namespace _2048.Scripts.Values
{
    public struct TileMove
    {
        public Vector2Int From { get; }
        public Vector2Int To { get; }
        public bool IsMerge { get; }
        public int ResultValue { get; }

        public TileMove(Vector2Int from, Vector2Int to, bool isMerge, int resultValue)
        {
            From = from;
            To = to;
            IsMerge = isMerge;
            ResultValue = resultValue;
        }
    }
}