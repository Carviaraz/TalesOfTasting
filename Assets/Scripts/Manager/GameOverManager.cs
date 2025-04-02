using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    public Animator animator;
    private GameObject finalScorePanel;
    private TextMeshProUGUI finalTitle;
    private TextMeshProUGUI finalTimeText;
    private TextMeshProUGUI finalCoinsText;
    private TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        finalScorePanel = GameObject.Find("FinalScorePanel");
        if (finalScorePanel != null)
        {
            finalScorePanel.SetActive(false);
            finalTitle = finalScorePanel.transform.Find("VerticalArrange/FinalScoreTitle_Text (TMP)")?.GetComponent<TextMeshProUGUI>();
            finalTimeText = finalScorePanel.transform.Find("VerticalArrange/Time_Horizontal/TimeUse_Text")?.GetComponent<TextMeshProUGUI>();
            finalCoinsText = finalScorePanel.transform.Find("VerticalArrange/Coin_Horizontal/CoinCollect_Text")?.GetComponent<TextMeshProUGUI>();
            finalScoreText = finalScorePanel.transform.Find("VerticalArrange/Score_Horizontal/ScoreTotal_Text")?.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("FinalScorePanel not found in GameOverManager!");
        }
    }

    public void ShowGameOverScreen(bool isVictory)
    {
        if (finalScorePanel == null) return;

        finalScorePanel.SetActive(true);
        float elapsedTime = DungeonTimer.Instance.GetElapsedTime();
        int coinsCollected = GameManager.Instance.coins;
        int finalScore = Mathf.RoundToInt(coinsCollected * (100f / (elapsedTime + 1)));

        if (isVictory)
        {
            finalTitle.text = "You Win!";
        }
        else
        {
            finalTitle.text = "Game Over";
            finalScore = Mathf.Max(finalScore / 2, 10);
        }

        finalTimeText.text = DungeonTimer.Instance.FormatTime(elapsedTime);
        finalCoinsText.text = coinsCollected.ToString();
        finalScoreText.text = finalScore.ToString();
    }

    public void OnRestart()
    {
        GameManager.Instance.SpendCoins(9999); 
        StartCoroutine(LoadSceneAfterFadeOut(1));
    }

    public void OnQuit()
    {
        GameManager.Instance.SpendCoins(9999);
        StartCoroutine(LoadSceneAfterFadeOut(0));
    }

    IEnumerator LoadSceneAfterFadeOut(int sceneIndex)
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneIndex);
    }
}
