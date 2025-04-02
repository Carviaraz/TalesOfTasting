using TMPro;
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
            Debug.Log("BossHealthExtension requires both EnemyHealth and ActionBossAttack components!");
        }
    }

    private void Start()
    {
        Invoke("InitializeHealth", 0.1f);

        if (enemyHealth != null)
        {
            enemyHealth.OnDamage += OnBossDamage;
            enemyHealth.OnDeath += OnBossDeath;
        }
    }

    private void InitializeHealth()
    {
        if (enemyHealth != null && bossAttack != null)
        {
            float currentHealth = enemyHealth.CurrentHealth;
            float maxHealth = enemyHealth.MaxHealth;
            bossAttack.UpdateBossHealth(currentHealth, maxHealth);
        }
    }

    private void OnBossDamage(float currentHealth, float damage)
    {
        bossAttack.UpdateBossHealth(currentHealth, enemyHealth.MaxHealth);
    }

    private void OnBossDeath(GameObject deadBoss)
    {
        DungeonTimer.Instance.StopTimer();
        PlayerEnergy.Instance.StopEnergyDrain();

        // Disable player controls on victory
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.DisablePlayerControls();
        }

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOverScreen(true);
        }

    }
}
