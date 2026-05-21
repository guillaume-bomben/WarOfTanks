using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace WarOfTanks.API
{
    public class ApiHandler : MonoBehaviour
    {
        public ApiUiHandler apiUiHandler;

        const string BASE_URL = "http://localhost:5000/api";

        [SerializeField] string username = "Thibault";
        [SerializeField] string email = "thibault.kine@laplateforme.io";
        [SerializeField] string password = "azerty";


        public void SetUsername(string username) { this.username = username; Debug.Log($"Username changed to '{username}'"); }
        public void SetEmail(string email) { this.email = email; Debug.Log($"Email changed to '{email}'"); }
        public void SetPassword(string password) { this.password = password; Debug.Log($"Password changed to '{password}'"); }

        public void Register()
        {
            StartCoroutine(_Register(BASE_URL + "/auth/register"));
        }

        public void Login()
        {
            StartCoroutine(_Login(BASE_URL + "/auth/login"));
        }

        private IEnumerator _Login(string url)
        {
            var data = new LoginRequest
            {
                email = email,
                password = password
            };
            string body = JsonUtility.ToJson(data);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
            // Debug.Log(body);

            var req = InitWebRequest(url, bodyRaw, HttpMethod.POST);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"API Error: {req.error}");
                yield break;
            }

            API.Player player = JsonUtility.FromJson<API.Player>(req.downloadHandler.text);
            // Debug.Log(player.score); 

            GameManager.Instance.loggedPlayer = player;
            if (player != null)
                Debug.Log($"Successfully logged in as {player.username}");
        }

        public IEnumerator _Register(string url)
        {
            var data = new RegisterRequest
            {
                username = username,
                email = email,
                password = password
            };
            string body = JsonUtility.ToJson(data);
            Debug.Log(body);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);

            var req = InitWebRequest(url, bodyRaw, HttpMethod.POST);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"API Error: {req.error}");
                yield break;
            }

            API.Player player = JsonUtility.FromJson<API.Player>(req.downloadHandler.text);
            Debug.Log($"Successfully registered {player.username}");

            // Redirect to "login" form
            apiUiHandler.panelDropdown.value = 1;
            apiUiHandler.ChangePanel(1);
        }

        public IEnumerator _GetLeaderboard(string url)
        {
            var req = InitWebRequest(url, null, HttpMethod.GET);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.Log($"API Error: {req.error}");

            // Display leaderboards...
        }

        private UnityWebRequest InitWebRequest(string url, byte[] rawData, HttpMethod method)
        {
            string _method = ""; 
            switch (method)
            {
                case HttpMethod.GET: _method = "GET"; break;
                case HttpMethod.POST: _method = "POST"; break;
                case HttpMethod.PUT: _method = "PUT"; break;
                case HttpMethod.PATCH: _method = "PATCH"; break;
                case HttpMethod.DELETE: _method = "DELETE"; break;
            }
            UnityWebRequest req = new UnityWebRequest(url, _method);
            req.SetRequestHeader("Content-Type", "application/json");
            req.uploadHandler = new UploadHandlerRaw(rawData);
            req.downloadHandler = new DownloadHandlerBuffer();

            return req;
        }

        enum HttpMethod
        {
            GET, POST, PUT, PATCH, DELETE
        }

        [System.Serializable]
        public struct LoginRequest
        {
            public string email;
            public string password;
        }

        [System.Serializable]
        public struct RegisterRequest
        {
            public string username;
            public string email;
            public string password;
        }
    }
}