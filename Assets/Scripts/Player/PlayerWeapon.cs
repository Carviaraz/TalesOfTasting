using System;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Weapon initialWeapon;
    [SerializeField] private Transform weaponPivot;  // Reference to the pivot GameObject
    [SerializeField] private float orbitRadius = 0.5f;  // Distance from player center

    private PlayerAction action;
    private Weapon currentWeapon;
    private SpriteRenderer weaponSprite;
    private Transform playerTransform;

    private void Awake()
    {
        action = new PlayerAction();
    }

    void Start()
    {
        action.Weapon.Shoot.performed += ctx => ShootWeapon();
        CreateWeapon(initialWeapon);
    }

    void Update()
    {
        WeaponAim();
    }

    private void CreateWeapon(Weapon weaponPrefab)
    {
        playerTransform = GetComponent<Transform>();
        // Create weapon at the orbit radius distance from pivot
        Vector3 spawnPosition = weaponPivot.position + new Vector3(orbitRadius, 0f, 0f);
        currentWeapon = Instantiate(weaponPrefab, spawnPosition, Quaternion.Euler(0, 0, -90), weaponPivot);
        weaponSprite = currentWeapon.GetComponentInChildren<SpriteRenderer>();
    }

    private void ShootWeapon()
    {
        if (currentWeapon == null)
        {
            return;
        }

        currentWeapon.UseWeapon();
    }

    private void WeaponAim()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Calculate aim direction from pivot point to mouse
        Vector2 aimDirection = mouseWorldPos - weaponPivot.position;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Rotate the pivot instead of the weapon directly
        weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip weapon sprite based on mouse position relative to player
        if (mouseWorldPos.x < playerTransform.position.x)
        {
            weaponSprite.flipY = true;
        }
        else
        {
            weaponSprite.flipY = false;
        }
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action?.Disable();
    }
}
