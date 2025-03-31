using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResultSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Item currentItem;
    public Image itemImage;
    private InventoryManager inventoryManager;
    
    // For drag functionality
    private Canvas parentCanvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform startParent;
    private GameObject draggedItemInstance;
    
     private void Awake()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in the scene!");
        }

        if (itemImage == null)
        {
            itemImage = GetComponent<Image>();
        }
    }

    
    private void Start()
    {
        UpdateUI();
    }
    
    public void SetItem(Item newItem)
    {
        currentItem = newItem;
        UpdateUI();
    }
    
    public void ClearSlot()
    {
        currentItem = null;
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (itemImage == null)
        {
            Debug.LogError("Image reference is missing in ResultSlot!");
            return;
        }

        if (currentItem != null && currentItem.icon != null)
        {
            itemImage.sprite = currentItem.icon;
            itemImage.color = Color.white;
            itemImage.enabled = true;
        }
        else
        {
            itemImage.sprite = null;
            itemImage.color = new Color(1, 1, 1, 0.5f);
            itemImage.enabled = true;
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        // Handle items being dropped onto this slot
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null)
        {
            // Check if the dropped item is a DraggableItem
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            if (draggableItem != null && draggableItem.parentSlot != null)
            {
                // Check what type of slot the item is coming from
                if (draggableItem.parentSlot is ResultSlot)
                {
                    ResultSlot sourceSlot = draggableItem.parentSlot as ResultSlot;
                    Item droppedItem = sourceSlot.currentItem;
                    
                    if (droppedItem != null)
                    {
                        // If this slot already has an item, swap
                        if (currentItem != null)
                        {
                            // Save our current item
                            Item ourItem = currentItem;
                            
                            // Set our slot to the dropped item
                            SetItem(droppedItem);
                            
                            // Set the source slot to our item
                            sourceSlot.SetItem(ourItem);
                        }
                        else
                        {
                            // Just take the dropped item
                            SetItem(droppedItem);
                            sourceSlot.ClearSlot();
                        }
                    }
                }
                else if (draggableItem.parentSlot is InventorySlot)
                {
                    InventorySlot sourceSlot = draggableItem.parentSlot as InventorySlot;
                    Item droppedItem = sourceSlot.item;
                    
                    if (droppedItem != null)
                    {
                        // If this slot already has an item, swap or handle appropriately
                        if (currentItem != null)
                        {
                            // Handle according to your game's rules
                            // For example, you might return the current item to inventory
                            // and take the new item from inventory
                            
                            inventoryManager.AddItem(currentItem);
                            SetItem(droppedItem);
                            sourceSlot.ClearSlot();
                        }
                        else
                        {
                            // Just take the dropped item from inventory
                            SetItem(droppedItem);
                            sourceSlot.ClearSlot();
                        }
                    }
                }
            }
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null)
        {
            // Nothing to drag
            return;
        }
        
        // Create a temporary item for dragging
        draggedItemInstance = new GameObject("DraggedItem");
        draggedItemInstance.transform.SetParent(parentCanvas.transform);
        draggedItemInstance.transform.SetAsLastSibling();  // Draw on top
        
        // Add a RectTransform
        RectTransform draggableRectTransform = draggedItemInstance.AddComponent<RectTransform>();
        draggableRectTransform.sizeDelta = rectTransform.sizeDelta;
        
        // Add an Image component
        Image draggableImage = draggedItemInstance.AddComponent<Image>();
        draggableImage.sprite = currentItem.icon;
        draggableImage.raycastTarget = false;  // Make it pass through raycasts
        
        // Add a DraggableItem component
        DraggableItem draggableItem = draggedItemInstance.AddComponent<DraggableItem>();
        draggableItem.parentSlot = this;  // Fixed: Now we're passing the ResultSlot component
        
        // Set starting position to mouse position
        draggedItemInstance.transform.position = eventData.position;
        
        // Slightly fade the original item
        if (itemImage != null)
        {
            itemImage.color = new Color(1, 1, 1, 0.5f);
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItemInstance != null)
        {
            draggedItemInstance.transform.position = eventData.position;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItemInstance != null)
        {
            // Check if dragged over a valid target
            if (eventData.pointerEnter != null)
            {
                // Check if it's an item slot
                ResultSlot targetSlot = eventData.pointerEnter.GetComponent<ResultSlot>();
                if (targetSlot != null)
                {
                    // For swapping items
                    Item ourItem = currentItem;
                    Item theirItem = targetSlot.currentItem;
                    
                    // If target has an item, swap them
                    if (theirItem != null)
                    {
                        targetSlot.SetItem(ourItem);
                        SetItem(theirItem);
                    }
                    // Otherwise just move our item there
                    else
                    {
                        targetSlot.SetItem(ourItem);
                        ClearSlot();
                    }
                }
                else
                {
                    // Try to find an inventory slot
                    InventorySlot inventorySlot = eventData.pointerEnter.GetComponent<InventorySlot>();
                    if (inventorySlot != null)
                    {
                        // Add to inventory
                        if (inventoryManager != null && currentItem != null)
                        {
                            inventoryManager.AddItem(currentItem);
                            ClearSlot();
                        }
                    }
                }
            }
            else
            {
                // Dropped outside any valid target
                // Option 1: Put back in slot (do nothing)
                // Option 2: Add to inventory
                if (inventoryManager != null && currentItem != null)
                {
                    inventoryManager.AddItem(currentItem);
                    ClearSlot();
                }
            }
            
            // Restore original opacity
            if (itemImage != null && currentItem != null)
            {
                itemImage.color = Color.white;
            }
            
            // Destroy the temporary dragged object
            Destroy(draggedItemInstance);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            Debug.Log($"Clicked result slot with item: {currentItem.itemName}");

            if (inventoryManager == null)
            {
                inventoryManager = FindAnyObjectByType<InventoryManager>();
                if (inventoryManager == null)
                {
                    Debug.LogError("InventoryManager still null on click!");
                    return;
                }
            }

            inventoryManager.AddItem(currentItem);
            Debug.Log($"Added {currentItem.itemName} to inventory.");
            ClearSlot();
        }
        else
        {
            Debug.Log("Result slot clicked but empty.");
        }
    }
}
