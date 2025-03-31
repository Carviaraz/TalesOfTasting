using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemMenuController : MonoBehaviour {
    public static ItemMenuController Instance;  // ✅ ใช้ Singleton เพื่อให้เข้าถึงง่าย
    public Item selectedItem;  // ✅ เก็บข้อมูลไอเท็มที่ถูกเลือก
    public GameObject menu; // ✅ อ้างอิงเมนูที่ต้องแสดง

    private void Awake() {
        Instance = this;
        menu.SetActive(false); // ✅ ซ่อนเมนูเมื่อเริ่มเกม
    }

    public void OpenMenu(Item item) {
        selectedItem = item; // ✅ ตั้งค่าไอเท็มที่ถูกเลือก
        menu.SetActive(true);
        Debug.Log($"Menu opened for item: {selectedItem.itemName}");
    }

    public void EatItem() {   
        if (selectedItem != null) {
            Debug.Log("Eating: " + selectedItem.itemName);
            InventoryManager.Instance.RemoveItem(selectedItem); // ✅ ลบไอเท็มจาก Inventory
            CloseMenu(); // ✅ ปิดเมนูหลังจากใช้ไอเท็ม
        } else {
            Debug.LogError("❌ No item selected!");
        }
    }

    public void DropItem() {
        if (selectedItem != null) {
            Debug.Log("Dropping: " + selectedItem.itemName);
            InventoryManager.Instance.RemoveItem(selectedItem);
            CloseMenu();
        } else {
            Debug.LogError("❌ No item selected!");
        }
    }

    void CloseMenu() {
        menu.SetActive(false);
        Debug.Log("Menu Closed");
    }
}
