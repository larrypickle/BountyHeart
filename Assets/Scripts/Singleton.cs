using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;
    // Start is called before the first frame update
    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance " + typeof(T) + " " +
                    "already destroyed. Returning null.");
            }

            lock (m_Lock)
            {
                if(m_Instance == null)
                {
                    //search for existing instance
                    m_Instance = (T)FindObjectOfType(typeof(T));
                    //if theres no instance create one
                    if(m_Instance == null)
                    {
                        // create a new gameobject to attach singleton to
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + "(Singleton)";
                        //make instance persistent
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return m_Instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }
    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }
}
