using UnityEngine;

namespace WarOfTanks
{
    public class Tank : Unit
    {
        private Transform bodyTransform, headTransform;

        // Start is called before the first frame update
        void Start()
        {
            bodyTransform = transform.Find("Body");
            headTransform = transform.Find("Head");
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
