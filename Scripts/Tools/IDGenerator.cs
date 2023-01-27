using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IDGenerator
{
    public static AttackData GenerateID (AttackData attackData)
    {
        attackData.ID = UnityEngine.Random.Range(0, 10000);
        return attackData;
    }
}
