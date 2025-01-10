using UnityEngine;

public class UISingleton : MonoBehaviour
{
    public static UISingleton Instance { get; private set; }

    public bool isReady = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

    }
}
