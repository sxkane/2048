namespace _2048.Scripts.Events
{
    public class AddScoreEvent : IEvent
    {
        public int Score { get; }
        public int AddScore { get; }

        public AddScoreEvent(int score, int addScore)
        {
            Score = score;
            AddScore = addScore;
        } 
    }
}