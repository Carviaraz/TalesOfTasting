using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CraftingSystem : MonoBehaviour
{
    [Header("UI References")]
    public CraftSlot[] craftSlots;
    public Image resultImage;
    public Button cookButton;
    public ResultSlot resultSlot;
    
    [Header("Recipes")]
    public List<Recipe> availableRecipes;
    
    private Recipe currentRecipe;
    private InventoryManager inventoryManager;  
    
    void Start()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("❌ InventoryManager not found in the scene! Make sure it's in the hierarchy.");
        }
        else
        {
            Debug.Log("✅ InventoryManager found successfully.");
        }

        SetupCookButton();
    }

    
    void OnEnable()
    {
        SetupCookButton();
        CheckRecipes();
    }
    
    private void ValidateRecipes()
    {
        if (availableRecipes == null || availableRecipes.Count == 0)
        {
            Debug.LogWarning("⚠️ No recipes available in the CraftingSystem");
            return;
        }
        
        foreach (Recipe recipe in availableRecipes)
        {
            if (recipe == null)
            {
                Debug.LogError("❌ Null recipe found in availableRecipes list");
                continue;
            }
            
            if (recipe.result == null)
            {
                Debug.LogError($"❌ Recipe '{recipe.name}' has no result item");
            }
            
            if (recipe.ingredients == null || recipe.ingredients.Count == 0)
            {
                Debug.LogError($"❌ Recipe '{recipe.name}' has no ingredients");
            }
        }
    }
    
    private void SetupCookButton()
    {
        if (cookButton == null)
        {
            Debug.LogError("❌ Cook button reference is missing");
            return;
        }

        cookButton.onClick.RemoveAllListeners();
        cookButton.onClick.AddListener(OnCookButtonClicked);
        cookButton.interactable = false; // Disable by default
    }
    
    public void CheckRecipes()
    {
        Debug.Log("🔍 Checking for matching recipes...");

        List<Item> currentIngredients = new List<Item>();
        bool hasAnyIngredient = false;

        foreach (CraftSlot slot in craftSlots)
        {
            if (slot == null)
            {
                Debug.LogError("❌ Craft slot reference is null");
                continue;
            }

            currentIngredients.Add(slot.currentItem);

            if (slot.currentItem != null)
            {
                Debug.Log($"✅ Slot contains: {slot.currentItem.itemName}");
                hasAnyIngredient = true;
            }
            else
            {
                Debug.Log("🔴 Empty slot detected.");
            }
        }

        if (!hasAnyIngredient)
        {
            Debug.LogWarning("⚠️ No ingredients in slots! Skipping recipe check.");
            ClearResult();
            return;
        }

        currentRecipe = FindMatchingRecipe(currentIngredients);

        if (currentRecipe != null)
        {
            Debug.Log($"✅ Recipe found: {currentRecipe.result.itemName}");

            if (resultImage != null)
            {
                resultImage.sprite = currentRecipe.result.icon;
                resultImage.enabled = true;
                resultImage.color = Color.white;
            }

            cookButton.interactable = true;
        }
        else
        {
            Debug.Log("❌ No matching recipe found.");
            ClearResult();
        }
    }

    private Recipe FindMatchingRecipe(List<Item> ingredients)
    {
        List<Item> validIngredients = ingredients.Where(i => i != null).ToList(); // Ignore empty slots
        Debug.Log($"🔍 Checking {validIngredients.Count} valid ingredients...");

        if (validIngredients.Count == 0)
        {
            Debug.LogError("❌ No valid ingredients provided!");
            return null;
        }

        foreach (Recipe recipe in availableRecipes)
        {
            if (recipe == null)
            {
                Debug.LogError("❌ Recipe is null in availableRecipes!");
                continue;
            }

            Debug.Log($"🔄 Checking recipe: {recipe.name}");

            if (recipe.Matches(validIngredients))
            {
                Debug.Log($"✅ Match found: {recipe.result.itemName}");
                return recipe;
            }
            else
            {
                Debug.Log($"❌ {recipe.name} does not match.");
            }
        }

        Debug.Log("❌ No matching recipe found.");
        return null;
    }

    private void ClearResult()
    {
        currentRecipe = null;
        if (resultImage != null)
        {
            resultImage.enabled = false;
        }
        if (cookButton != null)
        {
            cookButton.interactable = false;
        }
    }

    public void OnCookButtonClicked()
    {
        CheckRecipes(); // Ensure `currentRecipe` is updated before crafting

        // ✅ Fix: Check if a recipe was found
        if (currentRecipe == null)
        {
            Debug.Log("❌ Cannot craft: No valid recipe found.");
            return;
        }

        if (currentRecipe.result == null)
        {
            Debug.Log("❌ Crafted item (result) is NULL! Recipe might be broken.");
            return;
        }

        Debug.Log($"✅ Crafting {currentRecipe.result.itemName}");
        CraftItem();
    }

    public void CraftItem()
    {
        if (currentRecipe == null || currentRecipe.result == null)
        {
            Debug.LogError("❌ Cannot craft: Recipe or result is NULL!");
            return;
        }

        Debug.Log($"✅ Crafting {currentRecipe.result.itemName}...");

        // Add crafted item to inventory **before clearing slots**
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(currentRecipe.result);
            Debug.Log($"📦 Added {currentRecipe.result.itemName} to inventory");
        }
        else
        {
            Debug.LogError("❌ InventoryManager is NULL! Cannot add item.");
        }

        // Now clear crafting slots
        foreach (CraftSlot slot in craftSlots)
        {
            for (int i = 0; i < slot.itemCount; i++)
            slot.ClearSlot();
        }

        Debug.Log("🧹 Crafting slots cleared. Delaying recipe check...");

        // ✅ Prevent crash by delaying recipe check
        StartCoroutine(DelayedRecipeCheck());
    }

    private IEnumerator DelayedRecipeCheck()
    {
        yield return new WaitForSeconds(0.5f); // Small delay before checking
        Debug.Log("🔄 Rechecking recipes after crafting...");
        CheckRecipes(); // Now check recipes safely
    }

    [System.Serializable]
    public class Recipe
    {
        public string name;
        public List<Item> ingredients;
        public Item result;
        
        public bool Matches(List<Item> currentIngredients)
        {
            if (ingredients == null || currentIngredients == null)
            {
                Debug.Log("❌ Ingredients list is null.");
                return false;
            }

            List<Item> validIngredients = currentIngredients.Where(i => i != null).ToList();
            Debug.Log($"🔍 Recipe '{name}' needs: {string.Join(", ", ingredients.Select(i => i.itemName))}");
            Debug.Log($"🛠️ Provided: {string.Join(", ", validIngredients.Select(i => i.itemName))}");

            if (ingredients.Count != validIngredients.Count)
            {
                Debug.Log($"❌ Ingredient count mismatch! Recipe needs {ingredients.Count}, but got {validIngredients.Count}");
                return false;
            }

            List<string> recipeItems = ingredients.Select(i => i.itemName).OrderBy(i => i).ToList();
            List<string> providedItems = validIngredients.Select(i => i.itemName).OrderBy(i => i).ToList();

            if (!recipeItems.SequenceEqual(providedItems))
            {
                Debug.Log($"❌ Ingredient mismatch! Expected {string.Join(", ", recipeItems)} but got {string.Join(", ", providedItems)}");
                return false;
            }

            Debug.Log($"✅ Recipe {name} matches!");
            return true;
        }
    }
}
