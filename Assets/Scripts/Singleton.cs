using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return default;
            }
            T instance = default;
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {
                        GameObject gameObject = new GameObject();
                        _instance = gameObject.AddComponent<T>();
                        gameObject.name = "(singleton) " + typeof(T).ToString();
                        DontDestroyOnLoad(gameObject);
                    }
                }
                instance = _instance;
            }
            return instance;
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}