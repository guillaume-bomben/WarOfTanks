using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using WarOfTanks.Cosmetics.FogOfWar;

namespace WarOfTanks.MapGen
{
    [RequireComponent(typeof(TilemapPerlinGenerator))]
    public class HazardPerlinGenerator : MonoBehaviour
    {
        [Header("References")]
        public TilemapPerlinGenerator mapGen;

        [Tooltip("Tilemap où seront placés les hazards")]
        public Tilemap hazardMap;

        [Tooltip("Tiles de hazard possibles (swamp, poison, etc.)")]
        public HazardLayer[] hazardLayers;

        [Header("Hazard Noise Settings")]
        public float hazardScale = 15f;
        [Range(0f, 1f)] public float hazardThreshold = 0.6f;
        public Vector2 hazardOffset;

        [Header("Placement Rules")]
        public TerrainRuleTile.TerrainKind[] validKinds;
        public bool avoidEdges = true;
        public float edgeThreshold = 0.4f;

        float[,] heightMap;

        void Awake()
        {
            if (mapGen == null)
                mapGen = GetComponent<TilemapPerlinGenerator>();
        }

        [ContextMenu("Generate Hazards")]
        public void GenerateHazards()
        {
            if (mapGen == null || hazardMap == null)
            {
                Debug.LogWarning("Missing references");
                return;
            }

            hazardMap.ClearAllTiles();

            int width = mapGen.width;
            int height = mapGen.height;

            int offsetX = width / 2;
            int offsetY = height / 2;

            // Regénérer la map, sinon la hazard map ne se génère pas
            mapGen.GenerateMap();
            // Récupère la heightMap depuis le générateur principal
            heightMap = mapGen.GetHeightMap();

            if (heightMap == null)
            {
                Debug.LogError("HeightMap not found. Generate terrain first.");
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int pos = new Vector3Int(x - offsetX, y - offsetY, 0);

                    float hazardNoise = Mathf.PerlinNoise(
                        (x + hazardOffset.x) / hazardScale,
                        (y + hazardOffset.y) / hazardScale
                    );

                    if (hazardNoise < hazardThreshold)
                        continue;

                    if (avoidEdges)
                    {
                        float falloff = mapGen.AppyFalloff(x, y);
                        if (falloff > edgeThreshold)
                            continue;
                    }

                    TerrainRuleTile baseTile = mapGen.worldMap.GetTile(pos) as TerrainRuleTile;

                    if (baseTile == null)
                        continue;

                    if (!validKinds.Contains(baseTile.terrainKind))
                        continue;

                    if (!IsFlat(x, y))
                        continue;

                    foreach (var layer in hazardLayers)
                    {
                        float noise = Mathf.PerlinNoise(
                            (x + layer.offset.x) / layer.scale,
                            (y + layer.offset.y) / layer.scale
                        );

                        if (noise > layer.threshold)
                        {
                            if (!FogManager.Instance.IsExplored(pos))
                            {
                                HideHazardTile(pos);
                            }
                            else
                            {
                                PlaceHazardTile(pos, layer.ruleTile);
                            }
                            break;
                        }
                    }
                }
            }

            hazardMap.RefreshAllTiles();
        }

        void PlaceHazardTile(Vector3Int pos, TerrainRuleTile tile)
        {
            mapGen.worldMap.SetTile(pos, tile);
            hazardMap.SetTile(pos, tile);
        }

        void HideHazardTile(Vector3Int pos)
        {
            mapGen.worldMap.SetTile(pos, null);
            hazardMap.SetTile(pos, null);
        }

        bool IsFlat(int x, int y)
        {
            int width = mapGen.width;
            int height = mapGen.height;

            float h = heightMap[x, y];

            if (x + 1 < width && Mathf.Abs(h - heightMap[x + 1, y]) > 0.05f) return false;
            if (y + 1 < height && Mathf.Abs(h - heightMap[x, y + 1]) > 0.05f) return false;

            return true;
        }
    }
}