using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static UnityEditor.PlayerSettings;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Paramétrage
    [SerializeField] GameObject _toControl;

    [SerializeField]    float coefPropulsion;

    [SerializeField]    float _seuilVitessePourNvllBoule;
    [SerializeField]    public float _seuilDarret;


    [SerializeField]    Vector3 _fieldBounds;
    float _fieldSize;

    [SerializeField] int _tentativesSpawnMax;
    [SerializeField] int _indexMaxSpawn;

    //Prefab
    [SerializeField]    GameObject[] _typesDeBoules;
    int _nbTypesBoules;
    [SerializeField] GameObject _pointeurBouleUIPrefab;
    GameObject _pointeurBouleUI;

    //Son
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _sonFusion;
    [SerializeField] AudioClip _sonFusionFinale;
    [SerializeField] AudioClip _sonChocMur;
    [SerializeField] AudioClip _sonEntrechoc;

    //UI
    [SerializeField] TMP_Text _chiffreUI;
    [SerializeField] Slider _sliderUI;
    [SerializeField] LineRenderer _lineRendererPrefab;
    LineRenderer _lineRenderer;
    bool isDrawing;
    Vector3 mouseCurrentPosition;
    //Contrôles
    Vector3 _mouseStartPoint;
    Vector3 _mouseEndPoint;

    //Monitoring
    [SerializeField]    List<GameObject> listeBoule;

    [SerializeField]    bool ballsRolling;

    int score;

    // Start is called before the first frame update
    void Start()
    {
        ballsRolling = false;

        listeBoule = GameObject.FindGameObjectsWithTag("Boule").ToList();

        _nbTypesBoules = _typesDeBoules.Length;

        _fieldSize = 2 * _fieldBounds.x * 2 * _fieldBounds.z;

        score = 0;
        _chiffreUI.SetText (score+"");

        _pointeurBouleUI = _toControl.transform.GetChild(0).gameObject;

        _lineRenderer = Instantiate(_lineRendererPrefab);
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {


        //Cas où les boules roulent donc on ne peut pas jouer
        if (ballsRolling)
        {
            int nbStop = 0;
            foreach (GameObject boule in listeBoule)
            {
                if (boule.GetComponent<Rebond>().vitesse < _seuilVitessePourNvllBoule)
                {
                    nbStop++;
                }
            }
            if (nbStop == listeBoule.Count)
            {
                ballsRolling = false;
                SpawnBoule();
            }

        }//Cas où les boules sont fixes donc on peut jouer
        else if (isDrawing == true)
        {

            mouseCurrentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseCurrentPosition.y = 3;
            _lineRenderer.SetPosition(1, mouseCurrentPosition);
        }
        CheckDeath();
    }

    // Gestion des boules
    void SpawnBoule()
    {
        GameObject _toSpawn = _typesDeBoules[UnityEngine.Random.Range(0, _indexMaxSpawn)];
        float radius = _toSpawn.transform.localScale.x;
        bool _bouleApparue = false;
        for (int i = 0; i < _tentativesSpawnMax; i++)
        {
            Vector3 randomVector = new Vector3(UnityEngine.Random.Range(-_fieldBounds.x, _fieldBounds.x), 1, UnityEngine.Random.Range(-_fieldBounds.z, _fieldBounds.z));
            RaycastHit[] occupe = Physics.SphereCastAll(randomVector, radius, Vector3.up, 0.1f);
            if (occupe.Length == 0)
            {
                Debug.Log("spawn nouvelle boule en "+ randomVector);
                GameObject spawned = Instantiate(_toSpawn, randomVector, Quaternion.identity);
                _toControl = spawned;
                listeBoule = GameObject.FindGameObjectsWithTag("Boule").ToList();
                _bouleApparue = true;
                break;
            }
            Debug.Log("tenttive de création de boule i=" + i);
        }
        if (_bouleApparue)
        {
            TransmetPointeurBoule();
        }
    }

    public void FusionDesBoules(GameObject boule1,GameObject boule2, int niveauDeBoule, Vector3 vecteurPropulsion, float vitesse)
    {
        Vector3 pos = (boule1.transform.position + boule2.transform.position) / 2;
        boule1.gameObject.SetActive(false);
        boule2.gameObject.SetActive(false);
        GameObject.Destroy(boule1);
        GameObject.Destroy(boule2);

        if (_nbTypesBoules != niveauDeBoule)
        {
            GameObject _nouvelleBoule = Instantiate(_typesDeBoules[niveauDeBoule], pos, Quaternion.identity);
            _nouvelleBoule.GetComponent<Rebond>().vecteurPropulsion = vecteurPropulsion;
            _nouvelleBoule.GetComponent<Rebond>().vitesse = vitesse;
            _audioSource.PlayOneShot(_sonFusion);
        }
        else
        {
            _audioSource.PlayOneShot(_sonFusionFinale);
        }

        listeBoule = GameObject.FindGameObjectsWithTag("Boule").ToList();

        score += (int)Math.Pow(niveauDeBoule, 2);
        _chiffreUI.SetText("" + score);
    }
    //Gestion de la partie
    void TransmetPointeurBoule()
    {
        if(_pointeurBouleUI==null)
        {
            _pointeurBouleUI = Instantiate(_pointeurBouleUIPrefab,  _toControl.transform);
        }
        else
        {
            _pointeurBouleUI.transform.SetParent(_toControl.transform, false);
        }


    }

    void CheckDeath()
    {
        float _surfaceOccupée = 0;
        float _surfaceRatio = 0;
        foreach (GameObject boule in listeBoule)
        {
            float radius = boule.transform.localScale.x;
            _surfaceOccupée += (float)(Math.PI *  Math.Pow(radius,2));
            _surfaceRatio = _surfaceOccupée / _fieldSize;
            _sliderUI.value = _surfaceRatio;
        }
        if(_surfaceRatio  > 0.75f)
        {
            Recommencer();
        }
    }
    public void Recommencer()
    {
        SceneManager.LoadSceneAsync("SceneBillard");
    }

    // Controles
    private void OnMouseDown()
    {
        
        //_mouseStartPoint = Input.mousePosition;
        //_mouseStartPoint.z = _mouseStartPoint.y;
        //_mouseStartPoint.y = 0;
        Debug.Log("mousedown " + _mouseStartPoint);

        _lineRenderer.enabled = true;
        _mouseStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mouseStartPoint.y = 3;
        _lineRenderer.SetPosition(0, _mouseStartPoint);
        _lineRenderer.SetPosition(1, _mouseStartPoint);

        isDrawing = true;
    }

    private void OnMouseUp()
    {
        /*_mouseEndPoint = Input.mousePosition;
        _mouseEndPoint.z = _mouseEndPoint.y;
        _mouseEndPoint.y = 0;*/
        //Debug.Log("mouseup "+ _mouseEndPoint);
        _mouseEndPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mouseEndPoint.y = 3;
        if (!ballsRolling) { TireBoule(_mouseStartPoint - _mouseEndPoint); }

        _lineRenderer.enabled = false;
        isDrawing = false;
    }

    private void TireBoule(Vector3 vecteurPropulsion)
    {
        Debug.Log(vecteurPropulsion.normalized);
        _toControl.GetComponent<Rebond>().vitesse = vecteurPropulsion.magnitude * coefPropulsion;
        _toControl.GetComponent<Rebond>().vecteurPropulsion =  vecteurPropulsion.normalized ;
        ballsRolling = true;
    }

    //Son
    public void JoueEntrechoc()
    {
        _audioSource.PlayOneShot(_sonEntrechoc);
    }
    public void JoueChocMur()
    {
        _audioSource.PlayOneShot(_sonChocMur);
    }
}
