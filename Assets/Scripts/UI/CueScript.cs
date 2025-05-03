//using UnityEngine.UIElements;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class CueScript : MonoBehaviour
{
    public Transform orb;
    float distance = 0.1f;
    public float radius = 0.1f;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text cueForceDebugText;
    //Distances min et max de la queue à la bille
    float minRadius = 5;
    float maxRadius = 8;
    //Force dans la queue pour la physique
    float minForce;
    float maxForce;



    private Transform pivot;
    Vector3 clickPosition;
    Vector3 orbVector;
    public bool isValidate { get; set; }
    Vector3 queuePosition;
    bool isCollision;
    private Coroutine _aiShotDelay = null;

    private void Awake()
    {
        UISingleton.Instance.isReady = false;

    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    private void OnDestroy()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        this.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        isValidate = false;
        isCollision = false;
        minForce = PhysicsManager.Instance.CueMinForce;
        maxForce = PhysicsManager.Instance.CueMaxForce;


    }

    // Update is called once per frame
    void Update()
    {

        //oriente la queue tant que l'utilisateur n'a pas clique
        if (isValidate == false /*&&  UISingleton.Instance.isReady == true*/)
        {
            if (!UIManager.Instance._isClothoTurn && GameManager.Instance.AIStatus()) // if AI autoplay is active
            {
                clickPosition = -AIManager.Instance.NextShotInfo().Item1;
            }
            else //gestion rotation en fonction de la souris
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

                if (groundPlane.Raycast(ray, out float distance))
                {
                    clickPosition = ray.GetPoint(distance);
                    clickPosition.y = 0f;
                }
            }

            queuePosition = (clickPosition - orb.position).normalized * radius;
            transform.position = new Vector3(queuePosition.x + orb.position.x, 0.5f, queuePosition.z + orb.position.z);
            //transform.position = queuePosition + orb.position;
            orbVector = orb.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(orbVector, Vector3.up);
            transform.rotation = rotation;
            transform.rotation = Quaternion.Euler(0, -90, 0) * transform.rotation;


            if (!UIManager.Instance._isClothoTurn && GameManager.Instance.AIStatus())
            {
                radius = AIManager.Instance.NextShotInfo().Item2 * (maxForce - minForce) + minForce;
                if (_aiShotDelay == null)
                {
                    _aiShotDelay = StartCoroutine(ShotDelay(0.75f));
                }
            }
            else
            {
                //gestion puissance
                if (radius >= minRadius && radius <= maxRadius)
                {
                    radius += Input.GetAxis("Mouse ScrollWheel");
                    slider.value = radius;
                    cueForceDebugText.text = ((radius - 5) / 3).ToString();


                    if (radius < minRadius)
                    {
                        radius = minRadius;
                    }
                    if (radius > maxRadius)
                    {
                        radius = maxRadius;
                    }
                }

            }

            //enregistre que l'utilisateur a cliqué pour tirer
            if (Input.GetMouseButtonDown(0) && UISingleton.Instance.isReady == true)
            {
                UISingleton.Instance.isReady = false;
                isValidate = true;
                //HitBall(radius, queuePosition);
                //Debug.Log(UISingleton.Instance.isReady);
                //Debug.Log(Vector3.up);
            }

            //l'utilisateur a cliqué et la queue va vers la bille
            float distanceToBall = Vector3.Distance(transform.position, orb.position);
            if (isValidate == true)
            {
                if (distanceToBall > 4)
                {
                    //Debug.Log(radius);
                    transform.position = Vector3.MoveTowards(transform.position, orb.position, Time.deltaTime * radius * radius);
                }
                else
                {
                    if (!UIManager.Instance._isClothoTurn && GameManager.Instance.AIStatus())
                    {
                        EventBus.Publish(new EventApplyForceToWhiteRequest(AIManager.Instance.NextShotInfo().Item1, AIManager.Instance.NextShotInfo().Item2));
                        _aiShotDelay = null;
                    }
                    else
                    {
                        CalculateForce(radius);
                        Debug.Log("UIManager: Requesting force application.");
                        EventBus.Publish(new EventApplyForceToWhiteRequest(orbVector, UISingleton.Instance.force));
                    }
                    gameObject.SetActive(false);
                }
            }
        }
    }
    //convertit la valeur de la force entre 0 et 1
    public void CalculateForce(float _radius)
    {
        //on convertit la valeur du radius comprise entre 5 et 8 pour la mettre entre 0.2 et 1
        UISingleton.Instance.force = (_radius - minRadius) / (maxRadius - minRadius) * (maxForce - minForce) + minForce;
        orbVector.y = 0.0f;
        UISingleton.Instance.BallCuePos = orbVector;
        //Debug.Log("radius" + radius);
        //Debug.Log("force" + UISingleton.Instance.force);

    }

    public void RenewOrb(GameObject newBall)
    {
        Debug.Log("ui tries to connect white ball");
        //BallRoll ballRoll = createdBall._ball.GetComponent<BallRoll>();
        //if(ballRoll._ballId == 0) { orb = ballRoll.gameObject.transform; }
        orb = newBall.gameObject.transform;


    }

    public Vector3 GetOrbVector()
    {
        return orbVector;
    }

    private IEnumerator ShotDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UISingleton.Instance.isReady = false;
        isValidate = true;
    }


}
