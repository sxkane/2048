using System;
using _2048.Scripts.Events;
using UnityEngine;
using UnityEngine.UI;

namespace _2048.Scripts.Views
{
    public class GameEndView : MonoBehaviour
    {
        [SerializeField] public CanvasGroup canvasGroup;
        [SerializeField] private Button restartButton;

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
            EventBus.Publish(new GameRestartEvent());
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