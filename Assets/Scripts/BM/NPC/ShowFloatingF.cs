using System.Collections;
using UnityEngine;
using TMPro;

public class NPCFloatingF : MonoBehaviour
{
    [Header("Floating F Settings")]
    [SerializeField] private GameObject fPrefab;  // Prefab with a TextMeshPro/TextMeshProUGUI showing "F"
    [SerializeField] private float floatDuration = 1.5f;  // How long the F floats before disappearing
    [SerializeField] private float floatDistance = 1.0f;    // How far upward the F moves
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 2f, 0); // Offset above NPC's head

    /// <summary>
    /// Call this function to show a floating "F" above the NPC's head.
    /// </summary>
    public void ShowFloatingF()
    {
        if (fPrefab == null)
        {
            Debug.LogError("fPrefab is not assigned in NPCFloatingF!");
            return;
        }

        // Calculate the spawn position above the NPC.
        Vector3 spawnPosition = transform.position + spawnOffset;
        GameObject fObject = Instantiate(fPrefab, spawnPosition, Quaternion.identity);

        // Optionally, you can parent it to the NPC so it moves with the NPC.
        // fObject.transform.SetParent(transform);

        // Start the coroutine to move and fade the "F" upward.
        StartCoroutine(FloatAndFade(fObject));
    }

    private IEnumerator FloatAndFade(GameObject fObject)
    {
        // Try to get a TextMeshPro component.
        TextMeshPro textComp = fObject.GetComponent<TextMeshPro>();
        if (textComp == null)
        {
            textComp = fObject.GetComponentInChildren<TextMeshPro>();
        }
        if (textComp == null)
        {
            Debug.LogWarning("No TextMeshPro component found on fPrefab!");
        }

        float elapsed = 0f;
        Vector3 startPos = fObject.transform.position;
        Vector3 endPos = startPos + Vector3.up * floatDistance;

        Color originalColor = textComp != null ? textComp.color : Color.white;

        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / floatDuration;
            // Lerp position upward.
            fObject.transform.position = Vector3.Lerp(startPos, endPos, t);
            // Fade out the text.
            if (textComp != null)
            {
                textComp.color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 0), t);
            }
            yield return null;
        }

        Destroy(fObject);
    }
}
