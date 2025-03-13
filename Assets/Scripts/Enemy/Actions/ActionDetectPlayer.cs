using UnityEngine;

public class ActionDetectPlayer : FSMAction
{
    [Header("Config")]
    [SerializeField] private float rangeDetection;
    [SerializeField] private LayerMask playerMask;

    private EnemyAI enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyAI>();
    }

    public override void Act()
    {
        Collider2D collider2D = Physics2D.OverlapCircle(transform.position, rangeDetection, playerMask);
        
        if (collider2D == null)
        {
            enemy.Player = null;
            return;
        }

        enemy.Player = collider2D.transform;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangeDetection);
    }
}
