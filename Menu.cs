using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartGame()
    {
        DontDestroyOnLoad(this);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        DontDestroyOnLoad(this);
        PlayerController con = new PlayerController();
        SaveLoad.Load(ref con.data);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
