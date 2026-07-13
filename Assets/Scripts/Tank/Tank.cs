using System.Collections;
using UnityEngine;
using WarOfTanks.Cosmetics;
using WarOfTanks.UI;

namespace WarOfTanks
{
    public class Tank : Unit
    {
        private Transform bodyTransform, headTransform;
        [Header("Équipe")]
        public int teamId = 0;

        [Header("Respawn")]
        public float respawnDelay = 3f;
        public Transform respawnPoint;

        [Header("Sélection")]
        public GameObject selectionIndicator;

        private Vector3 startPosition;
        private Quaternion startRotation;


        protected override void Awake()
        {
            base.Awake(); // initialise currentHealth
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        public void SetSelected(bool selected)
        {
            if (selectionIndicator != null)
                selectionIndicator.SetActive(selected);
        }

        protected override void OnDeath()
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(RespawnRoutine());
        }

        IEnumerator RespawnRoutine()
        {
            // Cache les sprites
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                sr.enabled = false;

            var movement = GetComponent<TankMovement>();
            var shooting = GetComponent<TankShooting>();
            if (movement) movement.enabled = false;
            if (shooting) shooting.enabled = false;

            yield return new WaitForSeconds(respawnDelay);

            // Repositionne
            transform.position = respawnPoint != null
                ? respawnPoint.position
                : startPosition;
            transform.rotation = startRotation;
            currentHealth = stats.maxHealth;

            // Réaffiche
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
                sr.enabled = true;

            if (movement) movement.enabled = true;
            if (shooting) shooting.enabled = true;
        }


        public void SetSkin(TankSkin skin, Color color)
        {
            var sh = GetComponent<SpriteHandler>();
            sh.skin = skin;
            sh.color = color;
        }


        private string[] codenames =
        {
            "Alpha",
            "Bravo",
            "Charlie",
            "Delta",
            "Echo",
            "Foxtrot",
            "Golf",
            "Hotel",
            "India",
            "Juliet",
            "Kilo",
            "Lima",
            "Mike",
            "November",
            "Oscar",
            "Papa",
            "Quebec",
            "Romeo",
            "Sierra",
            "Tango",
            "Uniform",
            "Victor",
            "Whiskey",
            "X-ray",
            "Yankee",
            "Zulu"
        };

        public string GetRandomCodename()
        {
            name = codenames[Random.Range(0, codenames.Length)];
            return name;
        }

        public void HideHealthbar()
        {
            var healthbar = GetComponentInChildren<HealthBar>();
            if (healthbar != null)
                healthbar.gameObject.SetActive(false);
        }

        public void ShowHealthbar()
        {
            var healthbar = GetComponentInChildren<HealthBar>();
            if (healthbar != null)
                healthbar.gameObject.SetActive(true);
        }
    }
}