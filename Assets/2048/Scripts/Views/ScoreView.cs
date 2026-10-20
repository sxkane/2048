using TMPro;
using UnityEngine;

namespace _2048.Scripts.Views
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentScore;
        [SerializeField] private TextMeshProUGUI bestScore;
        
        private int _score;
        private int _bestScore;

        public void InitScore(int best)
        {
            _score = 0;
            _bestScore = best;
            currentScore.text = "0";
            bestScore.text = bestScore.ToString();
        }

        public void RefreshScore(int score)
        {
            if (_score == score)
                return;
            
            _score = score;
            currentScore.text = _score.ToString();

            if (_score > _bestScore)
            {
                _bestScore = _score;
                bestScore.text = _bestScore.ToString();
            }
        }
    }
}