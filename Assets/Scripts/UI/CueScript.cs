using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Rendering;


public class CueScript : MonoBehaviour
{
    int speed = 360;
    public Transform orb;
    public float angle;
    float distance = 0.1f;
    public float radius = 0.1f;
    [SerializeField] Vector3 offset;
    public float downAngle;
    Vector3 pos;
    float horizontalInput;

    private Transform pivot;
    public float maxPower = 10f; // La distance maximale de recul de la queue
    private float currentPower = 0f; // La puissance actuelle
    public float powerStep = 0.1f;
    private Vector2 initialPosition;
    Vector3 clickPosition;
    Vector3 orbVector;
    bool isValidate;
    Vector3 queuePosition;

    private void Awake()
    {
        UISingleton.Instance.isReady = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //pivot = orb;
        //transform.parent = pivot;
        //transform.localPosition = Vector3.zero;
        //transform.position += Vector3.up * radius;
        //transform.position = Vector3.up + offset;
        //initialCuePosition = transform.position;
        //initialPosition = transform.position - orb.position; // Calcul du décalage initial
        isValidate = false;

    }

    // Update is called once per frame
    void Update()
    {
        /*float scrollInput = Input.GetAxis("Mouse X");
        Vector3 newPosition = transform.position + Vector3.up * scrollInput * maxPower;
        // Adjust the object's position
        transform.position = newPosition;*/
        //transform.LookAt(orb.position);
        //horizontalInput = Input.GetAxis("Mouse X") * speed * Time.deltaTime;
        //transform.RotateAround(orb.position, Vector3.up, 0);

        // this.transform.Rotate((Input.GetAxis("Mouse X") * speed * Time.deltaTime), (Input.GetAxis("Mouse Y") * speed * Time.deltaTime), 0, Space.World);

        if(isValidate == false /*&&  UISingleton.Instance.isReady == true*/)
        {
            
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

        if (radius >= 5 && radius <= 8)
        {
            radius += Input.GetAxis("Mouse ScrollWheel");
            if (radius < 5)
            {
                radius = 5;
            }
            if (radius > 8)
            {
                radius = 8;
            }
        }

        }
        if (Input.GetMouseButtonDown(0) /*&& UISingleton.Instance.isReady == true*/)
        {
            isValidate = true;

            //HitBall(radius, queuePosition);
            Debug.Log(Vector3.up);
           
        }
        if(isValidate == true)
        {

            //queuePosition = (clickPosition - orb.position).normalized * radius;
            //transform.Translate(queuePosition * Time.deltaTime);
            //transform.position = queuePosition + orb.position;
            //orbVector = orb.position - transform.position;
            Debug.Log(radius);
            transform.position = Vector3.MoveTowards(transform.position, orb.position, Time.deltaTime*radius);
                
            
            
        }



        //this.transform.Translate(queuePosition * Input.GetAxis("Mouse ScrollWheel"));


        //Vector3 normalizedDirection = (this.transform.position - orb.transform.position).normalized;
        //this.transform.Translate(normalizedDirection * Input.GetAxis("Mouse ScrollWheel"));
        //this.transform.Translate(this.transform.position * Input.GetAxis("Mouse ScrollWheel"));
        //transform.LookAt(orb);

        /*float scrollInput = Input.GetAxis("Mouse ScrollWheel");  // Entrée de la molette
        if (scrollInput != 0)
        {
            currentPower = Mathf.Clamp(currentPower + scrollInput * powerStep, 0f, maxPower);
            AdjustCuePosition();
        }*/


        //float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        //Vector3 newPosition = transform.position + Vector3.up * scrollInput * maxPower;
        // Adjust the object's position
        //transform.position = newPosition;



        /* // Position de la souris en coordonnées mondiales
         Vector3 mousePosition = Input.mousePosition;
         mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);  // Profondeur de la caméra pour conversion correcte
         Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
         worldMousePosition.z = 0;  // Bloquer le Z pour 2D

         // Calcul de la direction et de l'angle
         Vector3 direction = worldMousePosition - orb.position;
         float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

         // Appliquer la rotation à la queue autour du pivot
         transform.position = orb.position;  // La queue suit la bille
         transform.rotation = Quaternion.Euler(0, 0, angle);*/
    }

    /* void turn()
     {
         transform.position = orb.position + offset;
         transform.LookAt(orb.position);
         transform.localEulerAngles = new Vector3(downAngle, transform.localEulerAngles.y, 0);
     }*/


    /*void HitBall(float radius, Vector3 queuePosition)
    {
        Debug.Log("OK");
        //this.transform.Translate(queuePosition * Input.GetAxis("Mouse ScrollWheel"));
       Debug.Log(Vectp)
        transform.Translate(Vector3.up * Time.deltaTime);
    }*/
}
