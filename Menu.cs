using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject settings;
    
    private bool _isLoading;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        settings.SetActive(false);
    }
    public void StartGame()
    {
        if (_isLoading)
        {
            return;
        }
        InterfaceMainGame.normkey = true;
        StartCoroutine(LoadScene());
    }
    public void LoadGame()
    {
        if (_isLoading)
        {
            return;
        }
        InterfaceMainGame.loadkey = true;
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        _isLoading = true;
        bool waitFading = true;
        Fader.instance.FadeIn(() => waitFading = false);
        while (waitFading)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        waitFading = true;
        Fader.instance.FadeOut(() => waitFading = false);

        while (waitFading)
        {
            yield return null;
        }
        _isLoading = false;
        Destroy(Fader.instance.gameObject);
        Destroy(this.gameObject);
    }

    public void MoveMenu(){
        menu.SetActive(settings.activeInHierarchy);
        settings.SetActive(!menu.activeInHierarchy); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
