using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManagement : MonoBehaviour
{

    bool _isPlayer;
    int _childNumber;
    bool _isBegining;
    bool _isRound;
    bool _alreadyShown = false;
    GameObject goddess;
    [SerializeField] GameObject Cue;
    [SerializeField] Slider sliderPower;

    private GameObject _leftSide;
    private GameObject _clothoProfile;
    private GameObject _rightSide;
    private GameObject _atroposProfile;



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
        UIManager.Instance._isClothoTurn = false;
        UIManager.Instance.AttachDialogManagement(this);
        _leftSide = GameObject.Find("leftSide");
        _clothoProfile = GameObject.Find("clothoProfile");
        _rightSide = GameObject.Find("rightSide");
        _atroposProfile = GameObject.Find("atroposProfile");

    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.Instance._isClothoTurn == true && Input.GetMouseButtonDown(0) && _isRound && UIManager.Instance.popupEnabled == false)
        {
            CloseFirstDialog();

        }
        else if (UIManager.Instance._isClothoTurn == false && Input.GetMouseButtonDown(0) && _isRound && UIManager.Instance.popupEnabled == false)
        {
            CloseFirstDialog();

        }

        if (UISingleton.Instance.isReady == true)
        {
            if (Cue.activeSelf == false)
            {
                Cue.SetActive(true);
                Cue.GetComponent<CueScript>().isValidate = false;
                sliderPower.gameObject.SetActive(true);
            }

            if (UIManager.Instance._isClothoTurn)
            {
                _leftSide.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                _clothoProfile.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                _rightSide.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                _atroposProfile.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            }
            else
            {
                _leftSide.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                _clothoProfile.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
                _rightSide.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                _atroposProfile.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            }
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

    public void ShowFirstDialogOnClothoTurn()
    {
        Debug.Log("First dialog on clotho's turn.");
        ShowFirstDialog(UIManager.Instance._isClothoTurn);
    }
            
    public void ShowFirstDialogOnAtroposTurn()
    {
        Debug.Log("First dialog on atropos's turn.");
        ShowFirstDialog(UIManager.Instance._isClothoTurn);
    }


    void ShowFirstDialog(bool isClotho)
    {
        //UISingleton.Instance.isClothoTurn = false;
        //Debug.Log(textDialog.Count);
        if (isClotho == true)
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
        if (UIManager.Instance._isClothoTurn)
        {
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
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

    public void RefreshCue(GameObject newBall)
    {
        Cue.SetActive(true);
        Cue.GetComponent<CueScript>().RenewOrb(newBall);
        Cue.SetActive(false);
    }
}
