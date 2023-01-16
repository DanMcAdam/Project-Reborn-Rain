using StarterAssets;
using System.Collections.Generic;
using UnityEngine;


public abstract class CharacterAbility: MonoBehaviour
{
    public abstract Vector3 PlayerPosition { get; }
    public abstract SOCharacterAbility ScriptableObject { get; set; }
    public abstract List<BaseUpgrade> AvailableUpgrades { get; set; }
    public abstract List<BaseUpgrade> UsedUpgrades { get; set; }
    public ThirdPersonController _moveController;
    public PlayerAbilityController _abilitycontroller;

    public virtual void InitializeAbility(ThirdPersonController thirdPersonController, PlayerAbilityController playerAbilityController)
    {
        _moveController = thirdPersonController;
        _abilitycontroller = playerAbilityController;
        string ScriptableObjectName = GetType().Name;
        ScriptableObject = (SOCharacterAbility)Resources.Load("AbilitySO/" + ScriptableObjectName);

        Debug.Log("CharacterAbility Initialize called");

        AvailableUpgrades = ScriptableObject.AvailableUpgrades;
    }


    public abstract bool Tick(bool released);

    public abstract void ApplyUpgrade(BaseUpgrade upgrade);


    public BaseUpgrade RequestUpgrade()
    {
        //temporary method to find a list of all ability ID's. Replace in future when fixed upgrade paths make this unecessary, this is messy implementation
        List<int> availableIDs = new List<int>();
        BaseUpgrade chosenUpgrade = null;
        int chosenID;
        if (AvailableUpgrades.Count == 0) { IsNull(); return null; } 

        foreach (BaseUpgrade upgrade in AvailableUpgrades)
        { if (!availableIDs.Contains(upgrade.GetID())) availableIDs.Add(upgrade.GetID()); }

        chosenID = availableIDs[Random.Range(0, availableIDs.Count)];
        foreach (BaseUpgrade upgrade in AvailableUpgrades)
        {
            if (upgrade.GetID() == chosenID)
            {
                if (chosenUpgrade == null)
                {
                    chosenUpgrade = upgrade;
                    continue;
                }
                else if (chosenUpgrade.GetUpgradeLevel() > upgrade.GetUpgradeLevel())
                {
                    chosenUpgrade = upgrade;
                }
            }
        }

        if (chosenUpgrade == null) 
        {
            IsNull();
            return null;
        }
        else { return chosenUpgrade; }


        void IsNull()
        {
            {
                Debug.LogWarning(string.Format("No available upgrades for {0}", GetType().Name));
            }
        }
    }

    public void ChosenUpgrade(BaseUpgrade upgrade)
    {
        AvailableUpgrades.Remove(upgrade);
        UsedUpgrades.Add(upgrade);
    }
}
