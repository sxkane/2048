namespace _2048.Scripts.Events
{
    public class GameRestartEvent : IEvent
    {
        public int BestScore { get; }

        public GameRestartEvent(int bestScore = 0)
        {
            BestScore = bestScore;
        }
    }
}