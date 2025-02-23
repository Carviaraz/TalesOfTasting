using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private TownManagement _townManagement;

    private void Start()
    {
        _townManagement = FindObjectOfType<TownManagement>();

        if (_townManagement == null)
        {
            Debug.LogError("TownManagement not found! Make sure it's in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collide");
        Debug.Log("it is " + other.tag);
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportPlayer());
        }
    }

    private IEnumerator TeleportPlayer()
    {
        StartCoroutine(_townManagement.WaitForFade("FadeOut"));

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("Dungeon");
    }

}
