using UnityEngine;

public class cheat1Script : MonoBehaviour
{

    [SerializeField]
    GameObject description;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        description.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        Debug.Log("OK");
        description.SetActive(true);
    }

    void OnMouseExit()
    {
        Debug.Log("exit");
        description.SetActive(false);
    }
}
