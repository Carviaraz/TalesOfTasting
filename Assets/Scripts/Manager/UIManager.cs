using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] PlayerConfig playerConfig;

    [Header("Player UI")]
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image armorBar;
    [SerializeField] TextMeshProUGUI armorText;
    [SerializeField] Image energyBar;
    [SerializeField] TextMeshProUGUI energyText;

    void Update()
    {
        updatePlayerUI();
    }

    private void updatePlayerUI()
    {
        healthBar.fillAmount = Mathf.Lerp(
            healthBar.fillAmount,
            playerConfig.CurrentHealth / playerConfig.MaxHealth,
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
