using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite icon;
    public bool isStackable;

    public float healthRestore;
    public float energyRestore;

    public virtual void Use()
    {
        PlayerHealth playerHealth = GameObject.FindObjectOfType<PlayerHealth>();
        PlayerEnergy playerEnergy = GameObject.FindObjectOfType<PlayerEnergy>();

        if (playerHealth != null && healthRestore > 0)
        {
            playerHealth.RecoverHealth(healthRestore);
            Debug.Log($"❤️ Restored {healthRestore} HP!");
        }

        if (playerEnergy != null && energyRestore > 0)
        {
            playerEnergy.RecoverEnergy(energyRestore);
            Debug.Log($"⚡ Restored {energyRestore} Energy!");
        }

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
