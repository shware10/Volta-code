using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                var t = FindFirstObjectByType<T>();
                if (t != null) _instance = t;

                else
                {
                    var newObj = new GameObject(typeof(T).Name);
                    newObj.AddComponent<T>();

                    _instance = newObj.GetComponent<T>();
                }
            }
            return _instance;
        }
    }


    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
