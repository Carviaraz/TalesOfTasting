using UnityEngine;
using System.Collections.Generic;

public class ActionBossAttack : FSMAction
{
    [System.Serializable]
    public class AttackPattern
    {
        public string patternName;
        public GameObject projectilePrefab;
        public float cooldown = 3f;
        [Tooltip("Minimum range to use this attack (0 for no minimum)")]
        public float minRange = 0f;
        [Tooltip("Maximum range to use this attack (0 for no maximum)")]
        public float maxRange = 0f;
        [Tooltip("Health percentage threshold to activate this attack (0-1)")]
        [Range(0, 1)]
        public float healthThreshold = 1f;
        [Tooltip("Projectile firing directions (normalized vectors)")]
        public List<Vector2> directions = new List<Vector2>();
        [HideInInspector]
        public float currentCooldown = 0f;
    }

    public enum AttackType { Melee, Ranged, Special }

    [Header("Boss Config")]
    [SerializeField] private AttackType attackType = AttackType.Ranged;
    [SerializeField] private float globalCooldown = 0.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private EnemyHealth bossHealth; // Reference to EnemyHealth

    [Header("Melee Config")]
    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private float meleeDamage = 15f;
    [SerializeField] private Transform meleeHitboxPoint;

    [Header("Attack Patterns")]
    [SerializeField] private List<AttackPattern> attackPatterns = new List<AttackPattern>();

    [Header("Special Attacks")]
    [SerializeField] private float specialAttackCooldown = 15f;
    [SerializeField] private GameObject specialProjectilePrefab;
    [Tooltip("Health percentage threshold to enable special attacks (0-1)")]
    [Range(0, 1)]
    [SerializeField] private float specialAttackHealthThreshold = 0.7f;

    private EnemyAI enemy;
    private float currentGlobalCooldown = 0f;
    private float currentSpecialCooldown = 0f;
    private float currentHealthPercentage = 1f;
    private float currentHealth;
    private float maxHealth;

    private void Awake()
    {
        enemy = GetComponent<EnemyAI>();
        if (bossHealth == null)
        {
            bossHealth = GetComponent<EnemyHealth>();
        }
    }

    private void Start()
    {
        foreach (var pattern in attackPatterns)
        {
            pattern.currentCooldown = 0f;
        }

        currentSpecialCooldown = specialAttackCooldown;

        // Set default health values to ensure we don't have zeros
        currentHealth = 100f;
        maxHealth = 100f;
        currentHealthPercentage = 1.0f;

        // Set up initial health values if health component exists
        if (bossHealth != null)
        {
            // Make sure we're getting valid health values
            float health = bossHealth.CurrentHealth;
            float maxH = bossHealth.MaxHealth;

            // Only update if we got valid values
            if (health > 0 && maxH > 0)
            {
                currentHealth = health;
                maxHealth = maxH;
                currentHealthPercentage = bossHealth.HealthPercentage;
            }

            // Debug log to verify initial health values
            Debug.Log($"Boss initial health: {currentHealth}/{maxHealth} ({currentHealthPercentage:P2})");

            // Subscribe to death event
            bossHealth.OnDeath += HandleBossDeath;
        }
        else
        {
            Debug.LogWarning("No EnemyHealth component found on boss! Using default health values.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from death event
        if (bossHealth != null)
        {
            bossHealth.OnDeath -= HandleBossDeath;
        }
    }

    private void HandleBossDeath(GameObject deadBoss)
    {
        // Handle any special boss death effects or drops here
        Debug.Log("Boss has been defeated!");
    }

    // Updated health update method that takes both current and max health
    public void UpdateBossHealth(float newCurrentHealth, float newMaxHealth)
    {
        // Validate inputs to prevent division by zero or negative values
        if (newMaxHealth <= 0)
        {
            Debug.LogError($"Invalid max health value: {newMaxHealth}. Using previous value: {maxHealth}");
            newMaxHealth = maxHealth > 0 ? maxHealth : 100f;
        }

        if (newCurrentHealth < 0)
        {
            Debug.LogError($"Negative current health value: {newCurrentHealth}. Clamping to 0.");
            newCurrentHealth = 0;
        }

        this.currentHealth = newCurrentHealth;
        this.maxHealth = newMaxHealth;
        this.currentHealthPercentage = newCurrentHealth / newMaxHealth;

        // Debug log to verify health updates
        Debug.Log($"Boss health updated: {currentHealth}/{maxHealth} ({currentHealthPercentage:P2})");
    }

    public override void Act()
    {
        if (enemy.Player == null) return;

        // Update cooldowns
        currentGlobalCooldown -= Time.deltaTime;
        currentSpecialCooldown -= Time.deltaTime;
        foreach (var pattern in attackPatterns)
        {
            pattern.currentCooldown -= Time.deltaTime;
        }

        // Check if we can attack
        if (currentGlobalCooldown <= 0f)
        {
            // Determine player distance
            float distanceToPlayer = Vector2.Distance(transform.position, enemy.Player.position);

            // Debug current state for troubleshooting
            if (Random.value < 0.01f) // Only log occasionally to avoid spamming
            {
                Debug.Log($"Boss health: {currentHealthPercentage:P2}, Special threshold: {specialAttackHealthThreshold:P2}");
                Debug.Log($"Special cooldown: {currentSpecialCooldown}/{specialAttackCooldown}");
            }

            // Try special attack first
            if (currentHealthPercentage <= specialAttackHealthThreshold &&
                currentSpecialCooldown <= 0f &&
                specialProjectilePrefab != null)
            {
                Debug.Log($"Boss health {currentHealthPercentage:P2} below threshold {specialAttackHealthThreshold:P2} - Performing special attack!");
                PerformSpecialAttack();
                currentGlobalCooldown = globalCooldown;
                currentSpecialCooldown = specialAttackCooldown;
                return;
            }

            // Choose attack based on distance and health conditions
            AttackPattern selectedPattern = ChooseAttackPattern(distanceToPlayer);

            if (selectedPattern != null)
            {
                Debug.Log($"Selected attack pattern: {selectedPattern.patternName} (Health: {currentHealthPercentage:P2}, Threshold: {selectedPattern.healthThreshold:P2})");
                switch (attackType)
                {
                    case AttackType.Melee:
                        if (distanceToPlayer <= meleeAttackRange)
                        {
                            MeleeAttack();
                        }
                        break;
                    case AttackType.Ranged:
                    case AttackType.Special:
                        RangedAttack(selectedPattern);
                        break;
                }

                selectedPattern.currentCooldown = selectedPattern.cooldown;
                currentGlobalCooldown = globalCooldown;
            }
            else if (attackType == AttackType.Melee && distanceToPlayer <= meleeAttackRange)
            {
                // Fallback to melee if in range and no pattern selected
                MeleeAttack();
                currentGlobalCooldown = globalCooldown;
            }
        }
    }

    private AttackPattern ChooseAttackPattern(float distanceToPlayer)
    {
        List<AttackPattern> eligiblePatterns = new List<AttackPattern>();

        // Find all eligible patterns based on range and health threshold
        foreach (var pattern in attackPatterns)
        {
            bool inRange = (pattern.minRange == 0 || distanceToPlayer >= pattern.minRange) &&
                          (pattern.maxRange == 0 || distanceToPlayer <= pattern.maxRange);

            // The key fix: health threshold check
            // Pattern is eligible if current health percentage is BELOW threshold
            bool healthCondition = currentHealthPercentage <= pattern.healthThreshold;

            if (inRange && healthCondition && pattern.currentCooldown <= 0f)
            {
                eligiblePatterns.Add(pattern);
                Debug.Log($"Pattern {pattern.patternName} eligible: Health {currentHealthPercentage:P2} <= {pattern.healthThreshold:P2}");
            }
            else if (!healthCondition)
            {
                Debug.Log($"Pattern {pattern.patternName} not eligible: Health {currentHealthPercentage:P2} > {pattern.healthThreshold:P2}");
            }
        }

        // Return random eligible pattern or null if none
        if (eligiblePatterns.Count > 0)
        {
            return eligiblePatterns[Random.Range(0, eligiblePatterns.Count)];
        }

        return null;
    }

    private void MeleeAttack()
    {
        Debug.Log("Boss Melee Attack!");

        // Detect all objects in hitbox range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeHitboxPoint.position, meleeAttackRange, playerLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            ITakeDamage damageable = enemy.GetComponent<ITakeDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(meleeDamage);
                Debug.Log("Hit " + enemy.name);
            }
        }
    }

    private void RangedAttack(AttackPattern pattern)
    {
        if (pattern.projectilePrefab == null || attackPoint == null) return;

        // If no directions specified, just shoot at player
        if (pattern.directions == null || pattern.directions.Count == 0)
        {
            FireProjectile(pattern.projectilePrefab, (enemy.Player.position - attackPoint.position).normalized);
        }
        else
        {
            // Fire in all specified directions
            foreach (Vector2 dir in pattern.directions)
            {
                FireProjectile(pattern.projectilePrefab, dir.normalized);
            }
        }
    }

    private void PerformSpecialAttack()
    {
        Debug.Log("Boss Special Attack!");

        // Fire projectiles in a circular pattern
        int projectileCount = 12;
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * (360f / projectileCount);
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            FireProjectile(specialProjectilePrefab, direction);
        }

        // Add a homing projectile aimed at player
        FireProjectile(specialProjectilePrefab, (enemy.Player.position - attackPoint.position).normalized);
    }

    private void FireProjectile(GameObject projectilePrefab, Vector2 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript)
        {
            projectileScript.Direction = direction;
            projectileScript.Damage = 2f; // Boss projectiles do more damage

            // Set rotation to match direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // Draw Gizmos to visualize attack ranges in Unity Editor
    private void OnDrawGizmosSelected()
    {
        // Draw melee range
        if (meleeHitboxPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleeHitboxPoint.position, meleeAttackRange);
        }

        // Draw attack pattern ranges
        if (attackPoint != null)
        {
            foreach (var pattern in attackPatterns)
            {
                if (pattern.minRange > 0)
                {
                    Gizmos.color = new Color(1f, 0.6f, 0f, 0.3f); // Orange
                    Gizmos.DrawWireSphere(transform.position, pattern.minRange);
                }

                if (pattern.maxRange > 0)
                {
                    Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // Yellow
                    Gizmos.DrawWireSphere(transform.position, pattern.maxRange);
                }
            }
        }
    }
}