using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, ITakeDamage
{
    [SerializeField] private float maxHealth = 100f;
    private SpriteRenderer spriteRenderer;
    private float currentHealth;
    private Color initialColor;
    private Coroutine colorCoroutine;

    // Delegate for damage events
    public delegate void DamageEventHandler(float currentHealth, float damage);
    // Delegate for death events
    public delegate void DeathEventHandler(GameObject deadEnemy);

    // Events that other components can subscribe to
    public event DamageEventHandler OnDamage;
    public event DeathEventHandler OnDeath;

    // Public getters for health information
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercentage => (maxHealth > 0) ? (currentHealth / maxHealth) : 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Initialize health in Awake to ensure it's set before other components access it
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Make sure health is set in Start as well (redundant safety)
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }

        initialColor = spriteRenderer.color;

        // Debug log to verify health is initialized correctly
        Debug.Log($"EnemyHealth initialized: {currentHealth}/{maxHealth} ({HealthPercentage:P2})");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy health = {currentHealth}/{maxHealth} ({HealthPercentage:P2})");

        // Trigger the damage event so other components know health has changed
        OnDamage?.Invoke(currentHealth, damage);

        ShowDamageColor();

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }

    private void ShowDamageColor()
    {
        if (colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }
        colorCoroutine = StartCoroutine(IETakeDamage());
    }

    private IEnumerator IETakeDamage()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(.1f);
        spriteRenderer.color = initialColor;
    }
}
