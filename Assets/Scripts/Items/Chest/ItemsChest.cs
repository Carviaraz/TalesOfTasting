using System.Collections;
using UnityEngine;

public class ItemsChest : TreasureChest
{
    [SerializeField] private float explosionRadius = 1.5f;
    [SerializeField] private float explosionForce = 2.0f;
    [SerializeField] private GameObject coinPrefab; // Ensure a reference to the coin prefab

    protected override IEnumerator OpenChest()
    {
        isOpened = true;
        if (hintText != null) hintText.gameObject.SetActive(false);

        // Ensure coins always drop
        SpawnCoins();

        // Drop other random items (excluding coins)
        foreach (ChestItem chestItem in items)
        {
            if (chestItem.itemPrefab == coinPrefab) continue; // Skip coins as they are handled separately

            int itemCount = chestItem.GetRandomAmount();
            for (int i = 0; i < itemCount; i++)
            {
                if (Random.value > 0.5f) // 50% chance to spawn each item
                {
                    SpawnItem(chestItem.itemPrefab);
                }
            }
        }

        // Start the sprite change, fade out, and destroy sequence
        StartCoroutine(ChangeSpriteFadeAndDestroy());

        yield return null;
    }

    private void SpawnCoins()
    {
        if (coinPrefab == null) return;

        int coinAmount = Random.Range(2, 5); // Adjust range as needed
        for (int i = 0; i < coinAmount; i++)
        {
            SpawnItem(coinPrefab);
        }
    }

    private void SpawnItem(GameObject itemPrefab)
    {
        if (itemPrefab == null) return;

        GameObject item = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = false;
            rb.drag = 2.0f;
            rb.angularDrag = 2.0f;

            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(explosionRadius * 0.5f, explosionRadius);

            rb.AddForce(randomDirection * explosionForce, ForceMode2D.Impulse);
        }
        else
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(explosionRadius * 0.5f, explosionRadius);
            item.transform.position += (Vector3)(randomDirection * randomDistance);
        }
    }
}
