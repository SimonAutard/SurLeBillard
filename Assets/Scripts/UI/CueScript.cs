using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.UIElements;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Rendering;
using static UISingleton;
using UnityEngine.UI;

public class CueScript : MonoBehaviour
{
    int speed = 360;
    public Transform orb;
    public float angle;
    float distance = 0.1f;
    public float radius = 0.1f;
    [SerializeField] Vector3 offset;
    [SerializeField] Slider slider;
    public float downAngle;
    Vector3 pos;
    float horizontalInput;
    int minForce = 5;
    int maxForce = 8;

    

    private Transform pivot;
    public float maxPower = 10f; // La distance maximale de recul de la queue
    private float currentPower = 0f; // La puissance actuelle
    public float powerStep = 0.1f;
    private Vector2 initialPosition;
    Vector3 clickPosition;
    Vector3 orbVector;
    bool isValidate;
    Vector3 queuePosition;
    bool isCollision;

    private void Awake()
    {
        UISingleton.Instance.isReady = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        this.gameObject.SetActive(false);
        isValidate = false;
        isCollision = false;

    }

    // Update is called once per frame
    void Update()
    {
       
        //oriente la queue tant que l'utilisateur n'a pas clique
        if(isValidate == false /*&&  UISingleton.Instance.isReady == true*/)
        {
            //gestion rotation
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float distance))
            {
                clickPosition = ray.GetPoint(distance);
                clickPosition.y = 0f;
            }

            queuePosition = (clickPosition - orb.position).normalized * radius;
            transform.position = queuePosition + orb.position;
            orbVector = orb.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(orbVector, Vector3.up);
            transform.rotation = rotation;
            transform.rotation = Quaternion.Euler(0, -90, 0) * transform.rotation;

            //gestion puissance
            if (radius >= minForce && radius <= maxForce)
            {
                 radius += Input.GetAxis("Mouse ScrollWheel");
                 slider.gameObject.transform.GetComponent<Slider>().value = radius;
                
                if (radius < minForce)
                {
                    radius = minForce;
                }
                if (radius > maxForce)
                {
                    radius = maxForce;
                }
            }

        }

   
        if (Input.GetMouseButtonDown(0) && UISingleton.Instance.isReady == true)
        {
            isValidate = true;

            //HitBall(radius, queuePosition);
            //Debug.Log(UISingleton.Instance.isReady);
            //Debug.Log(Vector3.up);
           
        }

        //l'utilisateur a clique et la queue va vers la bille
        if(isValidate == true && !isCollision)
        {
            //Debug.Log(radius);
            CalculateForce(radius);
            transform.position = Vector3.MoveTowards(transform.position, orb.position, Time.deltaTime*radius * radius);
        }

    }

    void CalculateForce(float radius)
    {
        UISingleton.Instance.force = (radius - minForce)/(maxForce - minForce);
        //Debug.Log("radius" + radius);
        //Debug.Log("force" + UISingleton.Instance.force);
            
        }
  
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BilleBlanche")
        {
            isCollision = true;
        }
    }

}
