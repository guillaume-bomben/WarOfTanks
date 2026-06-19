using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfTanks
{
    public class Spawn : MonoBehaviour
    {
        public Team team;
        public int teamSize;
        public List<TankMovement> units = new List<TankMovement>();

        public float respawnTime;

        private SpriteRenderer sr;


        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            SetupSpawner();

            // First spawn sequence
            foreach (var tank in units)
            {
                var unit = tank.GetUnit();
                StartCoroutine(SpawnCoroutine(unit));
            }
        }


        IEnumerator SpawnCoroutine(Unit unit)
        {


            yield return null;
        }

        void SetupSpawner()
        {
            switch (team)
            {
                case Team.Red:
                    var tex = Resources.Load<Texture2D>("Sprites/base_red");
                    sr.sprite = Sprite.Create(
                        tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f),
                        10f
                    );
                    break;

                case Team.Blue:
                    var tex2 = Resources.Load<Texture2D>("Sprites/base_blue");
                    sr.sprite = Sprite.Create(
                        tex2,
                        new Rect(0, 0, tex2.width, tex2.height),
                        new Vector2(0.5f, 0.5f),
                        10f
                    );
                    break;

                default:
                    break;
            }
        }
    }
}