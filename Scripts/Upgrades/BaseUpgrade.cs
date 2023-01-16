using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUpgrade : MonoBehaviour 
{
    public abstract string GetName();
    public abstract string GetDescription();
    public abstract int GetUpgradeLevel();
    public abstract int GetID();
    public abstract int GetUseIncrease();
    public abstract bool GetIsLastUpgrade();
    public abstract void Initialize();

    public abstract Image GetImage();
}
