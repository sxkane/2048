namespace _2048.Scripts.Events
{
    public class AddScoreEvent : IEvent
    {
        public int Score { get; }
        
        public AddScoreEvent(int score)
        {
            Score = score;
        } 
    }
}