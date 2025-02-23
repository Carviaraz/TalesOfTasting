using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] CharacterList characterList;

    [Header("Player UI")]
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image armorBar;
    [SerializeField] TextMeshProUGUI armorText;
    [SerializeField] Image energyBar;
    [SerializeField] TextMeshProUGUI energyText;

    private PlayerConfig currentPlayer;

    void Start()
    {
        // Ensure character list is assigned
        if (characterList == null || characterList.Characters.Length == 0)
        {
            Debug.LogError("CharacterList is missing or empty!");
            return;
        }

        // Get the selected character
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        if (selectedIndex < 0 || selectedIndex >= characterList.Characters.Length)
        {
            Debug.LogError("Invalid character index, resetting to default.");
            selectedIndex = 0;
        }

        currentPlayer = characterList.Characters[selectedIndex];
    }

    void Update()
    {
        updatePlayerUI();
    }

    private void updatePlayerUI()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        PlayerConfig playerConfig = characterList.Characters[selectedIndex];

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
    }
}
