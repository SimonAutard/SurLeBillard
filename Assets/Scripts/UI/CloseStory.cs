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
        storyPopup.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        storyPopup.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        storyPopup.gameObject.transform.GetChild(2).gameObject.SetActive(false);
        UISingleton.Instance.isCollided = false;
    }
}
