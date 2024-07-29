using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton_AutoCreate<T> : MonoBehaviour where T : MonoSingleton_AutoCreate<T>
{
    private static T m_instance;
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                var gameObject = new GameObject(typeof(T).FullName);
                //gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                m_instance = gameObject.AddComponent<T>();
                m_instance.Init();
            }
            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        m_instance = this as T;
        Init();
    }

    protected bool hasInit;
    protected virtual void Init()
    {
        if (hasInit) return;
        hasInit = true;
    }
}
