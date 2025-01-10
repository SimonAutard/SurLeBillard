using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;

public class DialogManagement : MonoBehaviour
{
 
    bool _isPlayer;
    int _childNumber;
    bool _isBegining;
    bool _isRound;
    GameObject goddess;

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
        _isBegining = true;
        _isRound = false;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
        if (_isBegining == true)
        {
            ShowFirstDialog();
        }

        if (_isBegining == false && Input.GetMouseButtonDown(0) && _isRound)
        {
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            Debug.Log("début tour");
            _isRound = false;
            UISingleton.Instance.isReady = true;

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
        
        void ShowFirstDialog()
        {
            _isBegining = false;
            //Debug.Log(textDialog.Count);
            this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            goddess = this.gameObject.transform.GetChild(1).gameObject;
            goddess.GetComponentInChildren<TextMeshProUGUI>().text = textDialog["begining"];
        _isRound = true;

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
}
