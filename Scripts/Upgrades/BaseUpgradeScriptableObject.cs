using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUpgradeScriptableObject : ScriptableObject
{
    [InfoBox("Warning: Ensure that ID is correct for this upgrade path", InfoMessageType.Warning)]
    public int ID;
    [BoxGroup("UI"), HorizontalGroup("UI/Split", 90)]
    [PreviewField(90), HideLabel]
    public Sprite UpgradeImage;
    [BoxGroup("UI"), VerticalGroup("UI/Split/Right"), LabelWidth(100)]
    [MinValue(1)]
    public int UpgradeLevel;
    [BoxGroup("UI"), VerticalGroup("UI/Split/Right"), LabelWidth(100)]
    public string Name;
    [BoxGroup("UI"), VerticalGroup("UI/Split/Right"), LabelWidth(100)]
    [Multiline]
    public string Description;

    public bool IsLastUpgrade;

    [HideIf("UpgradeLevel", Value = 1)]
    public BaseUpgradeScriptableObject PreviousLevel;
    [HideIf("IsLastUpgrade")]
    public BaseUpgradeScriptableObject NextLevel;

    [FoldoutGroup("Universal Modifiers")]
    public int UseIncrease, Shield, Health;
    [FoldoutGroup("Universal Modifiers")]
    public float ChargeSpeed, CooldownReduction, Time, CritChance;
    [HideInInspector]
    public CharacterAbility ability;

    [BoxGroup("UI")]
    public SkillVariable[] SkillVars;

    [FoldoutGroup("Universal Modifiers")]
    public float RadiusChange, DamageMultiplier, RangeMultiplier;
    [FoldoutGroup("Universal Modifiers")]
    public int Count;

    [Button]
    public void TestDescription()
    {
        Debug.Log(GetDescription());
    }
    public string GetDescription()
    {
        object[] ConvertedVars = new object[SkillVars.Length];
        int i = 0;
        foreach (SkillVariable variable in SkillVars)
        {
            switch (variable)
            {
                case SkillVariable.Radius:
                    ConvertedVars[i] = Mathf.RoundToInt(RadiusChange * 100);
                    break;
                case SkillVariable.Damage:
                    ConvertedVars[i] = Mathf.RoundToInt(DamageMultiplier * 100);
                    break;
                case SkillVariable.Range:
                    ConvertedVars[i] = Mathf.RoundToInt(RangeMultiplier * 100);
                    break;
                case SkillVariable.Count:
                    ConvertedVars[i] = Count;
                    break;
                case SkillVariable.Cooldown:
                    ConvertedVars[i] = CooldownReduction;
                    break;
                case SkillVariable.Time:
                    ConvertedVars[i] = Time;
                    break;
                case SkillVariable.AtkSpeed:
                    ConvertedVars[i] = Mathf.RoundToInt(ChargeSpeed * 100);
                    break;
                case SkillVariable.Uses:
                    ConvertedVars[i] = UseIncrease;
                    break;
                case SkillVariable.Shield:
                    ConvertedVars[i] = Shield;
                    break;
                case SkillVariable.Health:
                    ConvertedVars[i] = Health;
                    break;
                case SkillVariable.CritChance:
                    ConvertedVars[i] = Mathf.RoundToInt(CritChance * 100);
                    break;
                default:
                    break;
            }
            i++;
        }

        switch (SkillVars.Length)
        {
            case 1:
                return string.Format(Description, ConvertedVars[0]);
                break;
            case 2:
                return string.Format(Description, ConvertedVars[0], ConvertedVars[1]);
                break;
            case 3:
                return string.Format(Description, ConvertedVars[0], ConvertedVars[1], ConvertedVars[2]);
            case 4:
                return string.Format(Description, ConvertedVars[0], ConvertedVars[1], ConvertedVars[2], ConvertedVars[3]);
            default:
                return Description;
                break;
        }
    }
}

public enum SkillVariable
{
    Radius,
    Damage,
    Range,
    Count,
    Cooldown,
    AtkSpeed,
    Shield,
    Health,
    Uses,
    CritChance, 
    Time
}
