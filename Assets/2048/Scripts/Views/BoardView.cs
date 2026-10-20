using System;
using System.Collections;
using System.Collections.Generic;
using _2048.Scripts.Values;
using UnityEngine;

namespace _2048.Scripts.Views
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private Transform[] boards;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform tileParent;
        
        private TileView[,] _tiles = new TileView[4, 4];

        public void GenerateTile(Vector2Int position, int value)
        {
            var go = Instantiate(tilePrefab, tileParent);
            var tile = go.GetComponent<TileView>();

            tile.SetTile(value, GetTileColor(value));
            tile.SpawnTile(GetPosition(position));

            _tiles[position.x, position.y] = tile;
        }
        
        public void PlayMoves(List<TileMove> moves)
        {
            StartCoroutine(PlayMovesRoutine(moves));
        }

        private IEnumerator PlayMovesRoutine(List<TileMove> moves)
        {
            // 先保存所有移动的源方块，避免更新映射时丢失引用。
            TileView[] movingTiles = new TileView[moves.Count];
            int completed = 0;

            for (int i = 0; i < moves.Count; i++)
            {
                var move = moves[i];
                movingTiles[i] = _tiles[move.From.x, move.From.y];
            }

            // 同时启动所有移动动画。
            for (int i = 0; i < moves.Count; i++)
            {
                if (movingTiles[i] == null)
                {
                    completed++;
                    continue;
                }

                var move = moves[i];

                movingTiles[i].MoveTile(GetPosition(move.To), () => completed++);
            }

            // 等待所有移动动画结束。
            while (completed < moves.Count)
            {
                yield return null;
            }

            // 所有动画结束后，再统一构建新的棋盘映射。
            var nextTiles = new TileView[4, 4];

            for (int i = 0; i < moves.Count; i++)
            {
                var move = moves[i];
                var tile = movingTiles[i];

                if (tile == null)
                    continue;

                int x = move.To.x;
                int y = move.To.y;

                if (move.IsMerge && nextTiles[x, y] != null)
                {
                    Destroy(tile.gameObject);
                    continue;
                }

                nextTiles[x, y] = tile;

                if (move.IsMerge)
                {
                    tile.SetTile(move.ResultValue, GetTileColor(move.ResultValue));
                    tile.PlayMergeAnimation();
                }
            }

            _tiles = nextTiles;
        }
        
        private Color GetTileColor(int value)
        {
            return value switch
            {
                2    => new Color32(238, 228, 218, 255),
                4    => new Color32(237, 224, 200, 255),
                8    => new Color32(242, 177, 121, 255),
                16   => new Color32(245, 149,  99, 255),
                32   => new Color32(246, 124,  95, 255),
                64   => new Color32(246,  94,  59, 255),
                128  => new Color32(237, 207, 114, 255),
                256  => new Color32(237, 204,  97, 255),
                512  => new Color32(237, 200,  80, 255),
                1024 => new Color32(237, 197,  63, 255),
                2048 => new Color32(237, 194,  46, 255),
                _    => new Color32(60, 58, 50, 255)
            };
        }

        private Vector3 GetPosition(Vector2Int position)
        {
            return boards[position.x *  4 + position.y].position;
        }
    }
}