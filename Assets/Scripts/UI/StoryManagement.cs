using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManagement : MonoBehaviour
{
    public bool isCol = false;
    Button closeButton;
    [SerializeField] GameObject textPrefab;

    //bool storyDisplayed = false;

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

        UIManager.Instance.storyDisplayed = false;

    }

    private void Update()
    {
        //On v�rifie qu'il y a une collision et que la narration est d�j� affich�e dans la popup
        if(UISingleton.Instance.isCollided == true && UIManager.Instance.storyDisplayed == false)
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

        //closeButton.onClick.AddListener(CloseStory);
    }
    
   /* public void CloseStory()
    {
        Debug.Log("sdjvhkqsdhfsqdfsdfsqdfqsdf");
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
        UISingleton.Instance.isCollided = false;
        UIManager.Instance.popupEnabled = false;
    }*/


    //Instantie texte dynamiquement � partir d'un prefab textPrefab

    public void DisplayStory()
    {
        //GameObject newTextObject = Instantiate(textPrefab, popup);

        //Debug.Log("khfkuhfkfeh" + newTextObject.transform.childCount);
        //textObject = thisBlock.transform.GetChild(0).gameObject;
        //textObject.GetComponent<TextMeshPro>().text = "" + currentResult;
       
        //newTextObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "hello";
       
        foreach(UIProphecy prophecy in UIManager.Instance.prophecies)
        {

           
            // Instancier le prefab du texte
            GameObject newTextObject = Instantiate(textPrefab, popup);
            // R�cup�rer le composant Text sur le prefab
            TMP_Text textTheme1 = newTextObject.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text textTheme2 = newTextObject.transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text textProphecy = newTextObject.transform.GetChild(2).GetComponent<TMP_Text>();

            // Mettre � jour le contenu du texte
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

        UIManager.Instance.storyDisplayed = true;

    }

    

}
