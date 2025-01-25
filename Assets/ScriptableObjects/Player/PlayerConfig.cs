using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerConfig : ScriptableObject
{
    [Header("Data")]
    public string Name;
    public Sprite Icon;

    [Header("Value")]
    public float MaxHealth;
    public float CurrentHealth;
    public float MaxArmor;
    public float CurrentArmor;
    public float MaxEnergy;
    public float CurrentEnergy;
    public float CritRate;
    public float CritDamage;


}
