using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;
using System.Collections;
using static UISingleton;
using UnityEngine.UI;

public class DialogManagement : MonoBehaviour
{
 
    bool _isPlayer;
    int _childNumber;
    bool _isBegining;
    bool _isRound;
    bool _alreadyShown= false;
    GameObject goddess;
    [SerializeField] GameObject Cue;
    [SerializeField] Slider sliderPower;
  


    Dictionary<string, string> textDialog = new Dictionary<string, string>()
        {
            {"begining", "A toi de jouer !"},
            {"failure", "Mauvais coup"},
            {"victory", "Bien joué, tu as gagné"},
            {"defeat", "Dommage pour toi, son destin est scellé"}
        };
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        /*this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        _childNumber = 0;*/

        //Debug.Log(textDialog.Count);
        //UISingleton.Instance.isClothoTurn = true;
        _isRound = false;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
        if (UISingleton.Instance.isClothoTurn == true && _alreadyShown == false)
        {
            ShowFirstDialog(UISingleton.Instance.isClothoTurn);
        }

        if (UISingleton.Instance.isClothoTurn == false && _alreadyShown == false)
        {
            ShowFirstDialog(UISingleton.Instance.isClothoTurn);
        }

        if (UISingleton.Instance.isClothoTurn == true && Input.GetMouseButtonDown(0) && _isRound)
        {
            CloseFirstDialog();

        }
        if (UISingleton.Instance.isClothoTurn == false && Input.GetMouseButtonDown(0) && _isRound)
        {
            CloseFirstDialog();

        }

        if (UISingleton.Instance.isReady == true)
        {
            Cue.SetActive(true);
            sliderPower.gameObject.SetActive(true);
        }
        /*if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("space");
            //ShowDialog();
           
                Debug.Log("clic");
            switch (_childNumber)
                {
                   case 0:
                        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                        _childNumber = 1;
                        break;
                   case 1:
                        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        _childNumber = 0;
                        break;
                }*/


            
        }
        
        void ShowFirstDialog(bool isClotho)
        {
            //UISingleton.Instance.isClothoTurn = false;
            //Debug.Log(textDialog.Count);
            if(isClotho == true)
            {
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                goddess = this.gameObject.transform.GetChild(1).gameObject;
                _isRound = true;
            }

            else
            {
                this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                goddess = this.gameObject.transform.GetChild(0).gameObject;
                _isRound = true;

            }
            _alreadyShown = true;
            goddess.GetComponentInChildren<TextMeshProUGUI>().text = textDialog["begining"];
            

         }

        void ShowDialog()
        {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            _childNumber = 0;
            Debug.Log("show dialog");
            if (Input.GetMouseButtonDown(0))
            { 
                Debug.Log("clic");
                switch (_childNumber)
                {
                    case 0:
                        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                        _childNumber = 1;
                        break;
                    case 1:
                        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        _childNumber = 0;
                        break;
                }
            }

        
        }

    void CloseFirstDialog()
    {
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        //Debug.Log("début tour");
        _isRound = false;
        StartCoroutine(timer());
        UISingleton.Instance.isReady = true;
        //UISingleton.Instance.currentState = ClickState.SecondAction;
    }

    IEnumerator timer()
    {
       
        //Wait for 4 seconds
        yield return new WaitForSeconds(2f);

    }
}
