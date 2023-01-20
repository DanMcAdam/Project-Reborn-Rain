using Sirenix.OdinInspector;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbility : SerializedMonoBehaviour
{
    public abstract Vector3 PlayerPosition { get; }
    public abstract SOCharacterAbility ScriptableObject { get; set; }
    public List<BaseUpgradeScriptableObject> AvailableUpgrades;
    public abstract List<BaseUpgradeScriptableObject> HeldUpgrades { get; set; }
    public ThirdPersonController _moveController;
    public PlayerAbilityController _abilitycontroller;


    public virtual void InitializeAbility(ThirdPersonController thirdPersonController, PlayerAbilityController playerAbilityController)
    {
        _moveController = thirdPersonController;
        _abilitycontroller = playerAbilityController;
        string ScriptableObjectName = GetType().Name;
        ScriptableObject = (SOCharacterAbility)Resources.Load("AbilitySO/" + ScriptableObjectName);

        Debug.Log("CharacterAbility Initialize called");

    }


    public abstract bool Tick(bool released);

    public abstract void ApplyUpgrade(BaseUpgradeScriptableObject upgrade);


    public abstract BaseUpgradeScriptableObject RequestUpgrade();

    public void ChosenUpgrade(BaseUpgradeScriptableObject upgrade)
    {
        AvailableUpgrades.Remove(upgrade);
        ApplyUpgrade(upgrade);
        HeldUpgrades.Remove(upgrade.PreviousLevel);
        HeldUpgrades.Add(upgrade);
        if (!upgrade.IsLastUpgrade) AvailableUpgrades.Add(upgrade.NextLevel);
    }
}
