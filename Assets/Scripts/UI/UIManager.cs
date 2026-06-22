using UnityEngine;

namespace WarOfTanks.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Parents UI")]
        public Transform mainCanvas;
        public Transform popUpLayer;


        void Awake() { Instance = this; }

        public T ShowUI<T>(T prefab, Transform parent = null) where T : Component
        {
            Transform target = parent != null ? parent : mainCanvas;

            T inst = Instantiate(prefab, target);
            inst.transform.SetParent(target, false);

            return inst;
        }
    }
}
