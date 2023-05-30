using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginRegisterController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_loadingTMP;

    [SerializeField] GameObject m_loginScreenGO;
    [SerializeField] GameObject m_registerScreenGO;

    [SerializeField] private Button m_loginButton;
    [SerializeField] private TMP_InputField m_userLoginTMP;
    [SerializeField] private TMP_InputField m_passwordLoginTMP;
    [SerializeField] TextMeshProUGUI m_errorMessageLoginTMP;

    [SerializeField] private Button m_registerButton;
    [SerializeField] private TMP_InputField m_userRegisterTMP;
    [SerializeField] private TMP_InputField m_passwordRegisterTMP;
    [SerializeField] TextMeshProUGUI m_errorMessageRegisterTMP;

    [SerializeField] TMP_Dropdown m_raceDropDown;

    Dictionary<int, int> m_dropdownIndexToRaceID;

    bool m_isBusy = false;

    private void Awake()
    {
        Network_Manager._NETWORK_MANAGER.OnServerResponseEvent.AddListener(HandleLoginResponseFromServer);
        Network_Manager._NETWORK_MANAGER.OnServerResponseEvent.AddListener(HandleRegisterResponseFromServer);

        m_loginScreenGO.SetActive(true);
        m_registerScreenGO.SetActive(false);

        m_dropdownIndexToRaceID = new Dictionary<int, int>();
        m_loadingTMP.transform.parent.gameObject.SetActive(false);

        m_passwordLoginTMP.contentType = TMP_InputField.ContentType.Password;
    }

    private void Start()
    {
        int index = -1;
        DataManager.Instance.RaceList.ForEach((Race race) =>
        {
            index++;
            m_raceDropDown.options.Add(new TMP_Dropdown.OptionData() { text = race.Name });
            m_dropdownIndexToRaceID.Add(index, race.ID);
        });
    }

    private void OnDestroy()
    {
        Network_Manager._NETWORK_MANAGER.OnServerResponseEvent.RemoveListener(HandleLoginResponseFromServer);
        Network_Manager._NETWORK_MANAGER.OnServerResponseEvent.RemoveListener(HandleRegisterResponseFromServer);
    }

    public void Login() 
    {
        if (m_isBusy) { return; }

        if (m_userLoginTMP.text.Length == 0 || m_passwordLoginTMP.text.Length == 0)
        {
            m_errorMessageLoginTMP.text = "Fields cannot be empty.";
            return;
        }

        m_errorMessageLoginTMP.text = "";

        string user = m_userLoginTMP.text;
        user = user.Replace(":", "");
        user = user.Replace(";", "");
        user = user.Replace("/", "");

        string pass = m_passwordLoginTMP.text;
        pass = pass.Replace(":", "");
        pass = pass.Replace(";", "");
        pass = pass.Replace("/", "");

        m_loadingTMP.text = "Logging in..";
        m_loadingTMP.transform.parent.gameObject.SetActive(true);
        Network_Manager._NETWORK_MANAGER.Login(user, pass);
        m_isBusy = true;
    }

    public void Register()
    {
        if (m_isBusy) { return; }

        if(m_userRegisterTMP.text.Contains(":") || m_userRegisterTMP.text.Contains(";") || m_passwordRegisterTMP.text.Contains(":") || m_passwordRegisterTMP.text.Contains(";"))
        {
            m_errorMessageRegisterTMP.text = "Characters ':' , ';' and '/' are not valid.";
            return;
        }
        else if(m_userRegisterTMP.text.Length == 0 || m_passwordRegisterTMP.text.Length == 0)
        {
            m_errorMessageRegisterTMP.text = "Fields cannot be empty.";
            return;
        }

        m_errorMessageRegisterTMP.text = "";
        m_loadingTMP.text = "Registering..";
        m_loadingTMP.transform.parent.gameObject.SetActive(true);

        Network_Manager._NETWORK_MANAGER.Register(m_userRegisterTMP.text, m_passwordRegisterTMP.text.ToString(), m_dropdownIndexToRaceID[m_raceDropDown.value]);
        m_isBusy = true;
    }

    void HandleLoginResponseFromServer(string p_data)
    {
        if(p_data != "login") { return; }

        if (Network_Manager._NETWORK_MANAGER.User.HasValue)
        {
            SceneManager.Instance.LoadScene(SceneManager.SCENE.LOBBY);
            Photon_Manager._PHOTON_MANAGER.JoinLobby();
            m_errorMessageLoginTMP.text = "";
        }
        else { m_errorMessageLoginTMP.text = "User and password do not match!"; }
        m_isBusy = false;
        m_loadingTMP.transform.parent.gameObject.SetActive(false);
    }

    void HandleRegisterResponseFromServer(string p_data)
    {
        string[] registerData = p_data.Split(";");

        if (registerData[0] != "register") { return; }

        m_isBusy = false;
        
        if (registerData[1] == "0")
        {
            m_errorMessageRegisterTMP.text = "User name already in use.";
        }
        else
        {
            OpenLoginScreen();
            m_userLoginTMP.text = registerData[2];
        }
        
        m_loadingTMP.transform.parent.gameObject.SetActive(false);
    }

    public void OpenRegisterScreen()
    {
        if (m_isBusy) { return; }
        m_errorMessageRegisterTMP.text = "";
        m_loginScreenGO.SetActive(false);
        m_registerScreenGO.SetActive(true);
    }

    public void OpenLoginScreen()
    {
        if (m_isBusy) { return; }
        m_errorMessageLoginTMP.text = "";
        m_userLoginTMP.text = "";
        m_passwordLoginTMP.text = "";
        m_loginScreenGO.SetActive(true);
        m_registerScreenGO.SetActive(false);

        m_userRegisterTMP.text = "";
        m_passwordRegisterTMP.text = "";
        m_raceDropDown.value = 0;
    }

}
