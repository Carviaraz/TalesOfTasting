using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public Animator animator;
    public GameObject fadePanel;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMenuMusic);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
