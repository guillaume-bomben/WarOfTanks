using System.Collections.Generic;
using UnityEngine;
using WarOfTanks.Nav;

namespace WarOfTanks
{
    /// <summary>
    /// Sélection RTS + ordres de déplacement/attaque.
    /// Sur clic droit sol → construit un Flow Field partagé et l'envoie
    /// à tous les tanks sélectionnés.
    /// </summary>
    public class TankSelector : MonoBehaviour
    {
        [Header("Références")]
        public GridController gridController; // assigné dans l'Inspector

        [Header("Sélection")]
        public float dragThreshold = 0.2f;

        private Camera mainCam;
        private List<Tank> selectedTanks = new List<Tank>();
        private Vector2 dragStart;
        private bool isDragging = false;

        void Start()
        {
            mainCam = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) OnLeftClickDown();
            if (Input.GetMouseButton(0))     OnLeftClickHeld();
            if (Input.GetMouseButtonUp(0))   OnLeftClickUp();
            if (Input.GetMouseButtonDown(1)) OnRightClick();
            if (Input.GetKeyDown(KeyCode.S)) StopAll();
        }

        // ── Clic gauche ──────────────────────────────────────────────────

        void OnLeftClickDown()
        {
            dragStart  = MouseWorldPos();
            isDragging = false;
        }

        void OnLeftClickHeld()
        {
            if (Vector2.Distance(MouseWorldPos(), dragStart) > dragThreshold)
                isDragging = true;
        }

        void OnLeftClickUp()
        {
            if (isDragging)
                SelectInRect();
            else
                SelectSingle();

            isDragging = false;
        }

        void SelectSingle()
        {
            Tank tank = RaycastTank();
            if (tank == null) return;

            if (Input.GetKey(KeyCode.LeftShift))
                AddToSelection(tank);
            else
                Select(tank);
        }

        void SelectInRect()
        {
            Vector2 end = MouseWorldPos();
            Vector2 min = Vector2.Min(dragStart, end);
            Vector2 max = Vector2.Max(dragStart, end);

            DeselectAll();

            foreach (Tank tank in FindObjectsOfType<Tank>())
            {
                Vector2 pos = tank.transform.position;
                if (pos.x >= min.x && pos.x <= max.x &&
                    pos.y >= min.y && pos.y <= max.y)
                    AddToSelection(tank);
            }
        }

        // ── Clic droit ───────────────────────────────────────────────────

        void OnRightClick()
        {
            if (selectedTanks.Count == 0) return;

            Vector2 mousePos = MouseWorldPos();

            if (Input.GetKey(KeyCode.A))
            {
                // A + clic droit → attaquer une zone (stub, sera implémenté avec A-04)
                AttackZone(mousePos);
                return;
            }

            Tank enemy = RaycastTank();

            if (enemy != null && !IsInSelection(enemy))
            {
                // Clic sur un ennemi → ordre d'attaque (stub)
                AttackTarget(enemy);
            }
            else
            {
                // Clic sur le sol → déplacement via Flow Field
                MoveAllWithFlowField(mousePos);
            }
        }

        // ── Commandes de déplacement ─────────────────────────────────────

        /// <summary>
        /// Construit UN seul Flow Field partagé vers la destination,
        /// puis le distribue à tous les tanks sélectionnés.
        /// Chaque tank suit le champ depuis SA position → chemins distincts.
        /// La séparation (dans TankMovement) évite les bousculades.
        /// </summary>
        void MoveAllWithFlowField(Vector2 destination)
        {
            if (gridController == null)
            {
                Debug.LogError("TankSelector : GridController non assigné !");
                return;
            }

            // Un seul calcul pour tous les tanks
            FlowField field = gridController.RequestFlowField(destination);

            foreach (Tank tank in selectedTanks)
            {
                TankMovement movement = tank.GetComponent<TankMovement>();
                if (movement != null)
                    movement.MoveWithFlowField(field, destination);
            }
        }

        void AttackTarget(Tank enemy)
        {
            // TODO A-04 : ordre "Attaquer" — le tank se positionne à portée puis tire
            foreach (Tank tank in selectedTanks)
                tank.GetComponent<TankShooting>()?.Shoot();
        }

        void AttackZone(Vector2 point)
        {
            // TODO A-04 : ordre "Attaquer une zone"
            foreach (Tank tank in selectedTanks)
                tank.GetComponent<TankShooting>()?.Shoot();
        }

        void StopAll()
        {
            foreach (Tank tank in selectedTanks)
                tank.GetComponent<TankMovement>()?.Stop();
        }

        // ── Sélection ────────────────────────────────────────────────────

        void Select(Tank tank)
        {
            DeselectAll();
            AddToSelection(tank);
        }

        void AddToSelection(Tank tank)
        {
            if (selectedTanks.Contains(tank)) return;
            selectedTanks.Add(tank);
            tank.SetSelected(true);
        }

        void DeselectAll()
        {
            foreach (Tank tank in selectedTanks)
                tank.SetSelected(false);
            selectedTanks.Clear();
        }

        bool IsInSelection(Tank tank) => selectedTanks.Contains(tank);

        // ── Utilitaires ──────────────────────────────────────────────────

        Tank RaycastTank()
        {
            RaycastHit2D hit = Physics2D.Raycast(MouseWorldPos(), Vector2.zero);
            return hit.collider?.GetComponentInParent<Tank>();
        }

        Vector2 MouseWorldPos()
        {
            Vector3 pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(pos.x, pos.y);
        }
    }
}