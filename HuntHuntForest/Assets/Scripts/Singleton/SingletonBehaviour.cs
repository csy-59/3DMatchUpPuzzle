using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();
            }

            if(instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                instance = go.AddComponent<T>();
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
    }
}
