using System;
using _2048.Scripts.Events;
using TMPro;
using UnityEngine;

namespace _2048.Scripts.Views
{
    public class ViewManager : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;
        [SerializeField] private ScoreView scoreView;

        private void OnEnable()
        {
            EventBus.Subscribe<TileMoveEvent>(OnTileMoved);
            EventBus.Subscribe<TileGenerateEvent>(OnTileGenerated);
            EventBus.Subscribe<AddScoreEvent>(OnScoreChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<TileMoveEvent>(OnTileMoved);
            EventBus.Unsubscribe<TileGenerateEvent>(OnTileGenerated);
            EventBus.Unsubscribe<AddScoreEvent>(OnScoreChanged);
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
    }
}