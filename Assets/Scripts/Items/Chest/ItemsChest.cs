using System.Collections;
using UnityEngine;

public class ItemsChest : TreasureChest
{
    [SerializeField] private float explosionRadius = 1.5f;
    [SerializeField] private float explosionForce = 2.0f;

    protected override IEnumerator OpenChest()
    {
        isOpened = true;
        if (hintText != null) hintText.gameObject.SetActive(false);

        foreach (ChestItem chestItem in items)
        {
            int itemCount = chestItem.GetRandomAmount();
            for (int i = 0; i < itemCount; i++)
            {
                SpawnItem(chestItem.itemPrefab);
            }
        }

        // Start the sprite change, fade out, and destroy sequence
        StartCoroutine(ChangeSpriteFadeAndDestroy());

        yield return null;
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
            Vector2 targetPosition = (Vector2)transform.position + (randomDirection * randomDistance);

            rb.AddForce(randomDirection * explosionForce, ForceMode2D.Impulse);
        }
        else
        {
            // If there's no Rigidbody2D, just move the item manually
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(explosionRadius * 0.5f, explosionRadius);
            item.transform.position += (Vector3)(randomDirection * randomDistance);
        }
    }
}
