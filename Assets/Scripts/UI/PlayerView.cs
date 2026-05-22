using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WarOfTanks.UI
{
    public class PlayerView : MonoBehaviour
    {
        TMP_Text username, score;

        void Start()
        {
            username = GameObject.Find("Username").GetComponent<TMP_Text>();
            score = GameObject.Find("Score").GetComponent<TMP_Text>();

            API.Player player = GameManager.Instance.loggedPlayer;

            username.text = player.username ?? "<NO PLAYER>";
            score.text = $"Score: {player.score}";
        }

        void Update()
        {

        }
    }
}