using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int itemID;  // เพิ่มตัวแปร ID
    public string itemName;  // ชื่อไอเท็ม
    public Sprite icon;  // รูปภาพไอเท็ม
    public bool isStackable;  // ไอเท็มสามารถเก็บซ้อนกันได้หรือไม่

    public virtual void Use()
    {
        Debug.Log($"🎮 กำลังใช้ไอเท็ม {itemName}");
        // ✅ เพิ่มโค้ดเฉพาะไอเท็ม เช่น เพิ่ม HP, เพิ่มพลัง ฯลฯ
    }

    public Item CloneOne()
{
    Item newItem = ScriptableObject.CreateInstance<Item>();
    newItem.itemID = this.itemID;
    newItem.itemName = this.itemName;
    newItem.icon = this.icon;
    newItem.isStackable = this.isStackable;
    return newItem;
}



public bool IsSameItem(Item other)
{
    return other != null && itemID == other.itemID;
}


}
