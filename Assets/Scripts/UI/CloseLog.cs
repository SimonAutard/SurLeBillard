using UnityEngine;

public class CloseLog : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    GameObject log;
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ClosePopUp()
    {
        EventBus.Publish(new EventMenuClickSignal());
        log.SetActive(false);
    }
}
