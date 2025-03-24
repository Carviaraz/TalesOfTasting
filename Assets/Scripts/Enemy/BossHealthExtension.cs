using UnityEngine;

public class BossHealthExtension : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private ActionBossAttack bossAttack;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        bossAttack = GetComponent<ActionBossAttack>();

        if (enemyHealth == null || bossAttack == null)
        {
            Debug.LogError("BossHealthExtension requires both EnemyHealth and ActionBossAttack components!");
        }
    }

    private void Start()
    {
        // Wait a frame to ensure EnemyHealth has initialized
        Invoke("InitializeHealth", 0.1f);

        // Subscribe to the damage and death events
        if (enemyHealth != null)
        {
            enemyHealth.OnDamage += OnBossDamage;
            enemyHealth.OnDeath += OnBossDeath;
        }
    }

    private void InitializeHealth()
    {
        // Initial health update with explicit debug
        if (enemyHealth != null && bossAttack != null)
        {
            float currentHealth = enemyHealth.CurrentHealth;
            float maxHealth = enemyHealth.MaxHealth;
            float percentage = enemyHealth.HealthPercentage;

            Debug.Log($"BossHealthExtension initializing boss health: {currentHealth}/{maxHealth} ({percentage:P2})");

            // Ensure health is valid before updating
            if (currentHealth <= 0 || maxHealth <= 0)
            {
                Debug.LogError($"Invalid health values detected: current={currentHealth}, max={maxHealth}");
                currentHealth = maxHealth > 0 ? maxHealth : 100f;
            }

            bossAttack.UpdateBossHealth(currentHealth, maxHealth);
        }
    }

    private void OnBossDamage(float currentHealth, float damage)
    {
        // Update the boss attack component with the new health
        bossAttack.UpdateBossHealth(currentHealth, enemyHealth.MaxHealth);
    }

    private void OnBossDeath(GameObject deadBoss)
    {
        // Additional boss death effects could go here
    }
}
