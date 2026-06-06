using System.Collections;
using UnityEngine;

namespace WarOfTanks
{
    public class Tank : Unit
    {
        [Header("Équipe")]
        public int teamId = 0;
        public int TeamId => teamId;

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
    }
}