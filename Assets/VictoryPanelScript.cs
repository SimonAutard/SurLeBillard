using TMPro;
using UnityEngine;



public class VictoryPanelScript : MonoBehaviour
{

    GameObject VictoryPan;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VictoryPan = this.gameObject.transform.GetChild(0).gameObject;
        VictoryPan.SetActive(false);
        UIManager.Instance.VictoryPanelDisplayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(UIManager.Instance.VictoryPanelDisplayed == true)
        {
            DisplayWinnerPanel();
        }
        
    }


    void DisplayWinnerPanel()
    {
        if (GameStateManager.Instance._winner == ActivePlayerName.Clotho)
        {

            VictoryPan.transform.GetChild(0).GetComponent<TMP_Text>().text = "Victoire !!!!";
        }
        else
        {
            VictoryPan.transform.GetChild(0).GetComponent<TMP_Text>().text = "Défaite";
        }
        VictoryPan.SetActive(true);
    }
}
