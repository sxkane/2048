using System;
using _2048.Scripts.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2048.Scripts.Views
{
    public class ViewManager : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;
        [SerializeField] private ScoreView scoreView;
        [SerializeField] private GameEndView gameEndView;
        [SerializeField] private Button restartButton;

        private void OnEnable()
        {
            EventBus.Subscribe<TileMoveEvent>(OnTileMoved);
            EventBus.Subscribe<TileGenerateEvent>(OnTileGenerated);
            EventBus.Subscribe<AddScoreEvent>(OnScoreChanged);
            EventBus.Subscribe<GameRestartEvent>(OnGameRestart);
            EventBus.Subscribe<GameEndEvent>(OnGameEnd);
            EventBus.Subscribe<GameSuccessEvent>(OnGameSuccess);
            restartButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<TileMoveEvent>(OnTileMoved);
            EventBus.Unsubscribe<TileGenerateEvent>(OnTileGenerated);
            EventBus.Unsubscribe<AddScoreEvent>(OnScoreChanged);
            EventBus.Unsubscribe<GameRestartEvent>(OnGameRestart);
            EventBus.Unsubscribe<GameEndEvent>(OnGameEnd);
            EventBus.Unsubscribe<GameSuccessEvent>(OnGameSuccess);
            restartButton.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnTileMoved(TileMoveEvent tileMoveEvent)
        {
            boardView.PlayMoves(tileMoveEvent.TileMoves);
        }

        private void OnTileGenerated(TileGenerateEvent tileGenerateEvent)
        {
            boardView.GenerateTile(tileGenerateEvent.Position, tileGenerateEvent.Value);
        }
        
        private void OnScoreChanged(AddScoreEvent addScoreEvent)
        {
            scoreView.RefreshScore(addScoreEvent.Score + addScoreEvent.AddScore);
        }

        private void OnGameRestart(GameRestartEvent gameRestartEvent)
        {
            scoreView.InitScore(0);
            gameEndView.Hide();
        }

        private void OnButtonClicked()
        {
            EventBus.Publish(new GameRestartEvent());
        }

        private void OnGameEnd(GameEndEvent gameEndEvent)
        {
            gameEndView.Show();
        }

        private void OnGameSuccess(GameSuccessEvent gameSuccessEvent)
        {
            gameEndView.Show();
        }
    }
}