using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehavior<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get 
        {
            if(instance == null)
            {
                instance = FindObjectOfType<T>();
            }

            if(instance == null)
            {
                GameObject go = new GameObject();
                go.name = typeof(T).Name;
                instance = go.AddComponent<T>();
            }

            return instance;
        }
    }

    public void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
    }
}
