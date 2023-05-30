using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine.Events;

public class Network_Manager : MonoBehaviour
{
    User? m_user;

    public static Network_Manager _NETWORK_MANAGER;

    private TcpClient socket;
    private NetworkStream stream;

    private StreamWriter writer;
    private StreamReader reader;

    UnityEvent<string> m_onServerResponseEvent;
    public UnityEvent<string> OnServerResponseEvent { get { return m_onServerResponseEvent; } }

    //Ip del servidor (Maquina virtual)
    const string host = "192.168.0.16";
    const int port = 6543;

    private bool connected = false;

    float m_timeSinceLastPing = 0;

    private void Awake()
    {
        if (_NETWORK_MANAGER != null && _NETWORK_MANAGER != this)
        {
            Destroy(_NETWORK_MANAGER);
        }
        else 
        {
            _NETWORK_MANAGER = this;
            m_onServerResponseEvent = new UnityEvent<string>();
            m_onServerFailedConnectionEvent = new UnityEvent();
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (connected)
        {
            m_timeSinceLastPing += Time.deltaTime;
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    ManageData(data);
                }
            }
            if(m_timeSinceLastPing > 10)
            {
                Application.Quit();
            }
        }
    }

    private void OnGUI()
    {
        if(m_timeSinceLastPing > 6)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.normal.background = Texture2D.grayTexture;
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200f, 100f), "Connection to server lost.. " + (int)(10-m_timeSinceLastPing),style);
        }
    }

    private void ManageData(string p_data) 
    {
        if (p_data == "connection")
        {
            Debug.Log("Connection realized!");
            m_onServerResponseEvent.Invoke("connection");
        }
        else if (p_data == "ping")
        {
            Debug.Log("Recibido ping");
            m_timeSinceLastPing = 0;
            try
            {
                writer.WriteLine("1");
                writer.Flush();
            }
            catch
            {
                connected = false;
            }
        }else if (p_data.Split(";")[0] == "login")
        {
            string[] userData = p_data.Split(";");

            if (userData[1] == "1")
            {
                m_user = new User(userData[2], System.Int32.Parse(userData[3]));
            }
            else { m_user = null; }
            m_onServerResponseEvent.Invoke("login");
        }
        else if (p_data.Split(";")[0] == "races")
        {
            DataManager.Instance.HandleRacesResponseFromServer(p_data);
            m_onServerResponseEvent.Invoke("races");
        }
        else if(p_data.Split(";")[0] == "register")
        {
            m_onServerResponseEvent.Invoke(p_data);
        }
    }

    UnityEvent m_onServerFailedConnectionEvent;
    public UnityEvent OnServerFailedConnectionEvent { get { return m_onServerFailedConnectionEvent; } }
    public void ConnectToServer()
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

            writer.WriteLine("3");
            writer.Flush();
        }
        catch {
            connected = false;
            m_onServerFailedConnectionEvent.Invoke();
        }
    }

    public void Login(string nick, string password)
    {
        try
        {
            //Envio valores de login
            writer.WriteLine("0" + "/" + nick + "/" + password);
            writer.Flush();
        } catch { connected = false; }
    }

    public void Register(string nick, string password, int p_raceID)
    {
        try
        {
            //Envio valores de login
            writer.WriteLine("4" + "/" + nick + "/" + password + "/" + p_raceID.ToString());
            writer.Flush();
        }
        catch { connected = false; }
    }

    public void GetRacesFromDataBase()
    {
        try
        {
            //Envio valores de login
            writer.WriteLine("2");
            writer.Flush();
        }
        catch { connected = false; }
    }

    public User? User { get { return m_user; } }

}
