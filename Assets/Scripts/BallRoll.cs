using UnityEngine;

public class BallRoll : MonoBehaviour
{
    private float drag = 0.3f;
    [SerializeField]    protected float speed = 0;
    protected Vector3 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {

        if (speed > 0.1f)
        {
            transform.position += direction * speed * Time.deltaTime;
            speed -= drag * Time.deltaTime;
        }

        else { speed = 0; }
    }
}
