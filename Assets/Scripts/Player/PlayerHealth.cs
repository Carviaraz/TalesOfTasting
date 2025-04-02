using UnityEngine;

public class PlayerHealth : MonoBehaviour, ITakeDamage
{
    [Header("Player")]
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerWeapon playerWeapon;

    private void Start()
    {
        playerConfig.InitializeStats();
    }

    public void RecoverHealth(float amount)
    {
        playerConfig.CurrentHealth += amount;
        if (playerConfig.CurrentHealth > playerConfig.MaxHealth)
        {
            playerConfig.CurrentHealth = playerConfig.MaxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        if (playerConfig.CurrentArmor > 0)
        {
            float remainingDamage = amount - playerConfig.CurrentArmor;
            playerConfig.CurrentArmor = Mathf.Max(playerConfig.CurrentArmor - amount, 0f);

            if (remainingDamage > 0)
            {
                playerConfig.CurrentHealth = Mathf.Max(playerConfig.CurrentHealth - remainingDamage, 0f);
            }
        }
        else
        {
            playerConfig.CurrentHealth = Mathf.Max(playerConfig.CurrentHealth - amount, 0f);
        }

        if (playerConfig.CurrentHealth <= 0f)
        {
            PlayerDead();
        }
    }

    public void PlayerDead()
    {
        Debug.Log("Player is ded");

        // Stop timer and energy drain when the player dies
        DungeonTimer.Instance.StopTimer();
        PlayerEnergy.Instance.StopEnergyDrain();

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOverScreen(false);
        }

        DisablePlayerControls();
    }

    public void DisablePlayerControls()
    {
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerWeapon>().enabled = false;
    }

    public void ResetPlayerStats()
    {
        playerConfig.ResetStats();
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerWeapon>().enabled = true;
    }
}
