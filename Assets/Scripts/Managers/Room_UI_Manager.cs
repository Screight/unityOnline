using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room_UI_Manager : MonoBehaviour
{
    [SerializeField]
    private Button createButton;
    [SerializeField]
    private Button joinButton;

    [SerializeField]
    private TMPro.TMP_Text createText;
    [SerializeField]
    private TMPro.TMP_Text joinText;

    [SerializeField] GameObject m_defaultScreenGO;
    [SerializeField] GameObject m_roomScreenGO;

    [SerializeField] TMPro.TextMeshProUGUI m_userNameTMP;
    [SerializeField] TMPro.TextMeshProUGUI m_raceNameTMP;
    [SerializeField] TMPro.TextMeshProUGUI m_roomNameTMP;

    [SerializeField] TMPro.TextMeshProUGUI m_errorTMP;

    private void Awake()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);

        Photon_Manager._PHOTON_MANAGER.OnJoinedRoomEvent.AddListener(OnJoinRoom);
        Photon_Manager._PHOTON_MANAGER.OnCreateRoomEvent.AddListener(OnCreateRoom);
    }

    private void OnDestroy()
    {
        Photon_Manager._PHOTON_MANAGER.OnJoinedRoomEvent.RemoveListener(OnJoinRoom);
        Photon_Manager._PHOTON_MANAGER.OnCreateRoomEvent.RemoveListener(OnCreateRoom);
    }

    private void Start()
    {
        User user = Network_Manager._NETWORK_MANAGER.User.Value;
        m_userNameTMP.text = user.Name;
        m_raceNameTMP.text = DataManager.Instance.GetRace(user.RaceID).Value.Name;
    }
    bool m_isBusy = false;
    private void CreateRoom() 
    {
        if (m_isBusy) { return; }
        m_isBusy = true;
        Photon_Manager._PHOTON_MANAGER.CreateRoom(createText.text.ToString());
        m_errorTMP.text = "";
    }

    private void JoinRoom() 
    {
        if (m_isBusy) { return; }
        m_isBusy = true;
        m_errorTMP.text = "";
        Photon_Manager._PHOTON_MANAGER.JoinRoom(joinText.text.ToString());
    }

    void OnJoinRoom(bool p_isSuccesfull)
    {
        m_isBusy = false;
        if (p_isSuccesfull) { OpenRoomScreen(); }
        else { m_errorTMP.text = "Cannot connect to room: " + joinText.text + "."; }
        
    }

    void OnCreateRoom(bool p_isSuccesfull)
    {
        m_isBusy = false;
        if (p_isSuccesfull)
        {
            OpenRoomScreen();
            m_roomNameTMP.text = createText.text;
        }
        else
        {
            m_errorTMP.text = "Cannot create room: " + m_roomNameTMP.text + ", try a different name.";
        }
    }

    void OpenRoomScreen()
    {
        m_defaultScreenGO.SetActive(false);
        m_roomScreenGO.SetActive(true);
    }

}
