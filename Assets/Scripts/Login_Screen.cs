using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Login_Screen : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text loginText;
    [SerializeField] private TMP_Text passwordText;

    private void Awake()
    {
        loginButton.onClick.AddListener(Log);
    }

    private void Log() 
    {
        Network_Manager._NETWORK_MANAGER.ConnectToServer(loginText.text.ToString(), passwordText.text.ToString());
    }
}
