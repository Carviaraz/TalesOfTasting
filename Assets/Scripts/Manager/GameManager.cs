using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Reference")]
    [SerializeField] CharacterList characterList;
    [NonSerialized] public PlayerConfig currentPlayer;

    [Header("Game Data")]
    public int coins = 0;
    public List<string> inventory = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetSelectedCharacter();
    }

    private void SetSelectedCharacter()
    {
        // Ensure character list is assigned
        if (characterList == null || characterList.Characters.Length == 0)
        {
            Debug.LogError("CharacterList is missing or empty!");
            return;
        }

        // Get the selected character
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        if (selectedIndex < 0 || selectedIndex >= characterList.Characters.Length)
        {
            Debug.LogError("Invalid character index, resetting to default.");
            selectedIndex = 0;
        }

        currentPlayer = characterList.Characters[selectedIndex];
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            return true;
        }
        return false;
    }

    public void AddItem(string item)
    {
        inventory.Add(item);
    }

    public void RemoveItem(string item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
        }
    }

}
