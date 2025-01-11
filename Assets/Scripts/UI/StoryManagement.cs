using UnityEngine;

public class StoryManagement : MonoBehaviour
{
    public bool isCol = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
    }

    private void Update()
    {
        if(UISingleton.Instance.isCollided == true)
        {
            ShowStory();
        }
    }

    // Update is called once per frame
    public void ShowStory()
    {

        /*foreach(Transform child in GameObject.transform)
         {
               child.gameObject.setActive(true);
        }*/
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void CloseStory()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
    }
}
