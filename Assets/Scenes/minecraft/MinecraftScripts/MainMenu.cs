using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "BasicScene"; 

    public void StartGame()
    {
        Time.timeScale = 1f; // just in case menu was paused
        SceneManager.LoadScene(sceneToLoad); //unitys scene manager loads the specified scene scenetoload basicscene
    }
}