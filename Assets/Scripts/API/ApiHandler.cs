using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace WarOfTanks.API
{
    public class ApiHandler : MonoBehaviour
    {
        const string URL = "http://localhost:5000/api/auth/login";

        [SerializeField] string username = "Thibault";
        [SerializeField] string email = "thibault.kine@laplateforme.io";
        [SerializeField] string password = "azerty";


        public void Connect()
        {
            StartCoroutine(Login(URL));
        }

        private IEnumerator Login(string url)
        {
            var data = new LoginRequest
            {
                email = email,
                password = password
            };
            string body = JsonUtility.ToJson(data);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
            // Debug.Log(body);

            UnityWebRequest req = new UnityWebRequest(url, "POST");
            req.SetRequestHeader("Content-Type", "application/json");
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.Log($"API Error: {req.error}");

            API.Player player = JsonUtility.FromJson<API.Player>(req.downloadHandler.text);
            // Debug.Log(player.score); 

            GameManager.Instance.loggedPlayer = player;
        }

        [System.Serializable]
        public struct LoginRequest
        {
            public string email;
            public string password;
        }
    }
}