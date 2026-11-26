using System.Collections.Generic;
using UnityEngine;

namespace LabSW.Core
{
    public class BoardState
    {
        public const int BoardSize = 8;
        private CellState[,] grid;

        private Vector2Int pendingGoalPosition;
        public Vector2Int GoalPosition { get; private set; } = new Vector2Int(-1, -1);
        public bool HasGoal { get; private set; } = false;

        public BoardState()
        {
            grid = new CellState[BoardSize, BoardSize];
            ResetBoard();
        }

        public void ResetBoard()
        {
            HasGoal = false;
            GoalPosition = new Vector2Int(-1, -1);

            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    grid[x, y] = CellState.Empty;
                }
            }

            // 4 Zorras en casillas blancas de la Fila 0 (x = 1, 3, 5, 7)
            for (int x = 1; x < BoardSize; x += 2)
            {
                grid[x, 0] = CellState.Fox;
            }

            // Elegir aleatoriamente una de las dos posiciones centrales de salida de las zorras: (3, 0) o (5, 0)
            int centralX = (Random.value < 0.5f) ? 3 : 5;
            pendingGoalPosition = new Vector2Int(centralX, 0);

            // 1 Gallina en casilla blanca de la Fila 7 (x = 6)
            grid[6, 7] = CellState.Chicken;
        }

        public static bool IsValidBounds(int x, int y)
        {
            return x >= 0 && x < BoardSize && y >= 0 && y < BoardSize;
        }

        public static bool IsWhiteCell(int x, int y)
        {
            return (x + y) % 2 != 0;
        }

        public CellState GetCell(Vector2Int pos)
        {
            if (!IsValidBounds(pos.x, pos.y)) return CellState.Empty;
            return grid[pos.x, pos.y];
        }

        public void ExecuteMove(int fromX, int fromY, int toX, int toY)
        {
            if (!IsValidBounds(fromX, fromY) || !IsValidBounds(toX, toY)) return;

            CellState movingPiece = grid[fromX, fromY];

            grid[toX, toY] = movingPiece;
            grid[fromX, fromY] = CellState.Empty;

            // Activar la única Estrella Dorada cuando la zorra abandona la posición central sorteada
            if (!HasGoal &&
                movingPiece == CellState.Fox &&
                fromX == pendingGoalPosition.x &&
                fromY == pendingGoalPosition.y)
            {
                GoalPosition = pendingGoalPosition;
                HasGoal = true;
            }
        }

        public List<Vector2Int> GetFoxPositions()
        {
            List<Vector2Int> foxes = new List<Vector2Int>();
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    if (grid[x, y] == CellState.Fox)
                        foxes.Add(new Vector2Int(x, y));
                }
            }
            return foxes;
        }

        public Vector2Int GetChickenPosition()
        {
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    if (grid[x, y] == CellState.Chicken)
                        return new Vector2Int(x, y);
                }
            }
            return new Vector2Int(-1, -1);
        }
    }
}