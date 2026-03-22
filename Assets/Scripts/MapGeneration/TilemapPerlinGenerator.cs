using UnityEngine;
using UnityEngine.Tilemaps;

namespace  WarOfTanks.MapGen
{
    // [ExecuteAlways]
    public class TilemapPerlinGenerator : MonoBehaviour
    {
        [Header("Noise Settings")]
        [Range(1, 8)]   public int octaves = 4;
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
        [Tooltip("Visual tilemap")] 
        public Tilemap groundTilemap;
        [Tooltip("Units can \"walk\" on these tiles")] 
        public Tilemap walkableTilemap;
        [Tooltip("Units can \"walk\" on these tiles but their stats will be impacted")] 
        public Tilemap hazardTilemap;
        [Tooltip("Units can't \"walk\" on these tiles")] 
        public Tilemap unwalkableTilemap;

        [Header("Terrains (ordered by height)")]
        public TerrainType[] terrains;

        [Header("Slope tiles")]
        public TileBase verticalSlope;
        public TileBase horizontalSlope;

        float[,] heightMap;
        TerrainDataModifier[,] modifierMap;


        // Génération de la map
        public void GenerateMap()
        {
            Random.InitState(seed);
            Vector2 seedOffset = new Vector2(Random.value * 10_000f, Random.value * 10_000f);

            if (walkableTilemap == null || terrains.Length == 0)
                return;

            groundTilemap.ClearAllTiles();
            walkableTilemap.ClearAllTiles();
            unwalkableTilemap.ClearAllTiles();
            hazardTilemap.ClearAllTiles();

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

                    heightMap[x, y] = noise;
                }
            }

            // Placement des tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int pos = new Vector3Int(x - offsetX, y - offsetY, 0);
                    int currentLevel = GetLevel(heightMap[x, y]);
                    TerrainType terrain = terrains[currentLevel];
                    modifierMap[x, y] = terrain.modifier;

                    groundTilemap.SetTile(pos, terrain.tile);

                    if (terrain.isWalkable)
                        walkableTilemap.SetTile(pos, terrain.tile);
                    else
                        unwalkableTilemap.SetTile(pos, terrain.tile);

                    if (terrain.isHazard)
                        hazardTilemap.SetTile(pos, terrain.tile);

                    if (TryGetSlope(x, y, currentLevel, out TileBase slopeTile))
                    {
                        groundTilemap.SetTile(pos, slopeTile);
                        hazardTilemap.SetTile(pos, slopeTile);
                    }
                }
            }
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

                noiseHeight =+ perlin * amp;
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

        bool TryGetSlope(int x, int y, int currentLevel, out TileBase slopeTile)
        {
            slopeTile = null;

            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach (var dir in directions)
            {
                int nx = x + dir.x;
                int ny = y + dir.y;

                if (!InBounds(nx, ny))
                    continue;

                int neighborLevel = GetLevel(heightMap[nx, ny]);

                // pente uniquement si différence de 1 niveau
                if (Mathf.Abs(neighborLevel - currentLevel) == 1 && (Mathf.Abs(heightMap[nx, ny] - heightMap[x, y]) > 0.1f))
                {
                    slopeTile = GetSlopeTile(dir);
                    return true;
                }
            }

            return false;
        }

        bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        public TerrainDataModifier GetModifierAtWorldPos(Vector3 worldPos)
        {
            Vector3Int cell = groundTilemap.WorldToCell(worldPos);
            int x = cell.x + width / 2;
            int y = cell.y + height / 2;

            if (x < 0 || y < 0 || x >= width || y >= height)
                return null;
            
            return modifierMap[x, y];
        }

        // récupère une tile "pente" en fonction de la direction
        TileBase GetSlopeTile(Vector2Int dir)
        {
            if (dir == Vector2Int.up || dir == Vector2Int.down)
                return verticalSlope;

            if (dir == Vector2Int.left || dir == Vector2Int.right)
                return horizontalSlope;
        
            return verticalSlope;
        }

        float AppyFalloff(int x, int y)
        {
            float nx = Mathf.Abs((float)x / width * 2f - 1f);
            float ny = Mathf.Abs((float)y / width * 2f - 1f);
            float dist = Mathf.Max(nx, ny);
            return falloffCurve.Evaluate(dist);
        }
    }
}

