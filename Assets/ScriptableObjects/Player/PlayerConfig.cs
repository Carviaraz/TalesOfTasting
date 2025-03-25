using UnityEngine;

[CreateAssetMenu]
public class PlayerConfig : ScriptableObject
{
    [Header("Data")]
    public string Name;
    public Sprite Icon;
    public string CharacterDescription;
    public GameObject PlayerPrefab;

    [Header("Value")]
    public float MaxHealth;
    public float CurrentHealth;
    public float MaxArmor;
    public float CurrentArmor;
    public float MaxEnergy;
    public float CurrentEnergy;
    public float CritRate;
    public float CritDamage;

    // Store initial values
    private float initialMaxHealth;
    private float initialMaxArmor;
    private float initialMaxEnergy;
    private float initialCritRate;
    private float initialCritDamage;

    public void InitializeStats()
    {
        // Store original values when the game starts
        initialMaxHealth = MaxHealth;
        initialMaxArmor = MaxArmor;
        initialMaxEnergy = MaxEnergy;
        initialCritRate = CritRate;
        initialCritDamage = CritDamage;

        ResetStats(); // Initialize to default values
    }

    public void ResetStats()
    {
        MaxHealth = initialMaxHealth;
        CurrentHealth = MaxHealth;
        MaxArmor = initialMaxArmor;
        CurrentArmor = MaxArmor;
        MaxEnergy = initialMaxEnergy;
        CurrentEnergy = MaxEnergy;
        CritRate = initialCritRate;
        CritDamage = initialCritDamage;
    }
}
