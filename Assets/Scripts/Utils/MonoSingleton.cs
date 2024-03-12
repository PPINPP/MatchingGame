using Sirenix.OdinInspector;
using UnityEngine;

public class MonoSingleton<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
{
    private static T instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = "(MonoSingleton) " + typeof(T);

                        DontDestroyOnLoad(singleton);
                    }
                }

                return instance;
            }
        }
    }
    public virtual void Init()
    {

    }

    protected virtual void Awake()
    {
        Init();
    }
    public virtual void Dispose()
    {
        Destroy(this.gameObject);
    }
}