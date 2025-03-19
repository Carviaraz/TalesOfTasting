using System.Collections;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private float attackCooldown = 0.5f; // Cooldown time between attacks
    private bool isAttacking = false;

    public override void UseWeapon()
    {
        if (!isAttacking) // Check if cooldown is active
        {
            isAttacking = true;
            PlayShootAnimation();
            Debug.Log("Melee attack triggered!");

            // Start cooldown
            StartCoroutine(ResetCooldown());
        }
        else
        {
            Debug.Log("Attack on cooldown!");
        }
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false; // Reset cooldown
        Debug.Log("Melee weapon ready to attack again.");
    }
}
