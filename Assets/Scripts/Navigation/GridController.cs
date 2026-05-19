using UnityEngine;
using WarOfTanks.MapGen;

namespace WarOfTanks.Nav
{

    public class GridController : MonoBehaviour
    {
        [Header("Flow Field")]
        public float cellRadius = 0.5f;
        public TilemapPerlinGenerator map;

        [Header("Debug")]
        public GridDebug gridDebug;

        public FlowField CurrentFlowField { get; private set; }

        /// <summary>Exposé pour GridDebug (compatibilité)</summary>
        public Vector2Int gridSize => CurrentFlowField != null
            ? CurrentFlowField.gridSize
            : (map != null ? new Vector2Int(map.width, map.height) : Vector2Int.zero);

        // ── API publique ─────────────────────────────────────────────────

        public FlowField RequestFlowField(Vector3 worldDestination)
        {
            // Reconstruit la grille à chaque demande
            // (on peut optimiser en ne reconstruisant le cost field qu'une fois si la map ne change pas)
            CurrentFlowField = new FlowField(cellRadius, Vector2Int.zero, map);
            CurrentFlowField.CreateGrid();
            CurrentFlowField.CreateCostField();

            Cell destinationCell = CurrentFlowField.GetCellFromWorldPos(worldDestination);
            CurrentFlowField.CreateIntegrationField(destinationCell);
            CurrentFlowField.CreateFlowField();

            // Mise à jour du debug visuel
            if (gridDebug != null)
            {
                gridDebug.SetFlowField(CurrentFlowField);
                gridDebug.DrawFlowField();
            }

            return CurrentFlowField;
        }

    }
}