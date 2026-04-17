using UnityEditor;
using UnityEngine;

namespace WarOfTanks.Nav
{
    public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField };

    public class GridDebug : MonoBehaviour
    {
        public GridController gridController;
        public bool displayGrid;

        public GridDirection.DirectionMode directionMode;
        public FlowFieldDisplayType curDisplayType;

        private Vector2Int gridSize;
        private float cellRadius;
        private FlowField curFlowField;

        private Sprite[] ffIcons;

        private void Start()
        {
            ffIcons = Resources.LoadAll<Sprite>("Sprites/ff_icons");
        }

        public void SetFlowField(FlowField newFlowField)
        {
            curFlowField = newFlowField;
            newFlowField.directionMode = directionMode;
            cellRadius = newFlowField.cellRadius;
            gridSize = newFlowField.gridSize;
        }

        public void DrawFlowField()
        {
            ClearCellDisplay();

            switch (curDisplayType)
            {
                case FlowFieldDisplayType.AllIcons:
                    DisplayAllCells();
                    break;

                case FlowFieldDisplayType.DestinationIcon:
                    DisplayDestinationCell();
                    break;

                default:
                    break;
            }
        }

        private void DisplayAllCells()
        {
            if (curFlowField == null) { return; }
            foreach (Cell curCell in curFlowField.grid)
            {
                if (!IsInCameraView(curCell.worldPos)) continue;
                DisplayCell(curCell);
            }
        }

        private void DisplayDestinationCell()
        {
            if (curFlowField == null) { return; }
            DisplayCell(curFlowField.destinationCell);
        }

        private void DisplayCell(Cell cell)
        {
            GameObject iconGO = new GameObject();
            iconGO.transform.name = cell.gridIndex.ToString();
            SpriteRenderer iconSR = iconGO.AddComponent<SpriteRenderer>();
            iconGO.transform.parent = transform;
            iconGO.transform.position = cell.worldPos;
            iconSR.sortingOrder = 99;

            Sprite originIcon       = ffIcons[0];
            Sprite unwalkableIcon   = ffIcons[1];
            Sprite arrowIcon        = ffIcons[2];


            if (cell.cost == 0)
            {
                iconSR.sprite = originIcon;
            }
            else if (cell.cost == byte.MaxValue)
            {
                iconSR.sprite = unwalkableIcon;
            }
            else if (cell.bestDirection == GridDirection.North)
            {
                iconSR.sprite = arrowIcon;
                Quaternion newRot = Quaternion.Euler(0, 0, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.South)
            {
                iconSR.sprite = arrowIcon;
                Quaternion newRot = Quaternion.Euler(0, 0, 180);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.East)
            {
                iconSR.sprite = arrowIcon;
                Quaternion newRot = Quaternion.Euler(0, 0, -90);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.West)
            {
                iconSR.sprite = arrowIcon;
                Quaternion newRot = Quaternion.Euler(0, 0, 90);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.NorthEast)
            {
                iconSR.sprite = arrowIcon;
                Quaternion newRot = Quaternion.Euler(0, 0, -45);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.NorthWest)
            {
                iconSR.sprite = arrowIcon;
                Quaternion newRot = Quaternion.Euler(0, 0, 45);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.SouthEast)
            {
                iconSR.sprite = arrowIcon;
                Quaternion newRot = Quaternion.Euler(0, 0, -135);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.SouthWest)
            {
                iconSR.sprite = arrowIcon;
                Quaternion newRot = Quaternion.Euler(0, 0, 135);
                iconGO.transform.rotation = newRot;
            }
            else
            {
                iconSR.sprite = null;
            }
        }

        public void ClearCellDisplay()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (displayGrid)
            {
                if (curFlowField == null)
                {
                    DrawGrid(gridController.gridSize, Color.yellow, gridController.cellRadius);
                }
                else
                {
                    DrawGrid(gridSize, Color.green, cellRadius);
                }
            }
            
            if (curFlowField == null) { return; }

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;

            switch (curDisplayType)
            {
                case FlowFieldDisplayType.CostField:

                    foreach (Cell curCell in curFlowField.grid)
                    {
                        Handles.Label(curCell.worldPos, curCell.cost.ToString(), style);
                    }
                    break;
                    
                case FlowFieldDisplayType.IntegrationField:

                    foreach (Cell curCell in curFlowField.grid)
                    {
                        Handles.Label(curCell.worldPos, curCell.bestCost.ToString(), style);
                    }
                    break;
                    
                default:
                    break;
            }
            
        }

        private void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
        {
            Gizmos.color = drawColor;
            for (int x = 0; x < drawGridSize.x; x++)
            {
                for (int y = 0; y < drawGridSize.y; y++)
                {
                    Vector3 center = new Vector3(
                        drawCellRadius * 2 * x + drawCellRadius, 
                        drawCellRadius * 2 * y + drawCellRadius,
                        0
                    );
                    Vector3 size = Vector3.one * drawCellRadius * 2;
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
    
        private bool IsInCameraView(Vector3 worldPos)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPos);
            return viewportPos.x >= 0 && viewportPos.x <= 1 &&
                   viewportPos.y >= 0 && viewportPos.y <= 1 &&
                   viewportPos.z > 0;
        }
    }
}