using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public float attractRange = 3f;
    public float attractionSpeed = 2f;
    public float maxSpeedMultiplier = 4f;
    public float magnetDelay = 1.0f; // Time before magnet activates (for chest explosion)

    private Transform player;
    private bool isAttracted = false;
    private bool isCollected = false;
    private Collider2D coinCollider;
    private AudioSource audioSource;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        coinCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        // Disable magnet & collider initially
        coinCollider.enabled = false;
        isAttracted = false;

        // Enable them after delay
        Invoke(nameof(EnableMagnet), magnetDelay);
    }

    private void Update()
    {
        if (player == null || isCollected) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (isAttracted && distance < attractRange)
        {
            float speedMultiplier = Mathf.Lerp(1f, maxSpeedMultiplier, 1 - (distance / attractRange));
            transform.position = Vector2.Lerp(transform.position, player.position, attractionSpeed * speedMultiplier * Time.deltaTime);
        }
    }

    private void EnableMagnet()
    {
        coinCollider.enabled = true;
        isAttracted = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected || !other.CompareTag("Player")) return;

        isCollected = true;
        GameManager.Instance.AddCoins(coinValue);
        Debug.Log("Coin = "+ GameManager.Instance.coins);

        if (audioSource != null && audioSource.clip != null)
        {
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
        }

        Destroy(gameObject);
    }
}
