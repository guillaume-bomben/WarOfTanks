using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfTanks.Cosmetics.FogOfWar
{
    public class FogManager : MonoBehaviour
    {
        public int width, height;

        public Texture2D fogTexture;

        private Color32[] pixels;

        private void Start()
        {
            fogTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            pixels = new Color32[width * height];

            ClearFog();
        }

        void ClearFog()
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(0, 0, 0, 255);
            }

            fogTexture.SetPixels32(pixels);
            fogTexture.Apply();
        }

        public void UpdateFog(List<TankMovement> units)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a < 255)
                {
                    pixels[i] = new Color32(50, 50, 50, 200);
                }
            }

            foreach (var unit in units)
            {
                Reveal(unit.transform.position, unit.GetUnit().stats.visionRange);
            }

            fogTexture.SetPixels32(pixels);
            fogTexture.Apply();
        }

        void Reveal(Vector3 worldPos, float radius)
        {
            Vector2Int center = WorldToGrid(worldPos);
            int r = Mathf.RoundToInt(radius);

            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    int px = center.x + x;
                    int py = center.y + y;

                    if (px < 0 || py < 0 || px >= width || py >= height)
                        continue;

                    if (x * x + y * y > r * r)
                        continue;

                    int index = py * width + px;
                    pixels[index] = new Color32(0, 0, 0, 0);
                }
            }
        }

        Vector2Int WorldToGrid(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt(worldPos.x + width / 2);
            int y = Mathf.RoundToInt(worldPos.y + height / 2);
            return new Vector2Int(x, y);
        }
    }
}

