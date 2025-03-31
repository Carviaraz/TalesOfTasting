using UnityEngine;

public class CraftingStation : MonoBehaviour
{
    public GameObject craftCanvas; // ลาก UI คราฟจาก Inspector
    private bool playerNearby = false;
    private InventoryManager inventoryManager;

    private void Start()
    {
        // Get reference to the inventory manager
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("❌ InventoryManager not found in scene!");
        }
        
        if (craftCanvas != null)
            craftCanvas.SetActive(false); // ปิด UI ตอนเริ่มเกม
        else
            Debug.LogWarning("⚠ craftCanvas ไม่ได้ถูกตั้งค่าใน Inspector!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่าเป็น Player
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            CloseCraftingUI(); // ใช้ฟังก์ชันปิด UI แทน
        }
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.F)) // กด F เพื่อเปิด/ปิดคราฟ
        {
            ToggleCraftingUI();
        }
    }

    void ToggleCraftingUI()
    {
        if (craftCanvas != null)
        {
            bool newState = !craftCanvas.activeSelf;
            craftCanvas.SetActive(newState);
            
            // เปิด/ปิด Inventory พร้อมกับ Crafting UI
            if (inventoryManager != null && inventoryManager.inventoryUI != null)
            {
                inventoryManager.inventoryUI.SetActive(newState);
            }
        }
    }
    
    void CloseCraftingUI()
    {
        if (craftCanvas != null)
        {
            craftCanvas.SetActive(false);
            
            // ปิด Inventory เมื่อปิด Crafting UI
            if (inventoryManager != null && inventoryManager.inventoryUI != null)
            {
                inventoryManager.inventoryUI.SetActive(false);
            }
        }
    }
}