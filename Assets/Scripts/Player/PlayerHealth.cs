using UnityEngine;

public class PlayerHealth : MonoBehaviour, ITakeDamage
{
    [Header("Player")]
    [SerializeField] private PlayerConfig playerConfig;

    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        TakeDamage(1f);
    //    }
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        RecoverHealth(1f);
    //    }
    //}

    //public void RecoverHealth(float amount)
    //{
    //    playerConfig.CurrentHealth += amount;
    //    if (playerConfig.CurrentHealth > playerConfig.MaxHealth)
    //    {
    //        playerConfig.CurrentHealth = playerConfig.MaxHealth;
    //    }
    //}

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
        //Destroy(gameObject);
    }
}
