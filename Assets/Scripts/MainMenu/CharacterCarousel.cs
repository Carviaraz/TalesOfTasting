using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCarousel : MonoBehaviour
{
    public List<PlayerConfig> characters; // List of ScriptableObjects
    public Image[] characterImages; // 3 UI Image elements for left, center, right
    public TextMeshProUGUI nameText, descriptionText, MaxHealthText, MaxArrmorText;
    public Button leftButton, rightButton, selectButton;
    private int currentIndex = 0;

    public Image fadeScreen;
    public Animator animator;

    void Start()
    {
        leftButton.onClick.AddListener(() => MoveCarousel(-1));
        rightButton.onClick.AddListener(() => MoveCarousel(1));
        //selectButton.onClick.AddListener(SelectCharacter);
        UpdateUI();
    }

    void MoveCarousel(int direction)
    {
        currentIndex = (currentIndex + direction + characters.Count) % characters.Count;
        UpdateUI();
    }

    void UpdateUI()
    {
        int leftIndex = (currentIndex - 1 + characters.Count) % characters.Count;
        int rightIndex = (currentIndex + 1) % characters.Count;

        characterImages[0].sprite = characters[leftIndex].Icon;
        characterImages[1].sprite = characters[currentIndex].Icon;
        characterImages[2].sprite = characters[rightIndex].Icon;

        nameText.text = characters[currentIndex].Name;
        descriptionText.text = characters[currentIndex].CharacterDescription;
        MaxHealthText.text = characters[currentIndex].MaxHealth.ToString();
        MaxArrmorText.text = characters[currentIndex].MaxArmor.ToString();

        LeanTween.moveLocalX(characterImages[1].gameObject, 0, 0.3f).setEase(LeanTweenType.easeOutQuad);
    }

    public void SelectCharacter()
    {
        PlayerPrefs.SetInt("SelectedCharacterIndex", currentIndex);

        animator.SetTrigger("FadeOut");
        StartCoroutine(LoadSceneAfterFadeOut("Town"));

        //fadeScreen.gameObject.SetActive(true);
        //LeanTween.alpha(fadeScreen.rectTransform, 1f, 0.5f).setOnComplete(() =>
        //{
        //    UnityEngine.SceneManagement.SceneManager.LoadScene("Town");
        //});


        //UnityEngine.SceneManagement.SceneManager.LoadScene("Town");
    }

    IEnumerator LoadSceneAfterFadeOut(string sceneName)
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }
}
