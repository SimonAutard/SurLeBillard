using UnityEngine;

public class CloseStory : MonoBehaviour
{

    [SerializeField]
    GameObject storyPopup;



    void Start()
    {
        
    }

    // Update is called once per frame
    public void ClosePopUp()
    {
        EventBus.Publish(new EventMenuClickSignal());
        storyPopup.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        storyPopup.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        storyPopup.gameObject.transform.GetChild(2).gameObject.SetActive(false);
        UISingleton.Instance.isCollided = false;
        UIManager.Instance.popupEnabled = false;
        UIManager.Instance.storyDisplayed = false;
        foreach(Transform Child in storyPopup.gameObject.transform.GetChild(1))
        {
            Destroy(Child.gameObject);
        }
    }
}
