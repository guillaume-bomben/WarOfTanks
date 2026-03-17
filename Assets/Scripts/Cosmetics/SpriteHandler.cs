using TMPro;
using UnityEngine;

namespace WarOfTanks.Cosmetics
{
    public class SpriteHandler : MonoBehaviour
    {
        public TankSkin skin;
        public Color color = Color.white; 

        private SpriteRenderer headSR, bodySR;
        private TextMeshPro textMesh;

        void Start()
        {
            InitSprite();
            InitNameText();
        }

        void InitSprite()
        {
            Sprite headSprite, bodySprite;
            color.a = 1f;

            headSR = transform.Find("Head").GetComponent<SpriteRenderer>();
            headSprite = Resources.Load<Sprite>($"Sprites/Tanks/Head/tank_head_{skin.ToString().ToLower()}_camo");
            headSR.sprite = headSprite;
            headSR.color = color;

            bodySR = transform.Find("Body").GetComponent<SpriteRenderer>();
            bodySprite = Resources.Load<Sprite>($"Sprites/Tanks/Body/tank_body_{skin.ToString().ToLower()}_camo");
            bodySR.sprite = bodySprite;
            bodySR.color = color;
        }
    
        void InitNameText()
        {
            textMesh = transform.Find("Name").GetComponent<TextMeshPro>();
            textMesh.text = GetComponent<Unit>().name;
        }
    }

    public enum TankSkin
    {
        No,
        Tiger,
        Woodland
    }
}