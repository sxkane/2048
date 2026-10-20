using System;
using System.Collections.Generic;
using _2048.Scripts.Events;
using _2048.Scripts.Values;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _2048.Scripts.Core
{
    public class GameCore
    {
        private int[,] _board;
        private int _score;
        private int _size = 4;

        public GameCore()
        {
            _board = new int[_size, _size];
            _score = 0;
        }

        public void ReStart()
        {
            _board = new int[_size, _size];
            _score = 0;
            
            RandomizeBoard();
            RandomizeBoard();
        }

        public void Move(Direction direction)
        {
            var moves = new List<TileMove>();
            int score = 0;
            
            int[] line = new int[_size];
            var origins = new Vector2Int[_size];
            var cells = new Vector2Int[_size];

            for (int i = 0; i < _size; i++)
            {
                // 读取一行或一列
                for (int j = 0; j < _size; j++)
                {
                    int row, col;

                    switch (direction)
                    {
                        case Direction.Left:
                            row = i;
                            col = j;
                            break;

                        case Direction.Right:
                            row = i;
                            col = _size - 1 - j;
                            break;

                        case Direction.Up:
                            row = j;
                            col = i;
                            break;

                        case Direction.Down:
                            row = _size - 1 - j;
                            col = i;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(direction), direction, null);
                    }

                    line[j] = _board[row, col];

                    // 原始位置与目标位置都按移动方向排列
                    origins[j] = new Vector2Int(col, row);
                    cells[j] = new Vector2Int(col, row);
                }

                // 压缩、合并，并记录移动信息
                score += ProcessLine(line, origins, cells, moves);

                // 按原来的方向写回棋盘
                for (int j = 0; j < _size; j++)
                {
                    int row, col;

                    switch (direction)
                    {
                        case Direction.Left:
                            row = i;
                            col = j;
                            break;

                        case Direction.Right:
                            row = i;
                            col = _size - 1 - j;
                            break;

                        case Direction.Up:
                            row = j;
                            col = i;
                            break;

                        default: // Down
                            row = _size - 1 - j;
                            col = i;
                            break;
                    }

                    _board[row, col] = line[j];
                }
            }
            
            
            EventBus.Publish(new TileMoveEvent(moves));
            EventBus.Publish(new AddScoreEvent(_score, score));
            
            _score += score;
        }

        private int ProcessLine(int[] line, Vector2Int[] origins, Vector2Int[] cells, List<TileMove> moves)
        {
            int score = 0;
            int count = 0;

            // 把非零数字压缩到前面，同时保留原始坐标
            for (int i = 0; i < _size; i++)
            {
                if (line[i] == 0)
                    continue;

                line[count] = line[i];
                origins[count] = origins[i];
                count++;
            }

            // 合并
            int write = 0;

            for (int read = 0; read < count;)
            {
                int value = line[read];
                Vector2Int to = cells[write];

                if (read + 1 < count && value == line[read + 1])
                {
                    int resultValue = value * 2;
                    score += resultValue;
                    
                    moves.Add(new TileMove(origins[read], to, true, resultValue));

                    moves.Add(new TileMove(origins[read + 1], to, true, resultValue));

                    line[write++] = resultValue;
                    read += 2;
                }
                else
                {
                    moves.Add(new TileMove(origins[read], to, false, value));

                    line[write++] = value;
                    read++;
                }
            }

            while (write < _size)
                line[write++] = 0;

            return score;
        }


        public void RandomizeBoard()
        {
            var position = GetRandomEmptyBoard();

            // 生成格子的逻辑：
            // 先 100% 为 2
            _board[position[0], position[1]] = 2;
            
            EventBus.Publish(new TileGenerateEvent(position, 2));
        }

        private Vector2Int GetRandomEmptyBoard()
        {
            var emptyBoards = new List<Vector2Int>();

            for (var i = 0; i < _board.GetLength(0); i++)
            for (var j = 0; j < _board.GetLength(1); j++)
            {
                if (_board[i, j] == 0)
                {
                    emptyBoards.Add(new Vector2Int(i, j));
                }
            }

            return emptyBoards[Random.Range(0, emptyBoards.Count)];
        }
    }
}