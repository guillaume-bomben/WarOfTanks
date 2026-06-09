using UnityEngine;
using UnityEngine.UI;

namespace WarOfTanks.UI
{
    /// <summary>
    /// Barre de vie au-dessus d'un tank (World Space Canvas enfant du tank).
    /// - Reste TOUJOURS horizontale (rotation monde = identite), peu importe l'orientation du tank.
    /// - Met a jour le remplissage et la couleur a chaque frame.
    /// - Se cache uniquement quand le tank est mort.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("References")]
        public Image fillImage;
        public GameObject barRoot;

        [Header("Options")]
        [Tooltip("Garde la barre horizontale meme si le tank tourne")]
        public bool counterRotate = true;
        public bool hideWhenFull = false;

        [Tooltip("Logs de diagnostic")]
        public bool debug = false;

        private Unit unit;

        void Start()
        {
            unit = GetComponentInParent<Unit>();
            if (barRoot == null) barRoot = gameObject;

            if (debug && unit == null)
                Debug.LogError($"[HealthBar] Aucune Unit trouvee dans les parents de {name} !");
        }

        void LateUpdate()
        {
            // 1. Contre-rotation : on force la barre a l'horizontale absolue (monde),
            //    independamment de la rotation du tank parent.
            if (counterRotate)
                transform.rotation = Quaternion.identity;

            // 2. Mise a jour du remplissage
            if (unit == null || fillImage == null) return;

            float percent = unit.GetHealthPercent();

            bool shouldShow = percent > 0.001f && !(hideWhenFull && percent >= 1f);
            if (barRoot.activeSelf != shouldShow)
                barRoot.SetActive(shouldShow);
            if (!shouldShow) return;

            fillImage.fillAmount = percent;
            fillImage.color = Color.Lerp(Color.red, Color.green, percent);

            if (debug)
                Debug.Log($"[HealthBar] {unit.name} : {percent:P0}");
        }
    }
}