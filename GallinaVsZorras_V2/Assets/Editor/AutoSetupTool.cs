using System.IO;
using UnityEditor;
using UnityEngine;
using LabSW.View;
using LabSW.Controller;

public class AutoSetupTool
{
    [MenuItem("LABSW/Construir Escena MVP")]
    public static void BuildMVPScene()
    {
        string artFolder = "Assets/Art/Generated";
        if (!Directory.Exists(artFolder))
        {
            Directory.CreateDirectory(artFolder);
        }

        Sprite whiteTile = CreateSquareSprite($"{artFolder}/cell_white.png", new Color(0.93f, 0.94f, 0.96f, 1f));
        Sprite darkTile = CreateSquareSprite($"{artFolder}/cell_dark.png", new Color(0.24f, 0.27f, 0.33f, 1f));
        Sprite highlightTile = CreateSquareSprite($"{artFolder}/cell_highlight.png", new Color(0.15f, 0.95f, 0.45f, 0.55f));
        Sprite foxSprite = CreateTriangleSprite($"{artFolder}/fox_triangle.png", new Color(0.95f, 0.35f, 0.12f, 1f));
        Sprite chickenSprite = CreateCircleSprite($"{artFolder}/chicken_circle.png", new Color(1.0f, 0.85f, 0.12f, 1f));
        Sprite starSprite = CreateStarSprite($"{artFolder}/goal_star.png", new Color(1.0f, 0.78f, 0.05f, 1f));

        AssetDatabase.Refresh();

        // Limpiar instancias previas si existen
        GameObject existingBoard = GameObject.Find("BoardContainer");
        if (existingBoard != null) Object.DestroyImmediate(existingBoard);

        GameObject existingGM = GameObject.Find("GameManager");
        if (existingGM != null) Object.DestroyImmediate(existingGM);

        // Crear BoardContainer y asignar sprites mediante SerializedObject
        GameObject boardObj = new GameObject("BoardContainer");
        BoardView boardView = boardObj.AddComponent<BoardView>();

        SerializedObject so = new SerializedObject(boardView);
        so.FindProperty("whiteTileSprite").objectReferenceValue = whiteTile;
        so.FindProperty("darkTileSprite").objectReferenceValue = darkTile;
        so.FindProperty("highlightSprite").objectReferenceValue = highlightTile;
        so.FindProperty("foxTriangleSprite").objectReferenceValue = foxSprite;
        so.FindProperty("chickenCircleSprite").objectReferenceValue = chickenSprite;
        so.FindProperty("goalStarSprite").objectReferenceValue = starSprite;
        so.ApplyModifiedPropertiesWithoutUndo();

        // Crear GameManager
        GameObject gmObj = new GameObject("GameManager");
        GameManager gm = gmObj.AddComponent<GameManager>();
        SerializedObject gmSo = new SerializedObject(gm);
        gmSo.FindProperty("boardView").objectReferenceValue = boardView;
        gmSo.ApplyModifiedPropertiesWithoutUndo();

        // Configurar Cámara 2D
        if (Camera.main != null)
        {
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 5.2f;
            Camera.main.transform.position = new Vector3(0f, 0f, -10f);
        }

        // Vista previa inmediata en el editor
        boardView.RenderBoard();
        boardView.RenderPieces(new LabSW.Core.BoardState());

        Debug.Log("[LABSW] Escena MVP con Estrella Dorada construida correctamente.");
    }

    private static Sprite CreateSquareSprite(string path, Color color)
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return SaveAndLoadSprite(tex, path, size);
    }

    private static Sprite CreateCircleSprite(string path, Color color)
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.44f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                tex.SetPixel(x, y, dist <= radius ? color : Color.clear);
            }
        }
        tex.Apply();
        return SaveAndLoadSprite(tex, path, size);
    }

    private static Sprite CreateTriangleSprite(string path, Color color)
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 a = new Vector2(size * 0.5f, size * 0.90f);
        Vector2 b = new Vector2(size * 0.12f, size * 0.14f);
        Vector2 c = new Vector2(size * 0.88f, size * 0.14f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                tex.SetPixel(x, y, PointInTriangle(new Vector2(x, y), a, b, c) ? color : Color.clear);
            }
        }
        tex.Apply();
        return SaveAndLoadSprite(tex, path, size);
    }

    private static Sprite CreateStarSprite(string path, Color color)
    {
        int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float outerR = size * 0.46f;
        float innerR = size * 0.20f;

        Vector2[] vertices = new Vector2[10];
        for (int i = 0; i < 10; i++)
        {
            float angle = Mathf.Deg2Rad * (90f + i * 36f);
            float r = (i % 2 == 0) ? outerR : innerR;
            vertices[i] = center + new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x, y);
                bool inside = false;
                for (int i = 0; i < 10; i++)
                {
                    Vector2 v1 = vertices[i];
                    Vector2 v2 = vertices[(i + 1) % 10];
                    if (PointInTriangle(p, center, v1, v2))
                    {
                        inside = true;
                        break;
                    }
                }
                tex.SetPixel(x, y, inside ? color : Color.clear);
            }
        }
        tex.Apply();
        return SaveAndLoadSprite(tex, path, size);
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = (p.x - b.x) * (a.y - b.y) - (a.x - b.x) * (p.y - b.y);
        float d2 = (p.x - c.x) * (b.y - c.y) - (b.x - c.x) * (p.y - c.y);
        float d3 = (p.x - a.x) * (c.y - a.y) - (c.x - a.x) * (p.y - a.y);
        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
        return !(hasNeg && hasPos);
    }

    private static Sprite SaveAndLoadSprite(Texture2D tex, string path, int ppu)
    {
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = ppu;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}