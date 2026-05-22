using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace  WarOfTanks.MapGen
{
    public class TilemapPerlinGenerator : MonoBehaviour
    {
        public TileInfoPopup popup;

        public bool autoUpdate = false;

        [Header("Noise Settings")]
        [Range(1, 8)]   public int octaves = 2;
        [Range(1f, 4f)] public float lacunarity = 2f;
        [Range(0f, 1f)] public float persistance = 0.5f;
                        public int seed;

        public AnimationCurve falloffCurve = AnimationCurve.Linear(0, 1, 1, 0);

        [Header("Size & scale")]
        public int width    = 128;
        public int height   = 128;
        public float scale  = 30f;

        [Header("Offset")]
        public Vector2 offset = new Vector2(100f, 100f);

        [Header("Tilemap")]
        public Tilemap worldMap;
        public Tilemap waterMap;
        public Tilemap sandMap;
        public Tilemap grassMap;
        public Tilemap unwalkableMap;
        public Tilemap hazardMap;

        [Header("Terrains (ordered by height)")]
        public TerrainData[] terrains;

        float[,] heightMap;
        TerrainDataModifier[,] modifierMap;


        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                HandleRightClick();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                popup.Hide();
            }
        }

        void HandleRightClick()
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = worldMap.WorldToCell(mouseWorld);

            TileBase tile = worldMap.GetTile(cellPos);

            if (tile == null) return;

            TerrainRuleTile terrainTile = tile as TerrainRuleTile;

            if (terrainTile != null)
            {
                ShowPopup(terrainTile, cellPos);
            }
        }

        void ShowPopup(TerrainRuleTile tile, Vector3Int pos)
        {
            string info = "";
            info += $"Terrain: {tile.name.Replace("RuleTile", "")}\n";
            info += $"Walkable: {tile.isWalkable}\n";
            info += $"Hazard: {tile.isHazard}\n";

            if (tile.modifier != null)
            {
                info += "Modifier(s):\n";
                foreach (var mod in tile.modifier.modifiers)
                {
                    switch (mod.modifier)
                    {
                        case Stats.StatsModifier.Flat:
                            info += $"• {mod.statType} {mod.value:+0;-0}\n";
                            break;

                        case Stats.StatsModifier.Percent:
                            info += $"• {mod.statType} {mod.value:+0;-0}%\n";
                            break;

                        case Stats.StatsModifier.Override:
                            info += $"• {mod.statType} = {mod.value:-0;0}\n";
                            break;
                    }
                }
            }

            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldMap.GetCellCenterWorld(pos));
            popup.Show(info, screenPos);
        }

        // Génération de la map
        public void GenerateMap()
        {
            Random.InitState(seed);
            Vector2 seedOffset = new Vector2(Random.value * 10_000f, Random.value * 10_000f);

            if (sandMap == null || terrains.Length == 0)
                return;

            waterMap.ClearAllTiles();
            sandMap.ClearAllTiles();
            unwalkableMap.ClearAllTiles();
            grassMap.ClearAllTiles();
            hazardMap.ClearAllTiles();

            int offsetX = width / 2;
            int offsetY = height / 2;

            heightMap = new float[width, height];
            modifierMap = new TerrainDataModifier[width, height];

            
            // Génération de la heightmap
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float xCoord = (x / scale + offset.x + seedOffset.x) / scale;
                    float yCoord = (y / scale + offset.y + seedOffset.y) / scale;

                    float noise = GenerateNoise(xCoord, yCoord);
                    float falloff = AppyFalloff(x, y);
                    noise = Mathf.Clamp01(noise - falloff);

                    heightMap[x, y] = noise + 0.1f;
                }
            }
    
            // Placement des tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int pos = new Vector3Int(x - offsetX, y - offsetY, 0);
                    int currentLevel = GetLevel(heightMap[x, y]);
                    TerrainData terrain = terrains[currentLevel];
                    modifierMap[x, y] = terrain.ruleTile.modifier;
                    terrain.SetWorldPos(new Vector3(x, y));

                    switch (terrain.ruleTile.terrainKind)
                    {
                        case TerrainRuleTile.TerrainKind.Water:
                            waterMap.SetTile(pos, terrain.ruleTile);
                            break;
                        
                        case TerrainRuleTile.TerrainKind.Sand:
                            sandMap.SetTile(pos, terrain.ruleTile);
                            break;

                        case TerrainRuleTile.TerrainKind.Grass:
                            grassMap.SetTile(pos, terrain.ruleTile);
                            break;

                        case TerrainRuleTile.TerrainKind.Rock:
                            unwalkableMap.SetTile(pos, terrain.ruleTile);
                            break;
                    }

                    if (terrain.ruleTile.isHazard)
                        hazardMap.SetTile(pos, terrain.ruleTile);
                    
                    worldMap.SetTile(pos, terrain.ruleTile);
                }
            }

            waterMap.RefreshAllTiles();
            sandMap.RefreshAllTiles();
            unwalkableMap.RefreshAllTiles();
            grassMap.RefreshAllTiles();

            worldMap.RefreshAllTiles();
        }

        float GenerateNoise(float x, float y)
        {
            float amp = 1f;
            float freq = 1f;
            float noiseHeight = 0f;

            float maxValue = 0f;

            for (int i = 0; i < octaves; i++)
            {
                float sampleX = x * freq;
                float sampleY = y * freq;

                float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;

                noiseHeight += perlin * amp;
                maxValue += amp;
                amp *= persistance;
                freq *= lacunarity;
            }

            noiseHeight /= maxValue;
            noiseHeight = Mathf.InverseLerp(-1f, 1f, noiseHeight);
            return Mathf.Clamp01(noiseHeight);
        }

        int GetLevel(float height)
        {
            for (int i = 0; i < terrains.Length; i++)
            {
                if (height <= terrains[i].height)
                    return i;
            }

            return terrains.Length - 1;
        }

        bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        public TerrainDataModifier GetModifierAtWorldPos(Vector3 worldPos)
        {
            Vector3Int cell = waterMap.WorldToCell(worldPos);
            int x = cell.x + width / 2;
            int y = cell.y + height / 2;

            if (x < 0 || y < 0 || x >= width || y >= height)
                return null;
            
            return modifierMap[x, y];
        }

        public float AppyFalloff(int x, int y)
        {
            float nx = Mathf.Abs((float)x / width * 2f - 1f);
            float ny = Mathf.Abs((float)y / width * 2f - 1f);
            float dist = Mathf.Max(nx, ny);
            return falloffCurve.Evaluate(dist);
        }

        public float[,] GetHeightMap() => heightMap;
    }
}

