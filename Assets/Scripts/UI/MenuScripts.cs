using UnityEngine;

public class MenuScripts : MonoBehaviour
{
    [SerializeField] GameObject firstCanvas;
    [SerializeField] GameObject secondCanvas;

    private void GoToSecondCanvas()
    {
        secondCanvas.SetActive(true);
        firstCanvas.SetActive(false);
    }


    private void GoToFirstCanvas()
    {
        firstCanvas.SetActive(true);
        secondCanvas.SetActive(false);
    }

}
