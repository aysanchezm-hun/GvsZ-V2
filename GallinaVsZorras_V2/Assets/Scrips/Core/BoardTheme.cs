using UnityEngine;

namespace LabSW.Core
{
    [System.Serializable]
    public struct BoardTheme
    {
        public string Name { get; set; }
        public Color LightTileColor { get; set; }
        public Color DarkTileColor { get; set; }
        public Color FoxColor { get; set; }
        public Color ChickenColor { get; set; }
        public Color HighlightColor { get; set; }

        public BoardTheme(string name, Color lightTile, Color darkTile, Color fox, Color chicken, Color highlight)
        {
            Name = name;
            LightTileColor = lightTile;
            DarkTileColor = darkTile;
            FoxColor = fox;
            ChickenColor = chicken;
            HighlightColor = highlight;
        }

        // Paletas Predefinidas
        public static BoardTheme[] GetDefaultThemes()
        {
            return new BoardTheme[]
            {
                new BoardTheme(
                    "Clásico MVP",
                    new Color(0.95f, 0.95f, 0.92f), // Blanco hueso
                    new Color(0.35f, 0.38f, 0.42f), // Gris pizarra
                    new Color(0.95f, 0.40f, 0.10f), // Naranja (Zorras)
                    new Color(1.00f, 0.88f, 0.20f), // Amarillo (Gallina)
                    new Color(0.20f, 0.95f, 0.30f, 0.55f) // Verde neón
                ),
                new BoardTheme(
                    "Neón Ciberpunk",
                    new Color(0.12f, 0.12f, 0.20f), // Azul noche oscuro
                    new Color(0.05f, 0.05f, 0.10f), // Negro profundo
                    new Color(1.00f, 0.10f, 0.50f), // Rosa Neón (Zorras)
                    new Color(0.10f, 0.90f, 1.00f), // Cyan Neón (Gallina)
                    new Color(1.00f, 0.90f, 0.00f, 0.70f) // Amarillo Neón
                ),
                new BoardTheme(
                    "Madera Rústica",
                    new Color(0.88f, 0.77f, 0.60f), // Madera clara
                    new Color(0.50f, 0.32f, 0.18f), // Madera oscura
                    new Color(0.70f, 0.15f, 0.10f), // Rojo arcilla (Zorras)
                    new Color(0.98f, 0.95f, 0.80f), // Crema suave (Gallina)
                    new Color(0.40f, 0.80f, 0.40f, 0.60f) // Verde bosque
                ),
                new BoardTheme(
                    "Alto Contraste",
                    new Color(1.00f, 1.00f, 1.00f), // Blanco puro
                    new Color(0.10f, 0.10f, 0.10f), // Negro puro
                    new Color(0.90f, 0.00f, 0.00f), // Rojo intesificado (Zorras)
                    new Color(1.00f, 0.95f, 0.00f), // Amarillo puro (Gallina)
                    new Color(0.00f, 0.80f, 1.00f, 0.75f) // Azul cyan
                )
            };
        }
    }
}