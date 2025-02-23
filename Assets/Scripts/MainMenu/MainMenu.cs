using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public Animator animator;
    public GameObject fadePanel;

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
