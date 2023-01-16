using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrogHopUpgrade : BaseJumpSlam
{
    public BaseJumpSlamScriptableObject[] UpgradeObjects;
        
    private int _currentUpgrade = 0;
    private int _jumpSlamUpgradeID = 0;

    private string _alteredDescription;

    public override void Initialize()
    {
        if (_currentUpgrade == 0) { _alteredDescription = string.Format(UpgradeObjects[_currentUpgrade].Description); }
        if (_currentUpgrade == 1) { _alteredDescription = string.Format(UpgradeObjects[_currentUpgrade].Description, UpgradeObjects[_currentUpgrade].HopNumber); }
    }

    public override float ReturnDamageMultiplier()
    {
        return UpgradeObjects[_currentUpgrade].DamageMultiplier;
    }

    public override string GetDescription()
    {
        return _alteredDescription;
    }

    public override int ReturnHopNumber()
    {
        return UpgradeObjects[_currentUpgrade].HopNumber;
    }

    public override int GetID()
    {
        return _jumpSlamUpgradeID;
    }

    public override Image GetImage()
    {
        return UpgradeObjects[_currentUpgrade].UpgradeImage;
    }

    public override bool GetIsLastUpgrade()
    {
        if (!UpgradeObjects[_currentUpgrade].IsLastUpgrade && _currentUpgrade + 1 > UpgradeObjects.Length) Debug.LogError("Custom Error: FrogHopUpgrade does not have last upgrade");
        else if (UpgradeObjects[_currentUpgrade].IsLastUpgrade && _currentUpgrade + 1 < UpgradeObjects.Length) Debug.LogError("Custom Error: FrogHopUpgrade Last Upgrade Misordered, Has Reached Final Upgrade Before End of Array");

        return (UpgradeObjects[_currentUpgrade].IsLastUpgrade);
    }

    public override string GetName()
    {
        return UpgradeObjects[_currentUpgrade].Name;
    }

    public override float ReturnRadiusChangeMultiplier()
    {
        return UpgradeObjects[_currentUpgrade].RadiusChange;
    }

    public override float ReturnRangeMultiplier()
    {
        throw new System.NotImplementedException();
    }

    public override int GetUpgradeLevel()
    {
        return _currentUpgrade - 1;
    }

    public override int GetUseIncrease()
    {
        return UpgradeObjects[_currentUpgrade].UseIncrease;
    }
}
