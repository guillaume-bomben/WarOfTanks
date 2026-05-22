using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WarOfTanks.Cosmetics
{
    public class SpriteHandler : MonoBehaviour
    {
        public TankSkin skin;
        public Color color = Color.white;

        private SpriteRenderer headSR, bodySR;
        private TMP_Text textMesh;

        void Start()
        {
            InitSprite();
            InitNameText();
        }

       void InitSprite()
        {
            color.a = 1f;

            headSR = transform.Find("Head")?.GetComponent<SpriteRenderer>();
            bodySR = transform.Find("Body")?.GetComponent<SpriteRenderer>();

            // Log pour vérifier que les SpriteRenderers sont bien trouvés
            Debug.Log($"headSR: {headSR} | bodySR: {bodySR}");

            if (headSR == null) { Debug.LogError("Head SpriteRenderer non trouvé !"); return; }
            if (bodySR == null) { Debug.LogError("Body SpriteRenderer non trouvé !"); return; }

            headSR.sortingOrder = bodySR.sortingOrder + 1;

            string skinName = skin.ToString().ToLower();

            Sprite[] headSprites = Resources.LoadAll<Sprite>(
                $"Sprites/Tanks/Head/tank_head_{skinName}_camo"
            );
            Sprite[] bodySprites = Resources.LoadAll<Sprite>(
                $"Sprites/Tanks/Body/tank_body_{skinName}_camo"
            );

            if (headSprites.Length > 0) headSR.sprite = headSprites[0];
            if (bodySprites.Length > 0) bodySR.sprite = bodySprites[0];

            headSR.color = color;
            bodySR.color = color;

            // Vérifie que le sprite est bien assigné
            Debug.Log($"Body sprite assigné : {bodySR.sprite}");
        }

        void InitNameText()
        {
            textMesh = transform.Find("Name").GetComponent<TextMeshPro>();
            if (textMesh != null)
            {
                textMesh.text = GetComponent<Unit>().name;
                textMesh.transform.position = new Vector2(0, 7f);
            }
        }
    }

    public enum TankSkin
    {
        No,
        Tiger,
        Woodland
    }
} 