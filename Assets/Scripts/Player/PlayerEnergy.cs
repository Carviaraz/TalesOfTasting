using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private float energyDrainRate = 1f;
    [SerializeField] private string dungeonSceneName = "Dungeon";

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

    private void StopEnergyDrain()
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
