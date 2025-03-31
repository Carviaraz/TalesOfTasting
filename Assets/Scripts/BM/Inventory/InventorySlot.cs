using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public Item item;
    public int count;
    public Image itemIcon;
    public TextMeshProUGUI countText;
    public GameObject itemMenuPanel;
    public bool isMenuOpen = false;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        if (itemIcon == null)
            itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        if (countText == null)
            countText = transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
        inventoryManager = InventoryManager.Instance;
    }

    private void Start()
    {
        UpdateUI();
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(ShowItemMenu);
    }

    private void Update()
    {
        if (isMenuOpen && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                HideItemMenu();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            ShowItemMenu();
    }

    public void ShowItemMenu()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.CloseAllItemMenus();

        if (item != null && itemMenuPanel != null)
        {
            itemMenuPanel.SetActive(true);
            isMenuOpen = true;
            itemMenuPanel.transform.SetAsLastSibling();
            itemMenuPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    public void HideItemMenu()
    {
        if (itemMenuPanel != null)
        {
            itemMenuPanel.SetActive(false);
            isMenuOpen = false;
        }
    }

    public void UseItem()
    {
        if (item != null)
        {
            Debug.Log($"Using item '{item.itemName}', count: {count}");
            item.Use();
            count--;
            if (count <= 0)
                ClearSlot();
            UpdateUI();
            HideItemMenu();
        }
    }

    public void DropItem()
    {
        if (item != null)
        {
            InventoryManager.Instance.RemoveItem(item);
            item = null;
            count = 0;
            UpdateUI();
            HideItemMenu();
        }
    }

    public void UpdateUI()
    {
        if (item != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            itemIcon.color = Color.white;
            // Always display the count if at least one exists.
            if (count >= 1)
            {
                countText.text = count.ToString();
                countText.enabled = true;
            }
            else
            {
                countText.text = "";
                countText.enabled = false;
            }
            Debug.Log("InventorySlot UpdateUI: " + item.itemName + ", count: " + count);
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            itemIcon.color = new Color(1f, 1f, 1f, 0f);
            countText.text = "";
            countText.enabled = false;
            Debug.Log("InventorySlot Clear UI slot");
        }
    }

    public void AddItem(Item newItem, int amount)
    {
        if (item == null)
        {
            item = newItem;
            count = amount;
            Debug.Log("Adding item '" + newItem.itemName + "', amount: " + amount + " to slot " + gameObject.name);
        }
        else if (item == newItem)
        {
            count += amount;
            Debug.Log("Incrementing item '" + newItem.itemName + "' count to " + count + " in slot " + gameObject.name);
        }
        UpdateUI();
    }

    public void Setup(Item newItem, InventoryManager manager)
    {
        item = newItem;
        inventoryManager = manager;
        UpdateUI();
    }

    public void OnClick()
    {
        inventoryManager?.SelectItem(item);
    }

    public void ClearSlot()
    {
        Debug.Log("Clearing slot " + gameObject.name + ", previous item: " + (item != null ? item.itemName : "None"));
        item = null;
        count = 0;
        UpdateUI();
    }

    public void SetItem(Item newItem, int amount)
    {
        item = newItem;
        count = amount;
        UpdateUI();
    }

    // IDropHandler implementation to accept dragged items
    public void OnDrop(PointerEventData eventData)
{
    Debug.Log("InventorySlot OnDrop called on " + gameObject.name);
    GameObject droppedObject = eventData.pointerDrag;
    if (droppedObject == null)
    {
        Debug.LogWarning("No dragged object found for InventorySlot drop.");
        return;
    }
    Debug.Log("Dragged object: " + droppedObject.name);

    // Try to get the DraggedItemData component from the dropped object or its children.
    DraggedItemData draggedData = droppedObject.GetComponent<DraggedItemData>();
    if (draggedData == null)
        draggedData = droppedObject.GetComponentInChildren<DraggedItemData>();

    // Attempt to retrieve the dragged item.
    Item draggedItem = null;
    if (draggedData != null && draggedData.draggedItem != null)
    {
        draggedItem = draggedData.draggedItem;
    }
    else if (DraggedItemData.currentDraggedItem != null)
    {
        draggedItem = DraggedItemData.currentDraggedItem;
        Debug.Log("Using static currentDraggedItem: " + draggedItem.itemName);
    }

    if (draggedItem == null)
    {
        Debug.LogWarning("No valid dragged item data found in InventorySlot OnDrop.");
        return;
    }

    // If the slot is empty, assign the dragged item.
    if (item == null)
    {
        SetItem(draggedItem, 1);
        DraggedItemData.dropSuccessful = true;
        Debug.Log("InventorySlot drop: item set to " + draggedItem.itemName);
    }
    // If the slot contains the same stackable item, increment its count.
    else if (item.IsSameItem(draggedItem) && item.isStackable)
    {
        count++;
        UpdateUI();
        DraggedItemData.dropSuccessful = true;
        Debug.Log("InventorySlot drop: incremented count for " + draggedItem.itemName);
    }
    else
    {
        Debug.Log("InventorySlot already contains a different item. Drop ignored.");
    }
}

}
