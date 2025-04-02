using UnityEngine;

public class CraftingStation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject craftCanvasPrefab; // Reference to canvas prefab instead of finding by name

    // Instance references
    private GameObject craftCanvasInstance;
    private InventoryManager inventoryManager;
    private CraftingSystem craftingSystem;

    // State tracking
    private bool playerNearby = false;
    private bool isCraftingUIOpen = false;

    private void Awake()
    {
        // Find inventory manager reference
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        if (inventoryManager == null)
            Debug.LogError("❌ InventoryManager not found in scene!");

        // Instantiate craft canvas if it doesn't exist
        if (craftCanvasPrefab != null)
        {
            craftCanvasInstance = Instantiate(craftCanvasPrefab);
            craftCanvasInstance.SetActive(false); // Start hidden

            // Cache crafting system reference
            craftingSystem = craftCanvasInstance.GetComponentInChildren<CraftingSystem>();
            if (craftingSystem == null)
                Debug.LogWarning("⚠ CraftingSystem component not found in CraftCanvas!");
        }
        else
        {
            Debug.LogError("❌ CraftCanvasPrefab is not assigned!");
        }
    }

    private void OnEnable()
    {
        // Ensure UI elements are correctly initialized when enabled
        if (craftCanvasInstance != null)
        {
            // Make sure all UI handlers are properly registered
            craftingSystem?.SetupCookButton();
        }
    }

    private void OnDisable()
    {
        // Close UI when disabled
        CloseCraftingUI();
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
            CloseCraftingUI();
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
        if (craftCanvasInstance != null)
        {
            if (!isCraftingUIOpen) // Open the UI
            {
                // Position canvas near this crafting station
                craftCanvasInstance.transform.position = transform.position + new Vector3(0, 1.5f, 0);
                craftCanvasInstance.SetActive(true);
                isCraftingUIOpen = true;

                // Force reinitialize crafting system
                if (craftingSystem != null)
                {
                    craftingSystem.ResetCraftingSystem();
                }

                // Open inventory UI
                if (inventoryManager != null && inventoryManager.inventoryUI != null)
                {
                    inventoryManager.inventoryUI.SetActive(true);
                }
            }
            else
            {
                CloseCraftingUI();
            }
        }
    }

    private void CloseCraftingUI()
    {
        if (craftCanvasInstance != null && isCraftingUIOpen)
        {
            craftCanvasInstance.SetActive(false);
            isCraftingUIOpen = false;

            // Close inventory UI
            if (inventoryManager != null && inventoryManager.inventoryUI != null)
            {
                inventoryManager.inventoryUI.SetActive(false);
            }
        }
    }

    // Clean up on destroy
    private void OnDestroy()
    {
        if (craftCanvasInstance != null)
        {
            Destroy(craftCanvasInstance);
        }
    }
}