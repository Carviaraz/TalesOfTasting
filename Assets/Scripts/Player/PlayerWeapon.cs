using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] public Weapon initialWeapon;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private float orbitRadius = 0.5f;

    private PlayerAction action;
    public Weapon currentWeapon;
    private SpriteRenderer weaponSprite;
    private Transform playerTransform;
    private bool isDisabled = false;

    // Fire rate control variables
    private float lastFireTime = 0f;
    private bool isHoldingFire = false;

    private void Awake()
    {
        action = new PlayerAction();
        playerTransform = GetComponent<Transform>();
    }

    void Start()
    {
        action.Weapon.Shoot.started += ctx => StartFiring();
        action.Weapon.Shoot.canceled += ctx => StopFiring();

        CreateWeapon(initialWeapon);
    }

    void Update()
    {
        if (!isDisabled)
        {
            WeaponAim();

            // Handle continuous firing if holding button
            if (isHoldingFire)
            {
                TryToShoot();
            }
        }
    }

    // New method to start firing
    private void StartFiring()
    {
        if (!isDisabled && currentWeapon != null)
        {
            isHoldingFire = true;
            // Try to shoot immediately, but respect fire rate
            TryToShoot();
        }
    }

    // New method to stop firing
    private void StopFiring()
    {
        isHoldingFire = false;
    }

    // New method to attempt firing, respecting fire rate
    private void TryToShoot()
    {
        // Get the current weapon's fire rate
        float fireRate = GetCurrentFireRate();

        // Calculate the time between shots based on fire rate
        float fireInterval = 1f / fireRate;

        // Check if enough time has passed since the last shot
        if (Time.time >= lastFireTime + fireInterval)
        {
            ShootWeapon();
        }
    }

    // Helper method to get the current weapon's fire rate
    private float GetCurrentFireRate()
    {
        if (currentWeapon is RangedWeapon rangedWeapon)
        {
            return rangedWeapon.GetFireRate();
        }

        // Default fire rate for other weapons
        return 1f; // 1 shot per second
    }

    private void CreateWeapon(Weapon weaponPrefab)
    {
        // Reset pivot rotation to ensure consistent initial position
        weaponPivot.localRotation = Quaternion.identity;

        // Check if the weapon is melee or ranged
        bool isMelee = weaponPrefab is MeleeWeapon;

        // Create weapon with proper position
        currentWeapon = Instantiate(weaponPrefab);

        // Important: Set the parent first, then adjust the local position
        currentWeapon.transform.SetParent(weaponPivot);

        // Set local position based on weapon type
        if (isMelee)
        {
            // Position melee weapon at pivot + 1 in x-axis
            currentWeapon.transform.localPosition = new Vector3(1f, 0f, 0f);
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Position ranged weapon at orbit radius
            currentWeapon.transform.localPosition = new Vector3(orbitRadius, 0f, 0f);
            currentWeapon.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }

        weaponSprite = currentWeapon.GetComponentInChildren<SpriteRenderer>();

        // Immediately update weapon aim after creation
        WeaponAim();
    }

    public void PickupWeapon(Weapon newWeaponPrefab)
    {
        // Destroy the current weapon
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        // If picking up an existing scene weapon
        if (newWeaponPrefab != null)
        {
            // Remove the scene instance
            Destroy(newWeaponPrefab.gameObject);

            // Create a new instance properly positioned
            CreateWeapon(newWeaponPrefab);
        }
    }

    private void ShootWeapon()
    {
        if (!isDisabled && currentWeapon != null)
        {
            currentWeapon.UseWeapon();
            lastFireTime = Time.time; // Update last fire time when weapon is used
        }
    }

    private void WeaponAim()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Calculate aim direction from pivot point to mouse
        Vector2 aimDirection = mouseWorldPos - weaponPivot.position;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Check if it's a melee weapon
        bool isMelee = currentWeapon is MeleeWeapon;

        weaponPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip weapon sprite based on mouse position relative to player
        if (mouseWorldPos.x < playerTransform.position.x)
        {
            if (isMelee)
            {
                weaponSprite.flipY = true;
            }
            else
            {
                weaponSprite.flipY = true;
            }
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

    public void DisableWeapon()
    {
        isDisabled = true;
        action.Disable();  // Disable weapon input
        isHoldingFire = false; // Stop any ongoing firing
    }
}
