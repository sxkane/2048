using _2048.Scripts.Core;
using _2048.Scripts.Events;
using _2048.Scripts.Input;
using _2048.Scripts.Saving;
using _2048.Scripts.Values;
using _2048.Scripts.Views;
using UnityEngine;

namespace _2048.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private BoardView boardView;

        private GameCore _gameCore;
        private SaveManager _saveManager;

        private void Awake()
        {
            _gameCore = new GameCore();
            _saveManager = GetComponent<SaveManager>();
        }

        private void OnEnable()
        {
            inputHandler.OnMove += HandleMove;
            EventBus.Subscribe<GameRestartEvent>(OnGameRestart);
        }

        private void OnDisable()
        {
            inputHandler.OnMove -= HandleMove;
            EventBus.Unsubscribe<GameRestartEvent>(OnGameRestart);
        }

        private void Start()
        {
            int bestScore = _saveManager == null ? 0 : _saveManager.Load();
            EventBus.Publish(new GameRestartEvent(bestScore));
            Debug.Log("Game started");
        }

        private void HandleMove(Direction direction)
        {
            if (boardView != null && boardView.IsAnimating)
                return;

            _gameCore.Move(direction);
        }

        private void OnGameRestart(GameRestartEvent gameRestartEvent)
        {
            boardView?.ResetBoard();
            _gameCore.ReStart();
        }
    }
}