using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationLobby : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    private bool loginPanelFunc;
    private bool registerPanelFunc;

    public void OpenAndCloseLoginPanel()
    {
        loginPanelFunc = !loginPanelFunc;
        lobbyPanel.SetActive(!lobbyPanel.activeSelf);
        loginPanel.SetActive(!loginPanel.activeSelf);
    }

    public void OpenAndCloseRegisterPanel()
    {
        registerPanelFunc = !registerPanelFunc;
        lobbyPanel.SetActive(!lobbyPanel.activeSelf);
        registerPanel.SetActive(!registerPanel.activeSelf);
    }
}
