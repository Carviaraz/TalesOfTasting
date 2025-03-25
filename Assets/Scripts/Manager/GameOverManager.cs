using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public Animator animator;
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    public void OnRestart()
    {
        if (playerHealth != null)
        {
            playerHealth.ResetPlayerStats(); // Reset player stats before reloading
        }
        StartCoroutine(LoadSceneAfterFadeOut(1));
    }

    public void OnQuit()
    {
        StartCoroutine(LoadSceneAfterFadeOut(0));
    }

    IEnumerator LoadSceneAfterFadeOut(int sceneIndex)
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneIndex);
    }
}
