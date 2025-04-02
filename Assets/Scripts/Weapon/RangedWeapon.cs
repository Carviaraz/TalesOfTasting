using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private Projectile ProjectilePrefab;

    public float GetFireRate()
    {
        return itemWeapon != null ? itemWeapon.FireRate : 1f; // Default to 1 shot per second if no item weapon
    }

    public override void UseWeapon()
    {
        //PlayShootAnimation();
        // Create projectile
        Projectile projectile = Instantiate(ProjectilePrefab);
        projectile.transform.position = shootPos.position;
        projectile.Damage = itemWeapon.Damage;
        projectile.Speed = itemWeapon.Speed;

        // Calculate spread angle
        float randomSpread = Random.Range(itemWeapon.MinSpread, itemWeapon.MaxSpread);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, randomSpread);

        // Set projectile direction based on weapon rotation + spread
        projectile.Direction = spreadRotation * shootPos.up;  // Apply spread to weapon aim direction
        float angle = Mathf.Atan2(projectile.Direction.y, projectile.Direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
