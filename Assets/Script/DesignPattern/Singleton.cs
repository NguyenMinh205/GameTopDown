using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton <T> : MonoBehaviour where T : MonoBehaviour 
{
    public static T instance;
    public static bool dontDestroyOnLoad = true;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>(true);
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).Name + "-singleton";
                    if (dontDestroyOnLoad)
                    {
                        DontDestroyOnLoad(singletonObject);
                    }
                }    
            }    
            return instance;
        }

    }

    public static void KeepALive(bool keepAlive)
    {
        dontDestroyOnLoad = keepAlive;
    }

    private void Awake()
    {
        if (instance != null && this.GetInstanceID() != instance.GetInstanceID())
        {
            Destroy(this);
            return;
        }

        instance = (T)(MonoBehaviour)this;
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(instance);
        }    
    }
}
