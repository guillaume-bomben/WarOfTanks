using UnityEngine;
using WarOfTanks.Nav;

namespace WarOfTanks
{
    public class TankAI : MonoBehaviour
    {
        [Header("AI")]
        public float detectionRange = 25f;
        public float updateRate = 0.5f;

        [Header("Combat")]
        [Tooltip("Distance à laquelle l'AI s'arrête et tire (doit être <= attackRange du Stats_SO)")]
        public float engageDistance = 6f;
        [Tooltip("Distance en dessous de laquelle l'AI recule un peu")]
        public float tooCloseDistance = 3f;

        private Tank tank;
        private TankMovement movement;
        private TankShooting shooting;
        private GridController gridController;

        private Transform currentTarget;
        private float timer;

        private void Awake()
        {
            tank      = GetComponent<Tank>();
            movement  = GetComponent<TankMovement>();
            shooting  = GetComponent<TankShooting>();
            gridController = FindObjectOfType<GridController>();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer < updateRate) return;
            timer = 0f;

            FindTarget();

            if (currentTarget == null)
            {
                // Pas de cible : on ne bouge pas
                return;
            }

            float dist = Vector2.Distance(transform.position, currentTarget.position);

            if (dist <= tooCloseDistance)
            {
                // Trop proche : reculer (on demande un point derrière soi)
                movement.Stop();
                // Optionnel : on pourrait demander un FlowField vers un point en retrait
            }
            else if (dist <= engageDistance)
            {
                // À portée : s'arrêter et tirer
                movement.Stop();
            }
            else
            {
                // Hors portée : se déplacer vers la cible
                MoveToTarget();
            }
        }

        private void FindTarget()
        {
            Tank[] allTanks = FindObjectsOfType<Tank>();
            float closestDistance = Mathf.Infinity;
            Transform bestTarget = null;

            foreach (Tank otherTank in allTanks)
            {
                if (otherTank == tank) continue;
                if (otherTank.TeamId == tank.TeamId) continue;

                float distance = Vector2.Distance(transform.position, otherTank.transform.position);

                if (distance < closestDistance && distance <= detectionRange)
                {
                    closestDistance = distance;
                    bestTarget = otherTank.transform;
                }
            }

            currentTarget = bestTarget;
        }

        private void MoveToTarget()
        {
            if (gridController == null) return;

            // On vise un point légèrement en retrait de la cible (à engageDistance)
            Vector2 dirToTarget = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
            Vector2 stoppingPoint = (Vector2)currentTarget.position - dirToTarget * (engageDistance * 0.8f);

            FlowField flowField = gridController.RequestFlowField(stoppingPoint);
            if (flowField != null)
                movement.MoveWithFlowField(flowField, stoppingPoint);
        }
    }
}