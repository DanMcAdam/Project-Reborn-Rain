using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUpgradeScriptableObject : ScriptableObject
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
    [HideIf("IsLastUpgrade")]
    public BaseUpgradeScriptableObject NextLevel;
    [HideIf("UpgradeLevel", Value = 1)]
    public BaseUpgradeScriptableObject PreviousLevel;

    [FoldoutGroup("Universal Modifiers")]
    public int UseIncrease;
    [FoldoutGroup("Universal Modifiers")]
    public float ChargeSpeed, CooldownReduction, Time;
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
