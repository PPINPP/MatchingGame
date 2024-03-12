using Sirenix.OdinInspector;
using UnityEngine;

public class MonoInstance<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
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
                        singleton.name = "(MonoInstance) " + typeof(T);
                    }
                }

                return instance;
            }
        }
    }

    public void Ping()
    {
    }
    public virtual void Init()
    {

    }

    #region override method
    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }
    public virtual void Dispose()
    {
        Destroy(this.gameObject);
    }
    #endregion
}
