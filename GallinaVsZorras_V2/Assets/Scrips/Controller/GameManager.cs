using System.Collections.Generic;
using UnityEngine;
using LabSW.Core;
using LabSW.View;

namespace LabSW.Controller
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;

        private BoardState boardState;
        private Team currentTurn = Team.Gallina;
        private GameState gameState = GameState.Playing;

        private Vector2Int selectedTile = new Vector2Int(-1, -1);
        private List<Vector2Int> currentLegalMoves = new List<Vector2Int>();
        private string statusMessage = "";

        private void Start()
        {
            if (boardView == null)
            {
#if UNITY_2023_1_OR_NEWER
                boardView = Object.FindAnyObjectByType<BoardView>();
#else
                boardView = Object.FindObjectOfType<BoardView>();
#endif
            }
            InitializeGame();
        }

        public void InitializeGame()
        {
            boardState = new BoardState();
            currentTurn = Team.Gallina;
            gameState = GameState.Playing;
            selectedTile = new Vector2Int(-1, -1);
            currentLegalMoves.Clear();
            statusMessage = "Turno de la Gallina (Círculo Amarillo)";

            if (boardView != null)
            {
                boardView.RenderBoard();
                boardView.RenderPieces(boardState);
            }
        }

        private void Update()
        {
            if (gameState == GameState.GameOver) return;

            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
        }

        private void HandleMouseClick()
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!boardView.WorldToGrid(mouseWorld, out Vector2Int clickedTile)) return;

            CellState cell = boardState.GetCell(clickedTile);

            // Mover a casilla válida elegida
            if (selectedTile.x != -1 && currentLegalMoves.Contains(clickedTile))
            {
                boardState.ExecuteMove(selectedTile.x, selectedTile.y, clickedTile.x, clickedTile.y);
                boardView.RenderPieces(boardState);
                boardView.ClearHighlights();
                selectedTile = new Vector2Int(-1, -1);
                currentLegalMoves.Clear();

                CheckWinConditions();
                if (gameState == GameState.Playing)
                {
                    SwitchTurn();
                }
                return;
            }

            // Seleccionar pieza del turno activo
            if (IsPieceBelongsToCurrentTurn(cell))
            {
                selectedTile = clickedTile;
                currentLegalMoves = MoveValidator.GetLegalDestinations(boardState, selectedTile);
                boardView.HighlightCells(currentLegalMoves);
            }
            else
            {
                selectedTile = new Vector2Int(-1, -1);
                currentLegalMoves.Clear();
                boardView.ClearHighlights();
            }
        }

        private bool IsPieceBelongsToCurrentTurn(CellState cell)
        {
            return (currentTurn == Team.Gallina && cell == CellState.Chicken) ||
                   (currentTurn == Team.Zorras && cell == CellState.Fox);
        }

        private void SwitchTurn()
        {
            currentTurn = (currentTurn == Team.Gallina) ? Team.Zorras : Team.Gallina;
            statusMessage = (currentTurn == Team.Gallina) ? "Turno de la Gallina (Círculo)" : "Turno de las Zorras (Triángulos)";

            if (!MoveValidator.HasAnyLegalMove(boardState, currentTurn))
            {
                gameState = GameState.GameOver;
                Team winner = (currentTurn == Team.Gallina) ? Team.Zorras : Team.Gallina;
                statusMessage = $"¡FIN DE PARTIDA! Ganador: {winner} (Sin movimientos válidos)";
            }
        }

        private void CheckWinConditions()
        {
            Vector2Int chickenPos = boardState.GetChickenPosition();
            if (boardState.HasGoal && chickenPos == boardState.GoalPosition)
            {
                gameState = GameState.GameOver;
                statusMessage = "¡VICTORIA DE LA GALLINA! Ha alcanzado la Estrella Dorada.";
            }
        }

        private void OnGUI()
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };
            boxStyle.normal.textColor = Color.white;

            GUI.Box(new Rect(10, 10, Screen.width - 20, 45), statusMessage, boxStyle);

            if (GUI.Button(new Rect(10, Screen.height - 45, 140, 35), "Reiniciar MVP"))
            {
                InitializeGame();
            }
        }
    }
}