using System.Collections.Generic;
using UnityEngine;
using LabSW.Core;

namespace LabSW.View
{
    public class BoardView : MonoBehaviour
    {
        [Header("Configuración de Tablero")]
        [SerializeField] private float tileSize = 1.0f;

        [Header("Sprites Asignados")]
        [SerializeField] private Sprite whiteTileSprite;
        [SerializeField] private Sprite darkTileSprite;
        [SerializeField] private Sprite highlightSprite;
        [SerializeField] private Sprite foxTriangleSprite;
        [SerializeField] private Sprite chickenCircleSprite;
        [SerializeField] private Sprite goalStarSprite;

        private GameObject[,] tileObjects = new GameObject[BoardState.BoardSize, BoardState.BoardSize];
        private List<GameObject> highlightObjects = new List<GameObject>();
        private List<GameObject> pieceObjects = new List<GameObject>();
        private GameObject goalObject;

        private Vector3 GetBoardOffset()
        {
            float half = (BoardState.BoardSize - 1) * tileSize * 0.5f;
            return new Vector3(-half, half, 0f);
        }

        public Vector3 GridToWorld(int x, int y)
        {
            Vector3 offset = GetBoardOffset();
            return new Vector3(offset.x + x * tileSize, offset.y - y * tileSize, 0f);
        }

        public bool WorldToGrid(Vector3 worldPos, out Vector2Int gridPos)
        {
            Vector3 offset = GetBoardOffset();
            int x = Mathf.RoundToInt((worldPos.x - offset.x) / tileSize);
            int y = Mathf.RoundToInt((offset.y - worldPos.y) / tileSize);
            gridPos = new Vector2Int(x, y);
            return BoardState.IsValidBounds(x, y);
        }

        public void RenderBoard()
        {
            ClearAllChildren();

            for (int x = 0; x < BoardState.BoardSize; x++)
            {
                for (int y = 0; y < BoardState.BoardSize; y++)
                {
                    GameObject tile = new GameObject($"Tile_{x}_{y}");
                    tile.transform.SetParent(transform);
                    tile.transform.position = GridToWorld(x, y);
                    tile.transform.localScale = Vector3.one * tileSize;

                    SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                    sr.sprite = BoardState.IsWhiteCell(x, y) ? whiteTileSprite : darkTileSprite;
                    sr.sortingOrder = 0;

                    tileObjects[x, y] = tile;
                }
            }
        }

        public void RenderPieces(BoardState state)
        {
            foreach (var p in pieceObjects)
            {
                if (p != null) DestroySafe(p);
            }
            pieceObjects.Clear();

            // Renderizar o actualizar estrella dorada (meta)
            if (state.HasGoal)
            {
                if (goalObject == null)
                {
                    goalObject = new GameObject("Goal_GoldenStar");
                    goalObject.transform.SetParent(transform);
                    goalObject.transform.localScale = Vector3.one * (tileSize * 0.72f);

                    SpriteRenderer goalSr = goalObject.AddComponent<SpriteRenderer>();
                    goalSr.sprite = goalStarSprite;
                    goalSr.sortingOrder = 2; // Encima del tablero (0) y resaltado (1), debajo de la gallina (3)
                }
                goalObject.SetActive(true);
                goalObject.transform.position = GridToWorld(state.GoalPosition.x, state.GoalPosition.y);
            }
            else if (goalObject != null)
            {
                DestroySafe(goalObject);
                goalObject = null;
            }

            // Renderizar piezas
            for (int x = 0; x < BoardState.BoardSize; x++)
            {
                for (int y = 0; y < BoardState.BoardSize; y++)
                {
                    CellState cell = state.GetCell(new Vector2Int(x, y));
                    if (cell == CellState.Empty) continue;

                    GameObject piece = new GameObject(cell == CellState.Fox ? "Fox" : "Chicken");
                    piece.transform.SetParent(transform);
                    piece.transform.position = GridToWorld(x, y);
                    piece.transform.localScale = Vector3.one * (tileSize * 0.78f);

                    SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
                    sr.sprite = (cell == CellState.Fox) ? foxTriangleSprite : chickenCircleSprite;
                    sr.sortingOrder = 3;

                    pieceObjects.Add(piece);
                }
            }
        }

        public void HighlightCells(List<Vector2Int> cells)
        {
            ClearHighlights();
            foreach (var c in cells)
            {
                GameObject hl = new GameObject($"Highlight_{c.x}_{c.y}");
                hl.transform.SetParent(transform);
                hl.transform.position = GridToWorld(c.x, c.y);
                hl.transform.localScale = Vector3.one * tileSize;

                SpriteRenderer sr = hl.AddComponent<SpriteRenderer>();
                sr.sprite = highlightSprite;
                sr.sortingOrder = 1;

                highlightObjects.Add(hl);
            }
        }

        public void ClearHighlights()
        {
            foreach (var hl in highlightObjects)
            {
                if (hl != null) DestroySafe(hl);
            }
            highlightObjects.Clear();
        }

        private void ClearAllChildren()
        {
            ClearHighlights();
            foreach (var p in pieceObjects)
            {
                if (p != null) DestroySafe(p);
            }
            pieceObjects.Clear();

            if (goalObject != null)
            {
                DestroySafe(goalObject);
                goalObject = null;
            }

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroySafe(transform.GetChild(i).gameObject);
            }
        }

        private void DestroySafe(Object obj)
        {
            if (Application.isPlaying) Destroy(obj);
            else DestroyImmediate(obj);
        }
    }
}