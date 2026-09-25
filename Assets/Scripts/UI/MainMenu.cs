using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MainHub");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
