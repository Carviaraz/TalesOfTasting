using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class ItemWeapon : ItemData
{
    [Header("Data")]
    public WeaponType WeaponType;
    public WeaponRarity Rarity;

    [Header("setting")]
    public float Damage;
    public float Speed;
    public float RequiredEnergy;
    public float FireRate;
    public float MinSpread;
    public float MaxSpread;
}
