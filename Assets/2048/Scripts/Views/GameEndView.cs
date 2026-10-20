using System;
using _2048.Scripts.Events;
using _2048.Scripts.Views;
using UnityEngine;
using UnityEngine.UI;

namespace _2048.Scripts.Views
{
    public class GameEndView : MonoBehaviour
    {
        [SerializeField] public CanvasGroup canvasGroup;
        [SerializeField] private Button restartButton;
        private ScoreView _scoreView;

        private void Awake()
        {
            _scoreView = FindFirstObjectByType<ScoreView>();
        }

        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            EventBus.Publish(new GameRestartEvent(_scoreView == null ? 0 : _scoreView.BestScore));
        }

        public void Hide()
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
        }

        public void Show()
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1;
        }
    }
}