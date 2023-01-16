using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectTest : ScriptableObject
{
    public CharacterAbility ability;

    public ThirdPersonController thirdPersonController;

    public PlayerAbilityController playerController;



    public bool Tick(bool released)
    {
        return ability.Tick(released);
    }
}
