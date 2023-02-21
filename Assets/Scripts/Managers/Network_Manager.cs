using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;

public class Network_Manager : MonoBehaviour
{
    public static Network_Manager _NETWORK_MANAGER;

    private TcpClient socket;
    private NetworkStream stream;

    private StreamWriter writer;
    private StreamReader reader;

    //Ip del servidor (Maquina virtual)
    const string host = "192.168.1.61";
    const int port = 6543;

    private bool connected = false;

    private void Awake()
    {
        if (_NETWORK_MANAGER != null && _NETWORK_MANAGER != this)
        {
            Destroy(_NETWORK_MANAGER);
        }
        else 
        {
            _NETWORK_MANAGER = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (connected)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    ManageData(data);
                }
            }
        }
    }

    private void ManageData(string data) 
    {
        if (data == "Ping")
        {
            Debug.Log("Recibido ping");
            writer.WriteLine("1");
            writer.Flush();
        }
    }

    public void ConnectToServer(string nick, string password)
    {
        try
        {
            //Realizo conexion con el servidor
            socket = new TcpClient(host, port);

            //Almaceno el canal de envio y recepcion de datos
            stream = socket.GetStream();

            //Indicamos que tenemos conexion
            connected = true;

            //Genero clases para escribir y leer los datos del canal de datos
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            //Envio valores de login
            writer.WriteLine("0" + "/" + nick + "/" + password);
            writer.Flush();
        } catch { connected = false; }
    }

}
