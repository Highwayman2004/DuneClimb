using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindAnyObjectByType<T>();
            }

            return instance;
        }
    }

    protected void UpdateInstance()
    {
        if (instance == null)
        {
            instance = FindAnyObjectByType<T>();
        }
    }
}
