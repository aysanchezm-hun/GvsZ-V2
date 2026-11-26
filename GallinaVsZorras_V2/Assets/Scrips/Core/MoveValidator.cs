using System.Collections.Generic;
using UnityEngine;

namespace LabSW.Core
{
    public static class MoveValidator
    {
        public static List<Vector2Int> GetLegalDestinations(BoardState state, Vector2Int from)
        {
            List<Vector2Int> moves = new List<Vector2Int>();
            CellState piece = state.GetCell(from);
            if (piece == CellState.Empty) return moves;

            if (piece == CellState.Fox)
            {
                // Zorras avanzan en diagonal hacia adelante (Y + 1) solo por casillas blancas
                Vector2Int[] dirs = { new Vector2Int(-1, 1), new Vector2Int(1, 1) };
                foreach (var d in dirs)
                {
                    Vector2Int target = from + d;
                    if (BoardState.IsValidBounds(target.x, target.y) && 
                        BoardState.IsWhiteCell(target.x, target.y) && 
                        state.GetCell(target) == CellState.Empty)
                    {
                        moves.Add(target);
                    }
                }
            }
            else if (piece == CellState.Chicken)
            {
                // Gallina mueve en diagonal omnidireccional solo por casillas blancas
                Vector2Int[] dirs = { new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(1, 1) };
                foreach (var d in dirs)
                {
                    Vector2Int target = from + d;
                    if (BoardState.IsValidBounds(target.x, target.y) && 
                        BoardState.IsWhiteCell(target.x, target.y) && 
                        state.GetCell(target) == CellState.Empty)
                    {
                        moves.Add(target);
                    }
                }
            }
            return moves;
        }

        public static bool HasAnyLegalMove(BoardState state, Team team)
        {
            if (team == Team.Zorras)
            {
                foreach (var fox in state.GetFoxPositions())
                {
                    if (GetLegalDestinations(state, fox).Count > 0) return true;
                }
                return false;
            }
            else
            {
                Vector2Int ch = state.GetChickenPosition();
                return ch.x != -1 && GetLegalDestinations(state, ch).Count > 0;
            }
        }
    }
}