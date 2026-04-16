using UnityEngine;

namespace WarOfTanks.Nav
{
    public class CellDataDisplayer : MonoBehaviour
    {
        [SerializeField]
        public Cell cell;

        public void SetCell(Cell c)
        {
            cell = c;
        } 
    }
}