using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoryManagement : MonoBehaviour
{
    public bool isCol = false;
    Button closeButton;
    [SerializeField] GameObject textPrefab;

    Transform popup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager.Instance.popupEnabled = false;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
        closeButton = this.gameObject.transform.GetChild(2).GetComponent<Button>();
        popup = this.gameObject.transform.GetChild(1);

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
        
        UIManager.Instance.popupEnabled=true;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
        DisplayStory();
        closeButton.onClick.AddListener(CloseStory);
    }
    
    public void CloseStory()
    {
        Debug.Log("sdjvhkqsdhfsqdfsdfsqdfqsdf");
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
        UISingleton.Instance.isCollided = false;
        UIManager.Instance.popupEnabled = false;
    }


    //Instantie texte dynamiquement à partir d'un prefab textPrefab

    public void DisplayStory()
    {
        
        foreach(UIProphecy prophecy in UIManager.Instance.prophecies)
        {
            // Instancier le prefab du texte
            GameObject newTextObject = Instantiate(textPrefab, popup);
            // Récupérer le composant Text sur le prefab
            Text textTheme1 = newTextObject.transform.GetChild(0).GetComponent<Text>();
            Text textTheme2 = newTextObject.transform.GetChild(1).GetComponent<Text>();
            Text textProphecy = newTextObject.transform.GetChild(2).GetComponent<Text>();
            // Mettre à jour le contenu du texte
            if (textTheme1 != null)
            {
                textTheme1.text = prophecy._fastestBall;
            }
            if (textTheme2 != null)
            {
                textTheme2.text = prophecy._slowestBall;
            }
            if (textProphecy != null)
            {
                textProphecy.text = prophecy._prophecy;
            }
        }

    }

    

}
