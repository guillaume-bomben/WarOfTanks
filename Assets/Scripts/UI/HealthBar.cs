using UnityEngine;
using UnityEngine.UI;

namespace WarOfTanks.UI
{
    public class HealthBar : MonoBehaviour
    {
        public Image fillImage; // Image de remplissage (type Filled)
        private Unit unit;

        void Start()
        {
            unit = GetComponentInParent<Unit>();
        }

        void Update()
        {
            if (unit == null) return;

            fillImage.fillAmount = unit.GetHealthPercent();

            // Couleur : vert → orange → rouge
            fillImage.color = Color.Lerp(Color.red, Color.green, unit.GetHealthPercent());
        }
    }
}