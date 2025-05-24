using UnityEngine;

public class MenuScripts : MonoBehaviour
{
    [SerializeField] GameObject firstCanvas;
    [SerializeField] GameObject secondCanvas;

    public void GoToSecondCanvas()
    {
        secondCanvas.SetActive(true);
        firstCanvas.SetActive(false);
    }


    public void GoToFirstCanvas()
    {
        firstCanvas.SetActive(true);
        secondCanvas.SetActive(false);
    }

}
