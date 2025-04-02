using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEnergy : MonoBehaviour
{
    public static PlayerEnergy Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private float energyDrainRate = 1f;
    [SerializeField] private string dungeonSceneName = "Dungeon";

    private PlayerHealth playerHealth;
    private bool isDraining = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.Log("PlayerHealth not found");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartEnergyDrain();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopEnergyDrain();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == dungeonSceneName)
        {
            StartEnergyDrain();
        }
        else
        {
            StopEnergyDrain();
        }
    }

    private void StartEnergyDrain()
    {
        if (SceneManager.GetActiveScene().name == dungeonSceneName)
        {
            InvokeRepeating(nameof(DrainEnergy), 1f, 1f); // Calls DrainEnergy every second
        }
    }

    public void StopEnergyDrain()
    {
        CancelInvoke(nameof(DrainEnergy));
    }

    private void DrainEnergy()
    {
        UseEnergy(energyDrainRate);
    }

    public void UseEnergy(float amount)
    {
        playerConfig.CurrentEnergy -= amount;
        if (playerConfig.CurrentEnergy < 0)
        {
            playerConfig.CurrentEnergy = 0;
            playerHealth.PlayerDead();
        }
    }

    public void RecoverEnergy(float amount)
    {
        playerConfig.CurrentEnergy += amount;
        if (playerConfig.CurrentEnergy > playerConfig.MaxEnergy)
        {
            playerConfig.CurrentEnergy = playerConfig.MaxEnergy;
        }
    }
}
