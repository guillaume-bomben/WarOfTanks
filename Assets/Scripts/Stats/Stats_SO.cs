using UnityEngine;

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