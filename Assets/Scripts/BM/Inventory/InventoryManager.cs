using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<Item> items = new List<Item>();
    public InventorySlot[] slots;
    public GameObject inventoryUI; // UI ของ Inventory
    public List<Item> inventoryItems = new List<Item>(); // รายการไอเทมทั้งหมด
    public Transform inventoryGrid; // Grid ที่แสดงไอเทมใน Inventory
    public GameObject inventorySlotPrefab; // Prefab ของช่องไอเทม
    
    private CraftSlot selectedCraftSlot; // ช่องคราฟที่ถูกเลือก



public void ToggleInventory()
{
    if (inventoryUI != null)
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
    else
    {
        Debug.LogError("❌ inventoryUI ไม่มีค่า! ตรวจสอบใน Inspector ว่ามีการ Assign ค่าแล้ว");
    }
}

public void ShiftItemsLeft()
{
    for (int i = 0; i < slots.Length - 1; i++)
    {
        if (slots[i].item == null)
        {
            for (int j = i + 1; j < slots.Length; j++)
            {
                if (slots[j].item != null)
                {
                    slots[i].SetItem(slots[j].item, slots[j].count);
                    slots[j].ClearSlot();
                    break;
                }
            }
        }
    }
}


    void Update()
{
    // Toggle inventory with Tab key
    if (Input.GetKeyDown(KeyCode.Tab))
    {
        ToggleInventory();
    }
    
    // Keep existing I key functionality if you want both options
    if (Input.GetKeyDown(KeyCode.I))
    {
        ToggleInventory();
    }
}


    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
    }
    else
    {
        Destroy(gameObject);
        return;
    }

    // Hide inventory UI at start
    if (inventoryUI != null)
    {
        inventoryUI.SetActive(false);
    }
    else
    {
        Debug.LogWarning("❌ inventoryUI reference is missing!");
    }

    // ✅ ค้นหา InventorySlot อัตโนมัติ
    if (slots == null || slots.Length == 0)
    {
        slots = FindObjectsByType<InventorySlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    if (slots.Length == 0)
    {
        Debug.LogError("❌ InventoryManager: ไม่พบ InventorySlot ใน Scene!");
    }
    else
    {
        Debug.Log($"✅ InventoryManager: พบ {slots.Length} ช่องใน Inventory");
    }
}

    public void UpdateInventoryUI()
{
    Debug.Log("🔄 Updating Inventory UI...");

    foreach (InventorySlot slot in slots)
    {
        if (slot.item != null && slot.count > 0)
        {
            slot.UpdateUI(); // แค่รีเฟรชหน้าตา
        }
        else
        {
            slot.ClearSlot(); // ล้างเฉพาะช่องที่ไม่มีของ
        }
    }
}



    public void RemoveItem(Item itemToRemove)
{
    if (itemToRemove == null)
    {
        Debug.LogError("❌ RemoveItem: item เป็น NULL!");
        return;
    }

    foreach (InventorySlot slot in slots)
    {
        if (slot != null && slot.item == itemToRemove)
        {
            // Reduce the count of the item
            slot.count--;

            // If count reaches 0, clear the slot
            if (slot.count <= 0)
            {
                slot.ClearSlot();
            }
            else
            {
                slot.UpdateUI();
            }

            break;
        }
    }

    UpdateInventoryUI(); // Refresh the entire inventory UI
}

   public void AddItem(Item item)
{
    if (item == null)
    {
        Debug.LogError("❌ AddItem: item เป็น NULL!");
        return;
    }

    bool itemStacked = false;

    // ตรวจสอบว่าไอเท็มสามารถสแต็กได้หรือไม่
    foreach (InventorySlot slot in slots)
    {
        if (slot != null && slot.item != null && slot.item.itemName == item.itemName && item.isStackable)
        {
            slot.count++; // เพิ่มจำนวนไอเท็ม
            slot.UpdateUI();
            Debug.Log($"🔄 สแต็กไอเท็ม '{item.itemName}' (จำนวนใหม่: {slot.count})");
            itemStacked = true;
            break;
        }
    }

    // ถ้ายังไม่มีไอเท็มนี้อยู่ หรือไม่สามารถสแต็กได้ ให้เพิ่มไอเท็มใหม่ในช่องว่าง
    if (!itemStacked)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot != null && slot.item == null) // ช่องว่าง
            {
                slot.AddItem(item, 1);
                Debug.Log($"✅ เพิ่มไอเท็มใหม่ '{item.itemName}' ลงช่อง: {slot.name}");
                break;
            }
        }
    }

    UpdateInventoryUI();
}


    public void CloseAllItemMenus()
    {
        Debug.Log("🔻 ปิดเมนูทั้งหมด");

        foreach (InventorySlot slot in slots)
        {
            if (slot != null && slot.itemMenuPanel != null)
            {
                slot.itemMenuPanel.SetActive(false);
                slot.isMenuOpen = false;
            }
        }
    }

    public void OpenInventory(CraftSlot craftSlot)
    {
        selectedCraftSlot = craftSlot; // บันทึกช่องคราฟที่ถูกเลือก
        inventoryUI.SetActive(true); // เปิด Inventory
        PopulateInventory(); // โหลดไอเทมทั้งหมดมาแสดง
    }

    void PopulateInventory()
    {
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject); // ล้างของเก่า
        }

        foreach (Item item in inventoryItems)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot != null)
            {
                inventorySlot.Setup(item, this); // ✅ ป้องกัน inventoryManager เป็น null
            }
            else
            {
                Debug.LogError("❌ ไม่สามารถกำหนดค่า InventorySlot ได้");
            }
        }
    }

    public void SelectItem(Item item)
    {
        if (selectedCraftSlot != null)
        {
            selectedCraftSlot.SetItem(item,1); // ใส่ไอเทมลง CraftSlot ที่เลือก
            inventoryUI.SetActive(false); // ปิด Inventory
        }
    }
}