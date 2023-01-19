using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UpgradeDescriptionModifier
{
    public static string StringFormatter(string description, int modifier)
    {
        return string.Format(description, modifier);
    }

    public static string StringFormatter(string description, int modifier, int secondModifier)
    {
        return string.Format(description, modifier, secondModifier);
    }

    public static string StringFormatter(string description, int modifier, int secondModifier, int thirdModifier)
    {
        return string.Format(description, modifier, secondModifier, thirdModifier);
    }


}
