using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Photon_Manager : MonoBehaviourPunCallbacks
{
    public static Photon_Manager _PHOTON_MANAGER;

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
            PhotonConnect();
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

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    //Al desconectarme
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("He implosionado porque: " + cause.ToString());
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

    //Al unirme a la sala
    public override void OnJoinedRoom()
    {
        Debug.Log("Me he unido a la sala: " + PhotonNetwork.CurrentRoom.Name + " con " + PhotonNetwork.CurrentRoom.PlayerCount + " jugadores conectados en ella.");
    }

    //Al no poderme conectar a una sala
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("No me he podido conectr a la sala dando el error: " + returnCode + " que significa: " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Si la sala esta llena y soy el master de la sala, inicio la carga del ingame para todos los jugadores 
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("InGame");
        }
    }
}
