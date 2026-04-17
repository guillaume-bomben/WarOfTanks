using System.Collections.Generic;
using UnityEngine;

namespace WarOfTanks
{
    public class TankSelector : MonoBehaviour
    {
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
            dragStart = MouseWorldPos();
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
                AddToSelection(tank);   // Maj → ajouter
            else
                Select(tank);
        }

        void SelectInRect()
        {
            // Rectangle entre dragStart et position actuelle
            Vector2 end = MouseWorldPos();
            Vector2 min = Vector2.Min(dragStart, end);
            Vector2 max = Vector2.Max(dragStart, end);

            DeselectAll();

            // Cherche tous les tanks dans le rectangle
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

            if (Input.GetKey(KeyCode.A))
            {
                // A + clic droit → attaquer une zone
                AttackZone(MouseWorldPos());
            }
            else
            {
                Tank enemy = RaycastTank();

                if (enemy != null && !selectedTanks.Contains(enemy))
                    AttackTarget(enemy);    // Ennemi ciblé → attaquer
                else
                    MoveAll(MouseWorldPos());
            }
        }

        // ── Commandes ────────────────────────────────────────────────────

        void MoveAll(Vector2 destination)
        {
            foreach (Tank tank in selectedTanks)
                tank.GetComponent<TankMovement>().MoveTo(destination);
        }

        void AttackTarget(Tank enemy)
        {
            foreach (Tank tank in selectedTanks)
                tank.GetComponent<TankShooting>().Shoot();
        }

        void AttackZone(Vector2 point)
        {
            foreach (Tank tank in selectedTanks)
                tank.GetComponent<TankShooting>().Shoot();
        }

        void StopAll()
        {
            foreach (Tank tank in selectedTanks)
                tank.GetComponent<TankMovement>().Stop();
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

        // ── Utilitaires ──────────────────────────────────────────────────

        Tank RaycastTank()
        {
            RaycastHit2D hit = Physics2D.Raycast(MouseWorldPos(), Vector2.zero);
            if (hit.collider != null)
                return hit.collider.GetComponentInParent<Tank>();
            return null;
        }

        Vector2 MouseWorldPos()
        {
            Vector3 pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(pos.x, pos.y);
        }
    }
}