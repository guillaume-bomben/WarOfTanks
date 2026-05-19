using UnityEngine;
using WarOfTanks.MapGen;

namespace WarOfTanks.Stats
{
    [CreateAssetMenu(fileName = "NewStats", menuName = "War Of Tanks/Create new Stats")]
    public class Stats_SO : ScriptableObject
    {
        [Header("Health")]
        public int health;
        public int maxHealth;

        [Header("Movement")]
        public float moveSpeed;

        [Header("Combat")]
        public int attackDamage;
        public float attackRange;
        public float fireRate = 1f;
        public float visionRange;
    

        public void ApplyModifier(StatMod mod) 
        {
            var oldValue = mod.statType switch
            {
                StatsEnum.Health => health,
                StatsEnum.MoveSpeed => moveSpeed,
                StatsEnum.AttackDamage => attackDamage,
                StatsEnum.AttackRange => attackRange,
                StatsEnum.VisionRange => visionRange,
                _ => 0
            };

            var newValue = mod.modifier switch
            {
                StatsModifier.Flat => oldValue + mod.value,
                StatsModifier.Percent => oldValue * (mod.value / 100f),
                StatsModifier.Override => mod.value,
                _ => 0
            };

            switch (mod.statType)
            {
                case StatsEnum.Health:
                    health = (int)newValue;
                    break;
                
                case StatsEnum.MoveSpeed:
                    moveSpeed = newValue;
                    break;
                
                case StatsEnum.AttackDamage:
                    attackDamage = (int)newValue;
                    break;

                case StatsEnum.AttackRange:
                    attackRange = newValue;
                    break;

                case StatsEnum.VisionRange:
                    visionRange = newValue;
                    break;
            }
        }
    }

    public enum StatsEnum
    {
        Health,
        MaxHealth,
        MoveSpeed,
        AttackDamage,
        AttackRange,
        VisionRange
    }

    public enum StatsModifier
    {
        Flat,
        Percent,
        Override
    }
}