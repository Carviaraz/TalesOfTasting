using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestItem
{
    public GameObject itemPrefab;
    public int minAmount = 1;
    public int maxAmount = 3;

    public int GetRandomAmount()
    {
        return Random.Range(minAmount, maxAmount + 1);
    }
}

public abstract class TreasureChest : MonoBehaviour
{
    public List<ChestItem> items; // Items stored in the chest
    public SpriteRenderer hintText;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Sprite closedSprite;
    [SerializeField] protected Sprite openedSprite;
    [SerializeField] protected float fadeDelay = 1.5f;
    [SerializeField] protected float fadeDuration = 0.5f;

    protected bool isOpened = false;
    protected bool playerNearby = false;

    private void Start()
    {
        if (hintText != null) hintText.gameObject.SetActive(false);
        if (spriteRenderer != null) spriteRenderer.sprite = closedSprite;
    }

    private void Update()
    {
        if (playerNearby && !isOpened && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(OpenChest());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintText != null)
        {
            hintText.gameObject.SetActive(true);
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintText != null)
        {
            hintText.gameObject.SetActive(false);
            playerNearby = false;
        }
    }

    // Abstract method to be implemented by child classes
    protected abstract IEnumerator OpenChest();

    // Method to handle sprite change, fade and destroy
    protected IEnumerator ChangeSpriteFadeAndDestroy()
    {
        // Change sprite to opened
        if (spriteRenderer != null && openedSprite != null)
        {
            spriteRenderer.sprite = openedSprite;
        }

        // Wait before starting fade
        yield return new WaitForSeconds(fadeDelay);

        // Fade out
        if (spriteRenderer != null)
        {
            float elapsedTime = 0f;
            Color originalColor = spriteRenderer.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }

        // Destroy the chest
        Destroy(gameObject);
    }
}
