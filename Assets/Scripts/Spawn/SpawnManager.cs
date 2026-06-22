using System.Collections.Generic;
using UnityEngine;
using WarOfTanks.Cosmetics;
using WarOfTanks.MapGen;
using WarOfTanks.Stats;

namespace WarOfTanks
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject tankPrefab;

        [Header("Spawn Settings")]
        public int unitsPerTeam = 3;

        private TilemapPerlinGenerator mapGen;
        private ZoneType[,] zones;

        public void Init()
        {
            mapGen = FindAnyObjectByType<TilemapPerlinGenerator>();
            MapZones zonesGen = new MapZones();
            zones = zonesGen.GenerateZones(mapGen.width, mapGen.height);
            SpawnTeams();
        }

        void SpawnTeams()
        {            
            SpawnTeam(ZoneType.RedSpawn, tankPrefab, Team.Red);
            SpawnTeam(ZoneType.BlueSpawn, tankPrefab, Team.Blue);
        }

        void SpawnTeam(ZoneType type, GameObject unitPrefab, Team team)
        {
            List<Vector3> usedPositions = new List<Vector3>();

            for (int i = 0; i < unitsPerTeam; i++)
            {
                Vector3 spawnPos = GetValidSpawnPosition(type, usedPositions);

                GameObject unitGO = Instantiate(unitPrefab, spawnPos, Quaternion.identity);

                Tank tank = unitGO.GetComponent<Tank>();

                if (tank != null)
                {
                    SetupTank(tank, team);
                    UnitManager.Instance.Register(tank.GetComponent<TankMovement>());
                }

                usedPositions.Add(spawnPos);
            }
        }

        void SetupTank(Tank tank, Team team)
        {
            tank.team = team;
            tank.stats = Resources.Load<Stats_SO>("Stats/BaseTankStats");
            tank.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            tank.SetSkin(
                tank.team == Team.Blue ? TankSkin.Tiger : TankSkin.Woodland, 
                tank.team == Team.Blue ? Color.blue : Color.red
            );
        }

        Vector3 GetValidSpawnPosition(ZoneType zone, List<Vector3> usedPositions)
        {
            for (int attempts = 0; attempts < 50; attempts++)
            {
                Vector3 pos = GetSpawnPoint(zone, zones);
                bool tooClose = false;

                foreach (var used in usedPositions)
                {
                    if (Vector3.Distance(pos, used) < 5f)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                    return pos;
            }

            Debug.LogWarning("Fallback spawns used");
            return GetSpawnPoint(zone, zones);
        }

        public Vector3 GetSpawnPoint(ZoneType targetZone, ZoneType[,] zones)
        {
            List<Vector3> candidates = new List<Vector3>();
            var mainIsland = mapGen.GetMainIsland();

            int offsetX = mapGen.width / 2;
            int offsetY = mapGen.height / 2;

            for (int x = 0; x < mapGen.width; x++)
            {
                for (int y = 0; y < mapGen.height; y++)
                {
                    if (zones[x, y] != targetZone)
                        continue;

                    if (!mainIsland.Contains(new Vector2Int(x, y)))
                        continue;

                    Vector3Int pos = new Vector3Int(x - offsetX, y - offsetY, 0);

                    if (!mapGen.IsValidSpawn(pos))
                        continue;

                    candidates.Add(mapGen.worldMap.GetCellCenterWorld(pos));
                }
            }

            if (candidates.Count == 0)
            {
                Debug.LogWarning("No valid spawn point found");
                throw new System.Exception("Spawn failed");
            }

            return candidates[Random.Range(0, candidates.Count)];
        }
    }
}