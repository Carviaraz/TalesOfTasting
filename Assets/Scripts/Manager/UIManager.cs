using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image armorBar;
    [SerializeField] TextMeshProUGUI armorText;
    [SerializeField] Image energyBar;
    [SerializeField] TextMeshProUGUI energyText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI timerText;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("GameManager instance is null! Ensure GameManager exists in the scene.");
        }
    }


    void Update()
    {
        updatePlayerUI();
    }

    private void updatePlayerUI()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        PlayerConfig playerConfig = gameManager.currentPlayer;

        healthBar.fillAmount = Mathf.Lerp(
            healthBar.fillAmount,
            playerConfig.MaxHealth / playerConfig.MaxHealth,
            10f * Time.deltaTime
        );
        armorBar.fillAmount = Mathf.Lerp(
            armorBar.fillAmount,
            playerConfig.CurrentArmor / playerConfig.MaxArmor,
            10f * Time.deltaTime
        );
        energyBar.fillAmount = Mathf.Lerp(
            energyBar.fillAmount,
            playerConfig.CurrentEnergy / playerConfig.MaxEnergy,
            10f * Time.deltaTime
        );

        healthText.text = $"{playerConfig.CurrentHealth}/{playerConfig.MaxHealth}";
        armorText.text = $"{playerConfig.CurrentArmor}/{playerConfig.MaxArmor}";
        energyText.text = $"{playerConfig.CurrentEnergy}/{playerConfig.MaxEnergy}";
        coinText.text = GameManager.Instance.coins.ToString();
        timerText.text = DungeonTimer.Instance.FormatTime(DungeonTimer.Instance.GetElapsedTime());
    }
}
