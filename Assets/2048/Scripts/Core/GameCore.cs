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
            bool hasMoved = false;
            
            int[] line = new int[_size];
            var origins = new Vector2Int[_size];
            var cells = new Vector2Int[_size];

            for (int i = 0; i < _size; i++)
            {
                // 读取一行或一列
                for (int j = 0; j < _size; j++)
                {
                    int x, y;

                    switch (direction)
                    {
                        case Direction.Left:
                            x = j;
                            y = i;
                            break;

                        case Direction.Right:
                            x = _size - 1 - j;
                            y = i;
                            break;

                        case Direction.Up:
                            x = i;
                            y = _size - 1 - j;
                            break;

                        case Direction.Down:
                            x = i;
                            y = j;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(direction), direction, null);
                    }

                    line[j] = _board[x, y];

                    // 原始位置与目标位置都按移动方向排列
                    origins[j] = new Vector2Int(x, y);
                    cells[j] = new Vector2Int(x, y);
                }

                // 压缩、合并，并记录移动信息
                int[] originalLine = (int[])line.Clone();
                score += ProcessLine(line, origins, cells, moves);

                for (int j = 0; j < _size; j++)
                {
                    if (originalLine[j] != line[j])
                    {
                        hasMoved = true;
                        break;
                    }
                }

                // 按原来的方向写回棋盘
                for (int j = 0; j < _size; j++)
                {
                    int x, y;

                    switch (direction)
                    {
                        case Direction.Left:
                            x = j;
                            y = i;
                            break;

                        case Direction.Right:
                            x = _size - 1 - j;
                            y = i;
                            break;

                        case Direction.Up:
                            x = i;
                            y = _size - 1 - j;
                            break;

                        default: // Down
                            x = i;
                            y = j;
                            break;
                    }

                    _board[x, y] = line[j];
                }
            }
            
            
            if (!hasMoved)
            {
                if (IsSuccessState())
                    EventBus.Publish(new GameSuccessEvent());
                else if (IsBoardLocked())
                    EventBus.Publish(new GameEndEvent());

                return;
            }

            EventBus.Publish(new TileMoveEvent(moves));
            EventBus.Publish(new AddScoreEvent(_score, score));
            _score += score;

            bool generated = RandomizeBoard();
            if (IsSuccessState())
                EventBus.Publish(new GameSuccessEvent());
            else if (!generated && IsBoardLocked())
                EventBus.Publish(new GameEndEvent());
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


        public bool RandomizeBoard()
        {
            if (!GetRandomEmptyBoard(out Vector2Int position))
            {
                return false;
            }
            
            // 生成格子的逻辑：
            // 先 100% 为 2
            _board[position.x, position.y] = 2;
            EventBus.Publish(new TileGenerateEvent(position, 2));
            
            return true;
        }

        private bool GetRandomEmptyBoard(out Vector2Int position)
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

            if (emptyBoards.Count == 0)
            {

                position = new Vector2Int(-1, -1);
                return false;
            }

            position = emptyBoards[Random.Range(0, emptyBoards.Count)];
            return true;
        }

        private bool IsBoardLocked()
        {
            if (GetEmptyBoardCount() > 0)
                return false;

            for (int x = 0; x < _size; x++)
            for (int y = 0; y < _size; y++)
            {
                int value = _board[x, y];
                if (x + 1 < _size && _board[x + 1, y] == value)
                    return false;
                if (y + 1 < _size && _board[x, y + 1] == value)
                    return false;
            }

            return true;
        }

        private bool IsSuccessState()
        {
            if (GetEmptyBoardCount() > 0)
                return false;

            var found = new bool[_size * _size];
            for (int x = 0; x < _size; x++)
            for (int y = 0; y < _size; y++)
            {
                int value = _board[x, y];
                if (value < 2 || (value & (value - 1)) != 0)
                    return false;

                int exponent = 0;
                for (int power = value; power > 1; power >>= 1)
                    exponent++;

                if (exponent < 1 || exponent > found.Length || found[exponent - 1])
                    return false;

                found[exponent - 1] = true;
            }

            for (int i = 0; i < found.Length; i++)
            {
                if (!found[i])
                    return false;
            }

            return true;
        }

        private int GetEmptyBoardCount()
        {
            int count = 0;
            for (int x = 0; x < _size; x++)
            for (int y = 0; y < _size; y++)
            {
                if (_board[x, y] == 0)
                    count++;
            }

            return count;
        }    }
}