using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    bool m_isPhotonConnected;
    bool m_isServerConnected;

    [SerializeField] TMPro.TextMeshProUGUI m_infoTMP;

    private void Start()
    {
        Screen.SetResolution(960, 540, false);
        Network_Manager._NETWORK_MANAGER.OnServerResponseEvent.AddListener(HandleServerConnection);
        Network_Manager._NETWORK_MANAGER.OnServerFailedConnectionEvent.AddListener(HandleServerFailedConnection);
        Photon_Manager._PHOTON_MANAGER.OnPhotonConnectedEvent.AddListener(HandlePhotonConnection);
        Network_Manager._NETWORK_MANAGER.ConnectToServer();
    }

    private void OnDestroy()
    {
        Network_Manager._NETWORK_MANAGER.OnServerResponseEvent.RemoveListener(HandleServerConnection);
        Network_Manager._NETWORK_MANAGER.OnServerFailedConnectionEvent.RemoveListener(HandleServerFailedConnection);
        Photon_Manager._PHOTON_MANAGER.OnPhotonConnectedEvent.RemoveListener(HandlePhotonConnection);
    }

    public void HandleServerConnection(string p_data)
    {
        if(p_data == "connection")
        {
            m_isServerConnected = true;
            Network_Manager._NETWORK_MANAGER.GetRacesFromDataBase();
        }
        else if(p_data == "races")
        {
            if (m_isPhotonConnected)
            {
                SceneManager.Instance.LoadScene(SceneManager.SCENE.LOGIN);
            }
        }
    }

    public void HandlePhotonConnection()
    {
        m_isPhotonConnected = true;
        Debug.Log("Connected to photon.");
        if (m_isServerConnected)
        {
            SceneManager.Instance.LoadScene(SceneManager.SCENE.LOGIN);
        }
    }

    public void HandleServerFailedConnection()
    {
        m_infoTMP.text = "Connection to server failed. Restart the game to try again.";
    }

}
