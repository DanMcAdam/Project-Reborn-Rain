using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgrade : MonoBehaviour
{
    public BaseUpgradeScriptableObject UpgradeScript;
    PlayerAbilityController _abilityController { get => PlayerManager.Instance.Player; }
    [SerializeField]
    private Image _upgradeSprite;
    [SerializeField]
    private TextMeshProUGUI _upgradeName, _upgradeLevel, _upgradeDescription;

    private LootPoint _lootPoint;

    public void PopulateStats(BaseUpgradeScriptableObject upgradeScript, LootPoint lootpoint)
    {
        _lootPoint= lootpoint;
        UpgradeScript = upgradeScript;

        _upgradeName.text = UpgradeScript.Name;
        _upgradeLevel.text = UpgradeScript.IsLastUpgrade? "MAX LEVEL" : "Level " + UpgradeScript.UpgradeLevel;
        _upgradeDescription.text = UpgradeScript.GetDescription();
        _upgradeSprite.sprite = UpgradeScript.UpgradeImage;
    }

    public void OnThisChoiceSelected()
    {
        if (UpgradeScript.ability != null)
            UpgradeScript.ability.ApplyUpgrade(UpgradeScript);
        else
            _abilityController.ChooseUpgrade(UpgradeScript);

        _lootPoint.DisableThisLootPoint();
    }
}
