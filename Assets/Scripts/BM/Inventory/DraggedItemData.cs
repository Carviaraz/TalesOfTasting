using UnityEngine;

public class DraggedItemData : MonoBehaviour
{
    public Item draggedItem;
    public InventorySlot originSlot;
    // Flag to indicate that the drop was successfully handled
    public static bool dropSuccessful = false;
    // Fallback static storage for dragged item (if needed)
    public static Item currentDraggedItem;
}
