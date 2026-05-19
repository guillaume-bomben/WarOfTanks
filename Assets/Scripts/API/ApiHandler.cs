using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiHandler : MonoBehaviour
{
    const string URL = "http://localhost:5000/";

    [SerializeField] string username;
    [SerializeField] string email;
    [SerializeField] string password;


    public void Connect()
    {
        StartCoroutine(Login());
    }

    private IEnumerator Login()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(URL))
        {
            yield return req.SendWebRequest();
        }

        // GameManager.Instance.player = player;
    }
}