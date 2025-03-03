using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerConfig playerConfig;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) 
        {
            UseEnergy(1f);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            RecoverEnergy(1f);
        }
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
        if(playerConfig.CurrentEnergy > playerConfig.MaxEnergy)
        {
            playerConfig.CurrentEnergy = playerConfig.MaxEnergy;
        }
    }
}
