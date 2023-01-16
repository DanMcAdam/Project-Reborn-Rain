using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public PlayerAbilityController Player;

    private void Awake()
    { 
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {

    }

    public bool RegisterPlayer(PlayerAbilityController player)
    {
        Player = player;

        return true;
    }


}
