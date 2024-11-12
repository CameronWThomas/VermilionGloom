using UnityEngine;

public abstract class GlobalSingleInstanceMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Static accessor
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null || !_instance.isActiveAndEnabled)
                _instance = FindFirstObjectByType<T>();

            return _instance;
        }
    }
    public static bool IsInstanceValid => _instance != null && _instance.isActiveAndEnabled;
    #endregion

    protected virtual void Start()
    {
        // make sure we have an instance created
        var instance = Instance;

        this.EnsureGlobalSingleInstance();
    }
}