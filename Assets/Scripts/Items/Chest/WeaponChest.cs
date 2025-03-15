using System.Collections;
using UnityEngine;

public class WeaponChest : TreasureChest
{
    protected override IEnumerator OpenChest()
    {
        isOpened = true;
        if (hintText != null) hintText.gameObject.SetActive(false);

        if (items.Count > 0 && items[0].itemPrefab != null)
        {
            // Only spawn one weapon in place
            Instantiate(items[0].itemPrefab, transform.position, Quaternion.identity);
        }

        // Start the sprite change, fade out, and destroy sequence
        StartCoroutine(ChangeSpriteFadeAndDestroy());

        yield return null;
    }
}
