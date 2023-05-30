using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class Photon_Manager : MonoBehaviourPunCallbacks
{
    public static Photon_Manager _PHOTON_MANAGER;

    UnityEvent m_onPhotonConnectedEvent;
    public UnityEvent OnPhotonConnectedEvent { get { return m_onPhotonConnectedEvent; } }
    private void Awake()
    {
        //Generamos singleton
        if (_PHOTON_MANAGER != null && _PHOTON_MANAGER != this)
        {
            Destroy(this.gameObject);
        }
        else 
        {
            _PHOTON_MANAGER = this;
            DontDestroyOnLoad(this.gameObject);

            //Realizo conexion
            m_onPhotonConnectedEvent = new UnityEvent();
            m_onJoinedRoomEvent = new UnityEvent<bool>();
            m_onCreateRoomEvent = new UnityEvent<bool>();
            PhotonConnect();
        }
    }

    private void Update()
    {
        if(m_timeDisconnected > 0)
        {
            m_timeDisconnected += Time.deltaTime;
        }
    }
    public void PhotonConnect() 
    {
        //Sincronizo la carga de la sala para todos los jugadores
        PhotonNetwork.AutomaticallySyncScene = true;

        //Conexion al servidor con la configuración establecida
        PhotonNetwork.ConnectUsingSettings();
    }

    //Al conectarme al servidor
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conexion realizada correctamente");
    }

    public override void OnConnected()
    {
        base.OnConnected();
        m_onPhotonConnectedEvent.Invoke();
    }

    float m_timeDisconnected = 0;

    private void OnGUI()
    {
        if (m_timeDisconnected > 0)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.normal.background = Texture2D.grayTexture;
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200f, 100f), "Connection to photon lost.. " + (int)(5 - m_timeDisconnected), style);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        m_timeDisconnected = 0.01f;
    }

    UnityEvent<bool> m_onCreateRoomEvent;
    public UnityEvent<bool> OnCreateRoomEvent { get { return m_onCreateRoomEvent; } }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        m_onCreateRoomEvent.Invoke(false);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        m_onCreateRoomEvent.Invoke(true);
    }

    //Al unirme al lobby
    public override void OnJoinedLobby()
    {
        Debug.Log("Accedido al lobby");
    }

    //Función para creare sala
    public void CreateRoom(string nameRoom) 
    {
        PhotonNetwork.CreateRoom(nameRoom, new RoomOptions {MaxPlayers = 2});
    }

    //Función para unirse a una sala
    public void JoinRoom(string nameRoom) 
    {
        PhotonNetwork.JoinRoom(nameRoom);
    }

    UnityEvent<bool> m_onJoinedRoomEvent;
    public UnityEvent<bool> OnJoinedRoomEvent { get { return m_onJoinedRoomEvent; } }
    //Al unirme a la sala
    public override void OnJoinedRoom()
    {
        Debug.Log("Me he unido a la sala: " + PhotonNetwork.CurrentRoom.Name + " con " + PhotonNetwork.CurrentRoom.PlayerCount + " jugadores conectados en ella.");
        m_onJoinedRoomEvent.Invoke(true);
    }

    //Al no poderme conectar a una sala
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("No me he podido conectr a la sala dando el error: " + returnCode + " que significa: " + message);
        m_onJoinedRoomEvent.Invoke(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Si la sala esta llena y soy el master de la sala, inicio la carga del ingame para todos los jugadores 
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("03_game_scene");
        }
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

}
