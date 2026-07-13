using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarOfTanks.Cosmetics;
using WarOfTanks.Stats;

namespace WarOfTanks
{
    public class Spawn : MonoBehaviour
    {
        [Header("Team")]
        public Team team;
        public int teamSize = 3;

        [Header("Prefab")]
        public GameObject tankPrefab;

        [Header("Spawn Settings")]
        public float respawnTime = 1f;
        public float startDelay = 5f;
        public float spawnRadius = 1.5f;

        private SpriteRenderer sr;
        private List<Vector3> usedPositions = new List<Vector3>();

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            SetupSpawner();

            StartCoroutine(StartSpawnSequence());
        }

        IEnumerator StartSpawnSequence()
        {
            float countdown = startDelay;

            while (countdown > 0)
            {
                yield return new WaitForSeconds(1f);
                countdown--;
            }

            // Spawn progressif
            for (int i = 0; i < teamSize; i++)
            {
                SpawnUnit();
                //yield return new WaitForSeconds(respawnTime);
            }

            yield return null;
        }

        void SpawnUnit()
        {
            Vector3 spawnPos = GetSpawnPosition();

            GameObject go = Instantiate(tankPrefab, spawnPos, Quaternion.identity);

            Tank tank = go.GetComponent<Tank>();
            TankMovement movement = go.GetComponent<TankMovement>();

            if (tank != null)
            {
                SetupTank(tank);
            }

            if (movement != null)
            {
                UnitManager.Instance.Register(movement);
            }
        }

        void SetupTank(Tank tank)
        {
            tank.team = team;
            tank.stats = Resources.Load<Stats_SO>("Stats/BaseTankStats");
            tank.name = $"Tank T-{Random.Range(10, 99)} \"{tank.GetRandomCodename()}\"";

            tank.transform.localScale = new Vector3(0.5f, 0.5f, 1);

            if (team == Team.Red)
            {
                tank.SetSkin(TankSkin.Woodland, Color.red);
            }
            else
            {
                tank.SetSkin(TankSkin.Tiger, Color.blue);
            }
        }

        Vector3 GetSpawnPosition()
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
                Vector3 candidate = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

                bool tooClose = false;

                foreach (var pos in usedPositions)
                {
                    if (Vector3.Distance(candidate, pos) < 1f)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    usedPositions.Add(candidate);
                    return candidate;
                }
            }

            return transform.position;
        }

        void SetupSpawner()
        {
            switch (team)
            {
                case Team.Red:
                    sr.sprite = LoadSprite("Sprites/base_red");
                    break;

                case Team.Blue:
                    sr.sprite = LoadSprite("Sprites/base_blue");
                    break;
            }
        }

        Sprite LoadSprite(string path)
        {
            Texture2D tex = Resources.Load<Texture2D>(path);

            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                10f
            );
        }
    }
}