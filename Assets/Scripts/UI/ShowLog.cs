using UnityEngine;
using UnityEngine.UIElements;

public class ShowLog : MonoBehaviour
{
    [SerializeField]
    GameObject popUp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        popUp.SetActive(false);
    }

    // Update is called once per frame
    public void ShowPopup()
    {
        popUp.SetActive(true);
    }
}
