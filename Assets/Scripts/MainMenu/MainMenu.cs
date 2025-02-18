using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator animator;

    IEnumerator LoadSceneAfterFadeOut(string sceneName)
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }

    public void PlayGame()
    {
        //animator.SetTrigger("FadeOut");
        StartCoroutine(LoadSceneAfterFadeOut("Dungeon"));
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
