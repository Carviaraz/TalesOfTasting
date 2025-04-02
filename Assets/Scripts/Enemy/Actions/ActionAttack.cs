using UnityEngine;

public class ActionAttack : FSMAction
{
    public enum AttackType { Melee, Ranged } // Attack types
    [Header("Config")]
    [SerializeField] private AttackType attackType;
    [SerializeField] private float timeBtwAttack = 1f;

    [Header("Melee Config")]
    [SerializeField] private float meleeAttackRange = 1.5f;
    [SerializeField] private float meleeDamage = 10f;
    [SerializeField] private Transform meleeHitboxPoint; // Position for melee hitbox
    [SerializeField] private LayerMask playerLayer; // Define what the enemy can hit

    [Header("Ranged Config")]
    [SerializeField] private GameObject projectilePrefab; // Projectile Prefab
    [SerializeField] private Transform attackPoint; // Position to spawn projectile

    private EnemyAI enemy;
    private float attackTimer;

    private void Awake()
    {
        enemy = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        attackTimer = timeBtwAttack;
    }

    public override void Act()
    {
        if (enemy.Player == null) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (attackType == AttackType.Melee)
                MeleeAttack();
            else
                RangedAttack();

            attackTimer = timeBtwAttack;
        }
    }

    private void MeleeAttack()
    {
        Debug.Log("Melee Attack!");

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

    private void RangedAttack()
    {
        if (projectilePrefab && attackPoint)
        {
            GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript)
            {
                Vector3 direction = (enemy.Player.position - attackPoint.position).normalized;
                projectileScript.Direction = direction;
                projectileScript.Damage = 1f; // Set damage if needed
            }
        }
    }

    // Draw Gizmos to visualize melee hitbox in Unity Editor
    private void OnDrawGizmosSelected()
    {
        if (meleeHitboxPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleeHitboxPoint.position, meleeAttackRange);
        }
    }
}
