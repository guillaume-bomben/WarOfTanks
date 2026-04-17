using UnityEngine;

namespace WarOfTanks
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 10f;
        public float lifetime = 3f;
        public float explosionRadius = 1.5f;
        public float maxDamage = 50f;

        private Rigidbody2D rb;
        private int shooterTeamId = -1;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.velocity = transform.up * speed;
            Destroy(gameObject, lifetime);
        }

        public void SetShooterTeam(int teamId)
        {
            shooterTeamId = teamId;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Tank tank = other.GetComponentInParent<Tank>();

            // Ignore si même équipe que le tireur
            if (tank != null && tank.teamId == shooterTeamId) return;

            Explode();
        }

        void Explode()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, explosionRadius
            );

            foreach (Collider2D hit in hits)
            {
                Tank tank = hit.GetComponentInParent<Tank>();
                if (tank == null) continue;
                if (tank.teamId == shooterTeamId) continue; // ignore alliés

                float distance = Vector2.Distance(
                    transform.position, hit.transform.position
                );
                float falloff = 1f - Mathf.Clamp01(distance / explosionRadius);
                tank.TakeDamage(maxDamage * falloff);
            }

            Destroy(gameObject);
        }
    }
}