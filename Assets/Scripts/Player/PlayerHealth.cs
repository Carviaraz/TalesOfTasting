using UnityEngine;

public class PlayerHealth : MonoBehaviour, ITakeDamage
{
    [Header("Player")]
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerWeapon playerWeapon;

    private GameObject gameOverPanel;

    private void Start()
    {
        playerConfig.InitializeStats();

        gameOverPanel = GameObject.Find("GameOverPanel");
        gameOverPanel.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1f);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RecoverHealth(1f);
        }
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
        Debug.Log("Player got hit");
        if (playerConfig.CurrentArmor > 0)
        {
            float remainingDamage = amount - playerConfig.CurrentArmor;

            // If the armor being negative it will be 0 instead
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
        gameOverPanel.SetActive(true); // Show Game Over Panel
        DisablePlayerControls();
    }

    private void DisablePlayerControls()
    {
        GetComponent<PlayerMovement>().enabled = false; // Disable movement
        GetComponent<PlayerWeapon>().enabled = false; // Disable weapon usage
    }

    public void ResetPlayerStats()
    {
        playerConfig.ResetStats(); // Reset stats to original values
        GetComponent<PlayerMovement>().enabled = true; // Re-enable movement
        GetComponent<PlayerWeapon>().enabled = true; // Re-enable weapon usage
    }
}
