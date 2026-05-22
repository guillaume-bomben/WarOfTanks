using TMPro;
using UnityEngine;

public class TileInfoPopup : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text text;


    public void Show(string content, Vector3 screenPos)
    {
        panel.SetActive(true);
        panel.transform.position = screenPos;
        text.text = content;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}