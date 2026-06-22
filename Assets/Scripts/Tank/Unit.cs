using UnityEngine;
using WarOfTanks.MapGen;
using WarOfTanks.Stats;

namespace WarOfTanks
{
    public abstract class Unit : MonoBehaviour
    {
        [Tooltip("Asset de stats de base (partagé). Une COPIE est créée par tank à l'Awake.")]
        [SerializeField] private Stats_SO baseStats;

        /// <summary>Stats propres à CETTE instance (copie runtime du baseStats).</summary>
        public Stats_SO stats { get; private set; }

        public new string name;

        protected float currentHealth;

        protected virtual void Awake()
        {
            if (baseStats == null)
            {
                Debug.LogError($"{gameObject.name} : Stats_SO manquant !");
                return;
            }

            // ── Clone : chaque tank a SA propre copie, le terrain/modifiers
            //    n'affectent plus l'asset partagé ni les autres tanks.
            stats = Instantiate(baseStats);

            if (stats.maxHealth <= 0)
            {
                Debug.LogError($"{gameObject.name} : maxHealth = {stats.maxHealth} — mets une valeur > 0 dans le ScriptableObject !");
                return;
            }

            currentHealth = stats.maxHealth;
            Debug.Log($"{gameObject.name} initialisé avec {currentHealth} PV");
        }

        public virtual void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, stats.maxHealth);

            OnHealthChanged();

            if (currentHealth <= 0)
                OnDeath();
        }

        protected virtual void OnHealthChanged() { }
        protected virtual void OnDeath() { }

        public float GetHealthPercent()
        {
            if (stats == null || stats.maxHealth <= 0) return 0f;
            return currentHealth / stats.maxHealth;
        }

        void ApplyTerrainEffect()
        {
            TerrainDataModifier mod = GameManager.Instance.mapGenerator.GetModifierAtWorldPos(transform.position);
            if (mod == null)
                return;

            foreach (var statMod in mod.modifiers)
            {
                stats.ApplyModifier(statMod);
            }
        }
    }
}