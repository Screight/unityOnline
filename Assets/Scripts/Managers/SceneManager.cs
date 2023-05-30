using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Singleton<SceneManager>
{
    public enum SCENE { INITIALIZE, LOGIN, LOBBY, GAME}

    static SceneManager m_instance;
    SCENE m_currentSceneID;
    int m_numberOfTotalScenes;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Initialize()
    {
        m_numberOfTotalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        m_currentSceneID = (SCENE)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadScene(SCENE p_scene)
    {
        if ((int)p_scene >= m_numberOfTotalScenes)
        {
            Debug.LogError("Scene index not found. Check if all the scenes have been added to Unity Scene Manager (File -> Build Settings).");
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)p_scene);
        m_currentSceneID = (SCENE)p_scene;
    }

    public SCENE Scene { get { return m_currentSceneID; } }

}