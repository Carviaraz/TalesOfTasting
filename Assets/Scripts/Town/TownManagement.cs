using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownManagement : MonoBehaviour
{
    public CharacterList characterList;
    public Animator animator;
    public GameObject fadePanel;

    private void Awake()
    {
        StartCoroutine(WaitForFade("FadeIn"));
        Debug.Log("Town Script awake");
    }

    private void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        if (selectedIndex >= 0 && selectedIndex < characterList.Characters.Length)
        {
            Instantiate(characterList.Characters[selectedIndex].PlayerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Invalid character index!");
        }

        AudioManager.Instance.PlayMusic(AudioManager.Instance.townMusic);
    }

    public IEnumerator WaitForFade(string tiggerName)
    {
        fadePanel.SetActive(true);
        animator.SetTrigger(tiggerName);
        yield return new WaitForSeconds(1);
    }
}
