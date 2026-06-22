using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WarOfTanks.Cosmetics.FogOfWar
{
    public class FogManager : MonoBehaviour
    {
        public static FogManager Instance;

        public int width, height;
        public Texture2D fogTexture;
        public Tilemap worldMap;

        private Color32[] pixels;
        public MeshRenderer mr;
        private Material mat;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {            
            fogTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            pixels = new Color32[width * height];

            ClearFog();

            mr = GetComponent<MeshRenderer>();

            mr.sortingLayerName = "Fog";
            mr.sortingOrder = 0;

            mat = mr.material;
            mat.SetTexture("_MainTex", fogTexture);

            InvokeRepeating(nameof(UpdateFogLoop), 0f, 0.1f);
        }

        void UpdateFogLoop()
        {
            UpdateFog(UnitManager.Instance.GetAllUnits());
        }

        void ClearFog()
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(50, 50, 50, 200);
            }

            fogTexture.SetPixels32(pixels);
            fogTexture.Apply();
        }

        public void UpdateFog(List<TankMovement> units)
        {
            // Assombrir
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a < 255)
                {
                    pixels[i] = new Color32(50, 50, 50, 200);
                }
            }

            foreach (var unit in units)
            {
                var tank = unit.GetUnit() as Tank;
            
                if (tank.team != GameManager.Instance.playerTeam)
                    continue;

                // Eclaircir
                Reveal(unit.transform.position, tank.stats.visionRange);
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
            var cell = worldMap.WorldToCell(worldPos);
            int x = cell.x + width / 2;
            int y = cell.y + height / 2;
            return new Vector2Int(x, y);
        }

        public bool IsVisible(Vector3 worldPos)
        {
            Vector2Int p = WorldToGrid(worldPos);
            if (p.x < 0 || p.y < 0 || p.x >= width || p.y >= height)
                return false;

            int index = p.y * width + p.x;

            return pixels[index].a == 0;
        }

        public bool IsExplored(Vector3 worldPos)
        {
            var p = WorldToGrid(worldPos);
            int index = p.y * width + p.x;
            return pixels[index].a != 255;
        }
    }
}

