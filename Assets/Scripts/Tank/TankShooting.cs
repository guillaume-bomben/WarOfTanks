using UnityEngine;

namespace WarOfTanks
{
    public class TankShooting : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform firePoint;

        private Transform headTransform;
        private float fireCooldown = 0f;
        private float fireRate;
        private Transform currentTarget;
        private bool canShoot = false; // bloque le tir au démarrage

        void Start()
        {
            headTransform = transform.Find("Head");
            fireRate = GetComponent<Tank>().stats.fireRate;

            // Attend 1 seconde avant de permettre le tir
            Invoke(nameof(EnableShooting), 1f);
        }

        void EnableShooting() => canShoot = true;

        void Update()
        {
            if (!canShoot) return;
            DetectEnemy();
            AimAtTarget();

            if (fireCooldown > 0f)
                fireCooldown -= Time.deltaTime;
        }

        void DetectEnemy()
        {
            float visionRange = GetComponent<Tank>().stats.visionRange;
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, visionRange
            );

            currentTarget = null;
            float closestDist = Mathf.Infinity;

            foreach (Collider2D hit in hits)
            {
                Tank enemy = hit.GetComponentInParent<Tank>();
                if (enemy == null || enemy == GetComponent<Tank>()) continue;
                if (enemy.teamId == GetComponent<Tank>().teamId) continue;

                float dist = Vector2.Distance(
                    transform.position, enemy.transform.position
                );
                if (dist < closestDist)
                {
                    closestDist = dist;
                    currentTarget = enemy.transform;
                }
            }
        }

        void AimAtTarget()
        {
            if (currentTarget == null || headTransform == null) return;

            Vector2 dir = (currentTarget.position - headTransform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            headTransform.rotation = Quaternion.Lerp(
                headTransform.rotation,
                Quaternion.Euler(0f, 0f, angle),
                Time.deltaTime * 5f
            );

            if (fireCooldown <= 0f) Shoot();
        }

        public void Shoot()
        {
            if (!canShoot) return;
            if (bulletPrefab == null || firePoint == null) return;
            if (fireCooldown > 0f) return;

            GameObject b = Instantiate(
                bulletPrefab, firePoint.position, headTransform.rotation
            );

            // Passe le teamId au bullet pour éviter le friendly fire
            Bullet bullet = b.GetComponent<Bullet>();
            if (bullet != null)
                bullet.SetShooterTeam(GetComponent<Tank>().teamId);

            fireCooldown = 1f / fireRate;
        }
    }
}