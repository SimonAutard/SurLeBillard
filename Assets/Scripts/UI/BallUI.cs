using UnityEngine;

public class BallUI : MonoBehaviour
{
    [SerializeField] GameObject billiardBall;

    // Update is called once per frame
    void Update()
    {
        if(billiardBall != null) { transform.position = billiardBall.transform.position; }
        else Destroy(gameObject);
        
    }
}
