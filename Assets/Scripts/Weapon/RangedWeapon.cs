using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private Projectile ProjectilePrefab;

    public override void UseWeapon()
    {
        PlayShootAnimation();

        // Create projectile
        Projectile projectile = Instantiate(ProjectilePrefab);
        projectile.transform.position = shootPos.position;

        // Calculate spread angle
        float randomSpread = Random.Range(itemWeapon.MinSpread, itemWeapon.MaxSpread);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, randomSpread);

        // Set projectile direction based on weapon rotation + spread
        projectile.Direction = spreadRotation * shootPos.up;  // Apply spread to weapon aim direction

        // Set the projectile's rotation to match its movement direction
        projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, projectile.Direction);
    }
}
