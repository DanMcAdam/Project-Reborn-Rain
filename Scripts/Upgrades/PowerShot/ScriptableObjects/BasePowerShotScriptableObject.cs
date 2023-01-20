using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/PowerShot")]
public class BasePowerShotScriptableObject : BaseUpgradeScriptableObject
{
    [BoxGroup("UI")]
    public SkillVariable[] PowerShotVars;

    [FoldoutGroup("PowerShot Modifiers")]
    public float RadiusChange, DamageMultiplier, RangeMultiplier;
    [FoldoutGroup("PowerShot Modifiers")]
    public int Count;

    [Button]
    public void TestDescription()
    {
        Debug.Log(GetDescription());
    }
    public override string GetDescription()
    {
        object[] ConvertedVars = new object[PowerShotVars.Length];
        int i = 0;
        foreach (SkillVariable variable in PowerShotVars)
        {
            switch (variable)
            {
                case SkillVariable.Radius:
                    ConvertedVars[i] = (int)(RadiusChange * 100);
                    break;
                case SkillVariable.Damage:
                    ConvertedVars[i] = (int)(DamageMultiplier * 100);
                    break;
                case SkillVariable.Range:
                    ConvertedVars[i] = (int)(RangeMultiplier * 100);
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
                    ConvertedVars[i] = (int)(ChargeSpeed * 100);
                    break;
                case SkillVariable.Uses:
                    ConvertedVars[i] = UseIncrease;
                    break;
                default:
                    break;
            }
            i++;
        }

        switch (PowerShotVars.Length)
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
