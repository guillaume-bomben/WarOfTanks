using UnityEngine;
using WarOfTanks.Nav;

namespace WarOfTanks
{
    /// <summary>
    /// Déplacement des tanks via Flow Field partagé.
    /// 
    /// ALGORITHME :
    ///   1. Flow Field  → chaque cellule de la grille pointe vers la destination.
    ///      Tous les tanks lisent le même champ (O(1) par tank, pas de recalcul A*).
    ///      Résultat : chaque tank suit le vecteur de SA cellule courante,
    ///      donc deux tanks partant de positions différentes empruntent
    ///      naturellement des chemins légèrement différents.
    ///
    ///   2. Separation Steering → force de répulsion entre tanks proches.
    ///      Chaque tank repousse ses voisins perpendiculairement, ce qui
    ///      les fait s'étaler et évite les bousculades / superpositions.
    ///      Léger (seuls les N voisins dans un rayon sont testés).
    /// </summary>
    public class TankMovement : MonoBehaviour
    {
        // ── Référence au FlowField partagé ───────────────────────────────
        /// Assigné par GridController quand un clic droit déclenche le mouvement.
        [HideInInspector] public FlowField currentFlowField;

        // ── Paramètres séparation ────────────────────────────────────────
        [Header("Séparation entre tanks")]
        [Tooltip("Rayon de détection des voisins (unités Unity)")]
        public float separationRadius = 1.2f;
        [Tooltip("Force de répulsion")]
        public float separationWeight = 2.5f;

        // ── Arrivée ──────────────────────────────────────────────────────
        [Header("Arrivée")]
        [Tooltip("Distance sous laquelle le tank est considéré arrivé")]
        public float arrivalRadius = 0.4f;
        [Tooltip("Distance à partir de laquelle le tank commence à ralentir")]
        public float slowRadius = 1.5f;

        // ── État interne ─────────────────────────────────────────────────
        private Tank tank;
        private Rigidbody2D rb;
        private bool isMoving = false;
        private Vector2 destination; // position world de la cellule cible

        // ── Masque de layers pour la séparation (tanks uniquement) ───────
        [Header("Layer")]
        public LayerMask tankLayerMask;

        void Start()
        {
            tank = GetComponent<Tank>();
            rb   = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            if (!isMoving || currentFlowField == null)
            {
                rb.velocity = Vector2.zero;
                return;
            }

            // 1. Direction du Flow Field pour la cellule courante
            Cell currentCell = currentFlowField.GetCellFromWorldPos(transform.position);
            if (currentCell == null || currentCell.bestDirection == GridDirection.None)
            {
                rb.velocity = Vector2.zero;
                isMoving = false;
                return;
            }

            Vector2 flowDir = ((Vector2)(Vector2Int)currentCell.bestDirection).normalized;

            // 2. Vérif arrivée
            float distToDest = Vector2.Distance(transform.position, destination);
            if (distToDest < arrivalRadius)
            {
                rb.velocity = Vector2.zero;
                isMoving = false;
                currentFlowField = null;
                return;
            }

            // 3. Facteur de ralentissement à l'approche
            float speedFactor = Mathf.Clamp01(distToDest / slowRadius);

            // 4. Séparation : repousse les tanks trop proches
            Vector2 separation = ComputeSeparation();

            // 5. Combinaison des forces
            Vector2 finalDir = (flowDir + separation).normalized;

            // 6. Rotation du corps vers la direction
            float targetAngle = Mathf.Atan2(finalDir.y, finalDir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0f, 0f, targetAngle),
                200f * Time.fixedDeltaTime
            );

            // 7. Vélocité
            rb.velocity = finalDir * tank.stats.moveSpeed * speedFactor;
        }

        /// <summary>
        /// Calcule la force de séparation : somme des vecteurs "fuis tes voisins",
        /// pondérée par la proximité (plus le voisin est proche, plus on le fuit fort).
        /// </summary>
        Vector2 ComputeSeparation()
        {
            Vector2 force = Vector2.zero;
            int count = 0;

            Collider2D[] neighbors = Physics2D.OverlapCircleAll(
                transform.position, separationRadius, tankLayerMask
            );

            foreach (Collider2D col in neighbors)
            {
                if (col.gameObject == gameObject) continue; // ignore soi-même

                Vector2 away = (Vector2)transform.position - (Vector2)col.transform.position;
                float dist = away.magnitude;

                if (dist < 0.001f) dist = 0.001f;

                // Plus le voisin est proche, plus la force est grande
                force += away.normalized * (separationRadius - dist) / separationRadius;
                count++;
            }

            if (count == 0) return Vector2.zero;
            return (force / count) * separationWeight;
        }

        // ── API publique ─────────────────────────────────────────────────

        /// <summary>
        /// Déclenche le mouvement via Flow Field.
        /// Appelé par TankSelector après que GridController a construit le champ.
        /// </summary>
        
        public void MoveWithFlowField(FlowField flowField, Vector2 dest)
        
        {
            currentFlowField = flowField;
            destination      = dest;
            isMoving         = true;
        }

        /// <summary>Mouvement direct sans flow field (legacy / test)</summary>
        public void MoveTo(Vector2 dest)
        {
            currentFlowField = null;
            destination      = dest;
            isMoving         = true;
        }

        public void Stop()
        {
            isMoving = false;
            currentFlowField = null;
            rb.velocity = Vector2.zero;
        }

        public bool IsMoving => isMoving;
    }
}