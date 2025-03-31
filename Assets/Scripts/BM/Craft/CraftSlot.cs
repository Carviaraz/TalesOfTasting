using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public static CraftSlot currentCraftSlot;
    public Item currentItem;
    public Image itemImage;           // Should be the child Image that displays the item icon.
    public Button slotButton;         // Optional: if you want the slot to be clickable.
    public GameObject inventoryUI;    // Reference to the inventory UI (to open it when needed)
    public int itemCount = 1;         // Always 1 for craft slots.
    public TextMeshProUGUI itemCountText; // (Optional) to display count.

    private CraftingSystem craftingSystem;
    private bool isOpeningInventory = false;

    private void Awake()
    {
        if (itemImage == null)
        {
            Transform imgTransform = transform.Find("Image");
            if (imgTransform != null)
                itemImage = imgTransform.GetComponent<Image>();
            else
                Debug.LogWarning("Image component not found in CraftSlot " + gameObject.name);
        }
        if (itemCountText == null)
        {
            Transform textTransform = transform.Find("ItemCount");
            if (textTransform != null)
                itemCountText = textTransform.GetComponent<TextMeshProUGUI>();
            else
                Debug.Log("ItemCount text not found in CraftSlot " + gameObject.name);
        }
        if (slotButton == null)
        {
            slotButton = GetComponent<Button>();
            if (slotButton == null)
                Debug.LogWarning("Button component not found in CraftSlot " + gameObject.name);
        }
        craftingSystem = FindObjectOfType<CraftingSystem>();
        if (craftingSystem == null)
            Debug.LogWarning("CraftingSystem not found in scene. Please add it.");
    }

    private void Start()
    {
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(HandleSlotClick);
        }
        UpdateUI();
    }

    public void HandleSlotClick()
    {
        isOpeningInventory = true;
        if (currentItem != null)
            ReturnItemToInventory();
        else
            OpenInventory();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isOpeningInventory && currentItem != null)
            ReturnItemToInventory();
        isOpeningInventory = false;
    }

    private void ReturnItemToInventory()
    {
        if (currentItem != null)
        {
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.AddItem(currentItem);
                Debug.Log("Returning 1 x '" + currentItem.itemName + "' to inventory from CraftSlot.");
                ClearSlot();
            }
        }
    }

    public void OpenInventory()
    {
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI reference is missing in CraftSlot!");
            return;
        }
        if (!inventoryUI.activeSelf)
        {
            inventoryUI.SetActive(true);
            currentCraftSlot = this;
        }
    }

    public void SetItem(Item newItem, int amount = 1)
    {
        currentItem = newItem;
        itemCount = amount;
        Debug.Log("CraftSlot SetItem: " + (newItem != null ? newItem.itemName : "null") + ", count: " + amount);
        UpdateUI();
        if (craftingSystem != null)
            craftingSystem.CheckRecipes();
        // Signal that the drop was successful.
        DraggedItemData.dropSuccessful = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemCount = 0;
        UpdateUI();
        if (craftingSystem != null)
            craftingSystem.CheckRecipes();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("CraftSlot OnDrop called on " + gameObject.name);

        // If the craft slot already contains an item, ignore the drop.
        if (currentItem != null)
        {
            Debug.Log("Craft slot already contains an item. Drop ignored.");
            return;
        }

        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null)
        {
            Debug.LogWarning("No dragged object found!");
            return;
        }
        Debug.Log("Dragged object: " + droppedObject.name);

        // Try to get DraggedItemData from the dropped object (or its children)
        DraggedItemData draggedData = droppedObject.GetComponent<DraggedItemData>();
        if (draggedData == null)
            draggedData = droppedObject.GetComponentInChildren<DraggedItemData>();

        if (draggedData != null && draggedData.draggedItem != null)
        {
            Debug.Log("Receiving item from DraggedItemData: " + draggedData.draggedItem.itemName);
            SetItem(draggedData.draggedItem, 1);
            return;
        }
        else if (DraggedItemData.currentDraggedItem != null)
        {
            Debug.Log("Using static currentDraggedItem: " + DraggedItemData.currentDraggedItem.itemName);
            SetItem(DraggedItemData.currentDraggedItem, 1);
            return;
        }

        Debug.LogWarning("No valid dragged item data found on dropped object.");
    }

    public void UpdateUI()
    {
        if (currentItem != null && currentItem.icon != null)
        {
            itemImage.sprite = currentItem.icon;
            itemImage.color = Color.white;
            itemImage.enabled = true;
            if (itemCountText != null)
            {
                itemCountText.text = itemCount > 1 ? itemCount.ToString() : "";
                itemCountText.enabled = itemCount > 1;
            }
            Debug.Log("CraftSlot UpdateUI: currentItem = " + currentItem.itemName);
        }
        else
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
            if (itemCountText != null)
            {
                itemCountText.text = "";
                itemCountText.enabled = false;
            }
            Debug.Log("CraftSlot UpdateUI: No current item.");
        }
    }

    public Item TakeItem()
    {
        Item takenItem = currentItem;
        currentItem = null;
        UpdateUI();
        if (craftingSystem != null)
            craftingSystem.CheckRecipes();
        return takenItem;
    }

    public void AddItem(Item newItem)
    {
        currentItem = newItem;
        UpdateUI();
        if (craftingSystem != null)
            craftingSystem.CheckRecipes();
    }
}
