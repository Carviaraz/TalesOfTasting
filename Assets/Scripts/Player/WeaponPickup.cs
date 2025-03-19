using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private Weapon weaponPrefab;
    [SerializeField] private float interactionDistance = 2f;

    private bool playerInRange = false;
    private PlayerWeapon playerWeapon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerWeapon = collision.GetComponent<PlayerWeapon>();
            if (playerWeapon != null)
            {
                playerInRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            playerWeapon = null;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            playerWeapon.PickupWeapon(weaponPrefab);
            Destroy(gameObject);
        }
    }
}
