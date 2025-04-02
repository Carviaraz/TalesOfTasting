using UnityEngine;

public class CraftingStation : MonoBehaviour
{
    private static GameObject craftCanvas;  // Shared among all campfires
    private static InventoryManager inventoryManager;

    private bool playerNearby = false; // Tracks if player is near this campfire
    private static bool isCraftingUIOpen = false; // Tracks if UI is open
    private static Transform currentCampfire; // Stores the campfire currently using UI

    private void Start()
    {
        // Only find the canvas once (for performance)
        if (craftCanvas == null)
        {
            craftCanvas = GameObject.Find("CraftCanvas");
            if (craftCanvas != null)
                craftCanvas.SetActive(false); // Start hidden
            else
                Debug.LogWarning("⚠ CraftCanvas is missing from the scene!");
        }

        // Only find InventoryManager once
        if (inventoryManager == null)
        {
            inventoryManager = FindAnyObjectByType<InventoryManager>();
            if (inventoryManager == null)
                Debug.LogError("❌ InventoryManager not found in scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (currentCampfire == transform) // Only close if this campfire opened it
            {
                CloseCraftingUI();
            }
        }
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.F))
        {
            ToggleCraftingUI();
        }
    }

    private void ToggleCraftingUI()
    {
        if (craftCanvas != null)
        {
            if (!isCraftingUIOpen) // Open the UI
            {
                // Move canvas to this campfire
                craftCanvas.transform.position = transform.position + new Vector3(0, 1.5f, 0); // Adjust position as needed
                craftCanvas.SetActive(true);
                isCraftingUIOpen = true;
                currentCampfire = transform; // Remember which campfire is using the UI

                // Open inventory UI
                if (inventoryManager != null && inventoryManager.inventoryUI != null)
                {
                    inventoryManager.inventoryUI.SetActive(true);
                }
            }
            else if (currentCampfire == transform) // Close only if this campfire opened it
            {
                CloseCraftingUI();
            }
        }
    }

    private void CloseCraftingUI()
    {
        if (craftCanvas != null)
        {
            craftCanvas.SetActive(false);
            isCraftingUIOpen = false;
            currentCampfire = null;

            // Close inventory UI
            if (inventoryManager != null && inventoryManager.inventoryUI != null)
            {
                inventoryManager.inventoryUI.SetActive(false);
            }
        }
    }
}