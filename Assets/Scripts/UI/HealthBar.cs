using UnityEngine;
using UnityEngine.UI;

namespace WarOfTanks.UI
{
    public class HealthBar : MonoBehaviour
    {
        public Vector2 offset;
        public Image fillImage; // Image de remplissage (type Filled)
        private Unit unit;

        public bool autoFindUnit = true;

        void Start()
        {
            if (autoFindUnit)
                unit = GetComponentInParent<Unit>();
        }

        void Update()
        {
            if (unit == null) return;

            fillImage.fillAmount = unit.GetHealthPercent();

            // Couleur : vert → orange → rouge
            fillImage.color = Color.Lerp(Color.green, Color.red, unit.GetHealthPercent() / 100);
        }

        void LateUpdate()
        {
            if (autoFindUnit && unit != null)
            {
                transform.rotation = Quaternion.identity;
                transform.position = unit.transform.position + new Vector3(offset.x, offset.y, 0);
            }
        }

        public void SetUnit(Unit unit)
        {
            this.unit = unit;
            autoFindUnit = false;
        }
    }
}