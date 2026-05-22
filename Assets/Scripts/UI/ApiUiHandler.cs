using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ApiUiHandler : MonoBehaviour
{
    public RectTransform loginPanel, registerPanel;
    public TMP_Dropdown panelDropdown;

    private void Start()
    {
        panelDropdown.onValueChanged.AddListener(ChangePanel);
    }

    public void ChangePanel(int value)
    {
        registerPanel.gameObject.SetActive(value == 0);
        loginPanel.gameObject.SetActive(value == 1);
    }
}
