using UnityEngine;

public class UISingleton : MonoBehaviour
{
    public static UISingleton Instance { get; private set; }
    public enum ClickState { FirstAction, SecondAction }
    public ClickState currentState = ClickState.FirstAction;
    public bool isReady = false;
    public bool isCollided = false;
    public float force;
    public bool isClothoTurn = false;
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
