using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Jumpslam")]
public class BaseJumpSlamScriptableObject : BaseUpgradeScriptableObject
{
    [BoxGroup("UI")]
    public SkillVariable[] JumpSlamVars;

    [FoldoutGroup("Jumpslam Modifiers")]
    public float RadiusChange, DamageMultiplier, RangeMultiplier;
    [FoldoutGroup("Jumpslam Modifiers")]
    public int Count;

    [Button]
    public void TestDescription()
    {
        Debug.Log(GetDescription());
    }
    public string GetDescription()
    {
        object[] ConvertedVars = new object[JumpSlamVars.Length];
        int i = 0;
        foreach (SkillVariable variable in JumpSlamVars)
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
                default:
                    break;
            }
            i++;
        }

        switch (JumpSlamVars.Length)
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

