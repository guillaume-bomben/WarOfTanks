using UnityEngine;
using WarOfTanks.MapGen;
using WarOfTanks.Stats;

namespace WarOfTanks
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] public Stats_SO stats;
        public new string name;

        protected float currentHealth;

        protected virtual void Awake()
        {
            if (stats == null)
            {
                Debug.LogError($"{gameObject.name} : Stats_SO manquant !");
                return;
            }

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
            if (stats.maxHealth <= 0) return 0;
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

        public float GetCurrentHealth() => currentHealth;
    }
}