using UnityEngine;

public class BeginingAnimationScript : MonoBehaviour
{
    Animator m_Animator;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        //m_Animator.ResetTrigger("FadingTrigger");
        m_Animator.SetTrigger("FadingTrigger");
    }

  
    public void Transition()
    {
        //m_Animator.SetTrigger("FadingTrigger");
    }
}
