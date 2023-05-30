using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T m_instance = null;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject gO = new GameObject($"{typeof(T).Name} (singleton)");
                m_instance = gO.AddComponent<T>();
                m_instance.Initialize();
            }
            return m_instance;
        }
        private set { }
    }

    protected virtual void Initialize()
    {

    }

    protected virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as T;
            Initialize();
        }
    }

}