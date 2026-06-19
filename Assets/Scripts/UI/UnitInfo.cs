using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfTanks.UI
{
    public class UnitInfo : MonoBehaviour
    {
        public Unit unit;

        public Color statUpColor, statDownColor; // Si la stat est baissée / augmentée
        Color normalColor = Color.white;

        public Image teamBandImage;

        public TMP_Text unitName;

        public HealthBar unitHealthBar;
        public TMP_Text unitHealth;

        public TMP_Text unitStats;


        public void Init(WarOfTanks.Unit unit)
        {
            SetUnit(unit);
            InitContent();
        }

        private void InitContent()
        {
            switch (unit.team)
            {
                case Team.Blue:
                    teamBandImage.color = new Color(0f, 0.2f, 0.9f);
                    break;

                case Team.Red:
                    teamBandImage.color = new Color(0.9f, 0f, 0f);
                    break;

                default:
                    teamBandImage.color = new Color(0.5f, 0.5f, 0.5f);
                    break;
            }

            unitName.text = unit.name;

            StatLine
                healthLine = new StatLine(unit.GetCurrentHealth(), normalColor),
                maxHealthLine = new StatLine(unit.stats.maxHealth, normalColor),
                moveSpeedLine = new StatLine(unit.stats.moveSpeed, normalColor),
                attackDamageLine = new StatLine(unit.stats.attackDamage, normalColor),
                attackRangeLine = new StatLine(unit.stats.attackRange, normalColor),
                fireRateLine = new StatLine(unit.stats.fireRate, normalColor);

            unitHealth.text = $"{healthLine} / {maxHealthLine}";

            unitStats.text =
                $"Move Speed: {moveSpeedLine}\n" +
                $"Attack Damage: {attackDamageLine}\n" +
                $"Attack Range: {attackRangeLine}\n" +
                $"Fire Rate: {fireRateLine}";

        }

        public void RefreshContent()
        {

        }

        public void SetUnit(Unit unit)
        {
            this.unit = unit;
            unitHealthBar.SetUnit(unit);
            RefreshContent();
        }
    }

    class StatLine
    {
        float _value;
        Color _color;

        public StatLine(float value, Color color)
        {
            _value = value;
            _color = color;
        }

        public void SetValue(float value) { _value = value; }
        public void SetColor(Color color) { _color = color; }

        public override string ToString()
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(_color)}>{_value}</color>";
        }
    }
}