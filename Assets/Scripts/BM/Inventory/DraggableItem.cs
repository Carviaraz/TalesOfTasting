using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Component parentSlot; // Should be InventorySlot or ResultSlot

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image itemImage;
    private Vector2 originalPosition;
    private Transform originalParent;
    private Canvas mainCanvas;

    private GameObject dragVisual;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        itemImage = GetComponent<Image>();
        mainCanvas = FindFirstObjectByType<Canvas>();

        // Get the parent slot (either InventorySlot or ResultSlot)
        parentSlot = GetComponentInParent<InventorySlot>() as Component;
        if (parentSlot == null)
            parentSlot = GetComponentInParent<ResultSlot>() as Component;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("🟢 Begin Drag: " + gameObject.name);
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // Clear previous drop flag
        DraggedItemData.dropSuccessful = false;

        if (parentSlot is InventorySlot inventorySlot)
        {
            if (inventorySlot.item == null || inventorySlot.count <= 0)
            {
                Debug.LogWarning("❌ No item in slot, cannot drag");
                return;
            }

            // Clone one item from the stack.
            Item singleItem = inventorySlot.item.CloneOne();

            // Decrement inventory count.
            inventorySlot.count--;
            if (inventorySlot.count <= 0)
                inventorySlot.ClearSlot();
            else
                inventorySlot.UpdateUI();

            // Create drag visual.
            dragVisual = new GameObject("DragVisual");
            dragVisual.transform.SetParent(mainCanvas.transform);
            RectTransform dragRectTransform = dragVisual.AddComponent<RectTransform>();
            Image dragImage = dragVisual.AddComponent<Image>();
            dragRectTransform.sizeDelta = new Vector2(50, 50);
            dragRectTransform.position = Input.mousePosition;
            dragImage.color = new Color(1, 1, 1, 0.7f);
            dragImage.sprite = singleItem.icon;

            // Attach dragged item data.
            DraggedItemData data = dragVisual.AddComponent<DraggedItemData>();
            data.draggedItem = singleItem;
            data.originSlot = inventorySlot;
            DraggedItemData.currentDraggedItem = singleItem;
        }
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragVisual != null)
            dragVisual.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("🔴 End Drag: " + gameObject.name);

        // Cache dragged item data before destroying visual.
        DraggedItemData data = dragVisual != null ? dragVisual.GetComponent<DraggedItemData>() : null;
        Item draggedItem = data != null ? data.draggedItem : DraggedItemData.currentDraggedItem;
        InventorySlot originSlot = data != null ? data.originSlot : null;

        // Delay destruction of dragVisual by 0.1 seconds.
        if (dragVisual != null)
            Destroy(dragVisual, 0.1f);
        DraggedItemData.currentDraggedItem = null;

        // Always re-enable raycasts.
        canvasGroup.blocksRaycasts = true;

        // If drop was successful, skip restoration.
        if (DraggedItemData.dropSuccessful)
        {
            Debug.Log("Drop successful; not restoring item to origin.");
            return;
        }

        GameObject dropTarget = eventData.pointerEnter;
        bool droppedOnValidTarget = dropTarget != null &&
            (dropTarget.GetComponent<CraftSlot>() != null ||
             dropTarget.GetComponentInParent<CraftSlot>() != null ||
             dropTarget.GetComponent<InventorySlot>() != null ||
             dropTarget.GetComponentInParent<InventorySlot>() != null ||
             dropTarget.GetComponent<ResultSlot>() != null ||
             dropTarget.GetComponentInParent<ResultSlot>() != null);

        // Additional check: if drop target is a CraftSlot that already has an item, treat as invalid.
        CraftSlot craftSlot = dropTarget != null ? dropTarget.GetComponentInParent<CraftSlot>() : null;
        if (craftSlot != null && craftSlot.currentItem != null)
            droppedOnValidTarget = false;

        if (!droppedOnValidTarget && draggedItem != null)
        {
            Debug.Log("❌ Invalid drop → Returning item");
            if (originSlot != null)
            {
                if (originSlot.item != null && originSlot.item.IsSameItem(draggedItem))
                    originSlot.count++;
                else if (originSlot.item == null)
                    originSlot.SetItem(draggedItem, 1);
                originSlot.UpdateUI();
            }
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
