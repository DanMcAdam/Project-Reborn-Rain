using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUpgradeScriptableObject : ScriptableObject
{
    public int ID;
    public int UpgradeLevel;
    public string Name;
    public string Description;
    public int UseIncrease;
    public bool IsLastUpgrade;
    public Image UpgradeImage;
}
