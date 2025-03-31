using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // กำหนดไอเท็มที่เก็บได้

    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log($"🛑 ตรวจพบ Player ชนกับ {gameObject.name}");
        
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("❌ InventoryManager.Instance เป็น NULL! ตรวจสอบว่า InventoryManager อยู่ใน Scene หรือไม่");
            return;
        }

        if (item == null)
        {
            Debug.LogError("❌ item เป็น NULL! ตรวจสอบว่า ItemPickup มีไอเท็มกำหนดค่าไว้หรือไม่");
            return;
        }

        InventoryManager.Instance.AddItem(item);
        Destroy(gameObject);
    }
}



}
