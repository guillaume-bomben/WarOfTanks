using UnityEngine;

namespace WarOfTanks
{
    public class TankMovement : MonoBehaviour
    {
        private Tank tank;
        private Rigidbody2D rb;
        private Vector2 targetPosition;
        private bool isMoving = false;

        void Start()
        {
            tank = GetComponent<Tank>();
            rb = GetComponent<Rigidbody2D>();
            targetPosition = transform.position;
        }

        void FixedUpdate()
        {
            if (!isMoving)
            {
                rb.velocity = Vector2.zero;
                return;
            }

            Vector2 toTarget = targetPosition - (Vector2)transform.position;

            if (toTarget.magnitude < 0.2f)
            {
                rb.velocity = Vector2.zero;
                isMoving = false;
                return;
            }

            Vector2 direction = toTarget.normalized;

            // Rotation du corps vers la destination
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0f, 0f, targetAngle),
                180f * Time.fixedDeltaTime
            );

            // Avance toujours vers la cible
            rb.velocity = direction * tank.stats.moveSpeed;
        }

        public void MoveTo(Vector2 destination)
        {
            targetPosition = destination;
            isMoving = true;
        }

        public void Stop()
        {
            isMoving = false;
            rb.velocity = Vector2.zero;
        }
    }
}