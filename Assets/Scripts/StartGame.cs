using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public void PlayGame()
    {

        UIManager.Instance.StartNewGame();
        SceneManager.LoadScene("PhysicsScene");
    }
}
