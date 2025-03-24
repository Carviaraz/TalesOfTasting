using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum AttackPattern { Spin, Wave, Straight, Boomerang, Homing, Burst, Spiral }

    [Header("Config")]
    [SerializeField] private float enemyProjectileSpeed;
    [SerializeField] private AttackPattern attackPattern;
    [SerializeField] private LayerMask targetLayer;

    [Header("Spin Parameters")]
    [SerializeField] private float spinSpeed = -720f;

    [Header("Wave Parameters")]
    [SerializeField] private float waveAmplitude = 0.5f;
    [SerializeField] private float waveFrequency = 2f;

    [Header("Boomerang Parameters")]
    [SerializeField] private float returnDelay = 1f;
    [SerializeField] private float returnSpeed = 2f;

    [Header("Homing Parameters")]
    [SerializeField] private float homingStrength = 2f;
    [SerializeField] private float homingDelay = 0.5f;
    [SerializeField] private string targetTag = "Player";

    [Header("Burst Parameters")]
    [SerializeField] private float burstRadius = 0.5f;
    [SerializeField] private float burstTime = 2f;

    [Header("Spiral Parameters")]
    [SerializeField] private float spiralRadius = 1f;
    [SerializeField] private float spiralSpeed = 3f;

    public Vector3 Direction { get; set; }
    public float Damage { get; set; }
    public float Speed { get; set; }

    // Common variables
    private Vector3 startPosition;
    private float timeSinceSpawn;
    private Vector3 perpendicular;
    private Transform target;
    private Vector3 initialDirection;
    private bool isReturning = false;

    void Start()
    {
        if (Speed <= 0) { Speed = enemyProjectileSpeed; }
        startPosition = transform.position;
        initialDirection = Direction;

        // Calculate perpendicular vector to direction for wave pattern
        perpendicular = new Vector3(-Direction.y, Direction.x, 0).normalized;
        timeSinceSpawn = 0f;

        // Find target for homing projectiles
        if (attackPattern == AttackPattern.Homing)
        {
            GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
            if (targetObject != null)
            {
                target = targetObject.transform;
            }
        }
    }

    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        switch (attackPattern)
        {
            case AttackPattern.Spin:
                // Move forward
                transform.position += Direction * (Speed * Time.deltaTime);
                // Spin the projectile
                transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
                break;

            case AttackPattern.Wave:
                // Calculate wave offset perpendicular to direction
                float waveOffset = Mathf.Sin(timeSinceSpawn * waveFrequency) * waveAmplitude;
                Vector3 waveVector = perpendicular * waveOffset;

                // Move forward along direction with wave offset
                Vector3 movement = Direction * (Speed * Time.deltaTime);
                transform.position += movement;
                transform.position += perpendicular * waveOffset * Mathf.Cos(timeSinceSpawn * waveFrequency) * Speed * Time.deltaTime;

                // Rotate projectile to face movement direction
                float angle = Mathf.Atan2(Direction.y + perpendicular.y * Mathf.Cos(timeSinceSpawn * waveFrequency) * waveAmplitude * waveFrequency,
                                         Direction.x + perpendicular.x * Mathf.Cos(timeSinceSpawn * waveFrequency) * waveAmplitude * waveFrequency) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                break;

            case AttackPattern.Straight:
                // Standard straight movement
                transform.position += Direction * (Speed * Time.deltaTime);
                break;

            case AttackPattern.Boomerang:
                if (!isReturning && timeSinceSpawn > returnDelay)
                {
                    isReturning = true;
                }

                if (!isReturning)
                {
                    // Move forward
                    transform.position += Direction * (Speed * Time.deltaTime);
                }
                else
                {
                    // Return to starting position
                    Vector3 returnDirection = (startPosition - transform.position).normalized;
                    transform.position += returnDirection * (Speed * returnSpeed * Time.deltaTime);

                    // Rotate to face return direction
                    float returnAngle = Mathf.Atan2(returnDirection.y, returnDirection.x) * Mathf.Rad2Deg - 90f;
                    transform.rotation = Quaternion.Euler(0, 0, returnAngle);

                    // Check if returned to starting position
                    if (Vector3.Distance(transform.position, startPosition) < 0.1f)
                    {
                        Destroy(gameObject);
                    }
                }

                // Spin while moving
                transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
                break;

            case AttackPattern.Homing:
                if (target != null && timeSinceSpawn > homingDelay)
                {
                    // Calculate direction to target
                    Vector3 directionToTarget = (target.position - transform.position).normalized;

                    // Gradually adjust direction towards target
                    Direction = Vector3.Lerp(Direction, directionToTarget, homingStrength * Time.deltaTime);
                    //Direction.z = 0;
                    Direction = Direction.normalized;

                    // Rotate to face direction
                    float homingAngle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg - 90f;
                    transform.rotation = Quaternion.Euler(0, 0, homingAngle);
                }

                // Move in current direction
                transform.position += Direction * (Speed * Time.deltaTime);
                break;

            case AttackPattern.Burst:
                if (timeSinceSpawn < burstTime)
                {
                    // Move forward normally
                    transform.position += Direction * (Speed * Time.deltaTime);

                    // Gradually increase scale to indicate charging
                    float scale = 1f + (timeSinceSpawn / burstTime) * 0.5f;
                    transform.localScale = new Vector3(scale, scale, scale);
                }
                else
                {
                    // Burst effect
                    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, burstRadius, targetLayer);
                    foreach (Collider2D hit in hitColliders)
                    {
                        ITakeDamage damageable = hit.GetComponent<ITakeDamage>();
                        if (damageable != null)
                        {
                            damageable.TakeDamage(Damage);
                            Debug.Log("Burst hit " + hit.name + " for " + Damage + " Damage");
                        }
                    }

                    // Optional: Spawn burst effect
                    // Instantiate(burstEffect, transform.position, Quaternion.identity);

                    Destroy(gameObject);
                }
                break;

            case AttackPattern.Spiral:
                // Calculate spiral movement
                float spiralAngle = timeSinceSpawn * spiralSpeed;
                Vector3 spiralOffset = new Vector3(
                    Mathf.Cos(spiralAngle) * spiralRadius,
                    Mathf.Sin(spiralAngle) * spiralRadius,
                    0
                );

                // Move forward along direction with spiral offset
                transform.position += Direction * (Speed * Time.deltaTime);
                transform.position = transform.position + spiralOffset - perpendicular * Mathf.Sin(spiralAngle - Time.deltaTime * spiralSpeed) * spiralRadius - perpendicular * Mathf.Cos(spiralAngle - Time.deltaTime * spiralSpeed) * spiralRadius;

                // Rotate projectile to face movement direction
                float spiralRotation = spiralAngle * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, spiralRotation);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            // For boomerang pattern, don't destroy on hit during return phase
            if (attackPattern == AttackPattern.Boomerang && isReturning)
            {
                return;
            }

            ITakeDamage damageable = collision.GetComponent<ITakeDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(Damage);
            }

            Destroy(gameObject);
            Debug.Log("Projectile hit " + collision.name + " for " + Damage + " Damage");
        }
    }

    // For debugging and visualization
    private void OnDrawGizmos()
    {
        if (attackPattern == AttackPattern.Burst)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, burstRadius);
        }
    }
}
