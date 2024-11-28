using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class Rebond : MonoBehaviour
{
    [SerializeField]
    float coef_attenuation_collision = 0.8f;
    [SerializeField]
    float coef_atténuation_frottement = 0.2f;
    [SerializeField]
    GameManager gameManager;
    
    public int niveauDeBoule;

    public float vitesse;

    GameObject incoming_ball;

     Vector3 vecteurNormal;
    Vector3 vecteurPlan;
    float angle;
    float seuilDarret;
    public Vector3 vecteurPropulsion;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = Singleton.Instance.manager;
        seuilDarret = Singleton.Instance.manager._seuilDarret;
    }

    private void Update()
    {
        if (vitesse > 0 )
        {
            transform.Translate(vecteurPropulsion * vitesse * Time.deltaTime);
            vitesse = vitesse * (1- coef_atténuation_frottement * Time.deltaTime);
            if (vitesse < seuilDarret){
                vitesse = 0;
                vecteurPropulsion = Vector3.zero;
            }
        }
    }
    /// <summary>
    /// dgergergerge
    /// </summary>
    /// <param name="collision">  dsfcdessdecez </param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Mur")
        {
            //Debug.Log("Collision avec mur");
            vecteurNormal = collision.gameObject.GetComponent<ValeursMur>().vecteurNormal;
            vecteurPropulsion = Vector3.Reflect(vecteurPropulsion, vecteurNormal);
            vecteurPropulsion.y = 0;
            vecteurPropulsion.Normalize();
            vitesse = (vitesse) * coef_attenuation_collision;
            gameManager.JoueChocMur();
        }
        else if (collision.gameObject.CompareTag("Boule"))
        {
            Rebond scriptIncBoule = collision.gameObject.GetComponent<Rebond>();
            if (niveauDeBoule == scriptIncBoule.niveauDeBoule && scriptIncBoule.vitesse > vitesse)
            {
                Vector3 nouvellePropulsion = scriptIncBoule.vecteurPropulsion + vecteurPropulsion;
                float nouvelleVitesse = (scriptIncBoule.vitesse + vitesse) * coef_attenuation_collision / 4;
                gameManager.FusionDesBoules(collision.gameObject, gameObject, niveauDeBoule, nouvellePropulsion, nouvelleVitesse);
            }
            else
            {
                //Debug.Log("Collision avec boule");
                vecteurNormal = transform.position - collision.contacts[0].point;
                if (vecteurPropulsion != Vector3.zero)
                {
                    vecteurPropulsion = Vector3.Reflect(vecteurPropulsion, vecteurNormal);
                }
                else
                {
                    vecteurPropulsion = vecteurNormal;
                }
                vecteurPropulsion.y = 0;
                vecteurPropulsion.Normalize();

                if(scriptIncBoule.vitesse > vitesse)
                {
                    vitesse = scriptIncBoule.vitesse;
                    gameManager.JoueEntrechoc();
                }
                else
                {
                    vitesse = ((scriptIncBoule.vitesse + vitesse) * coef_attenuation_collision) / 2;

                }
                Debug.Log(gameObject.name + " vitesse est " + vitesse);
            }
        }
        

    }

}
