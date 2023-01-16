using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseJumpSlam : BaseUpgrade
{
    public abstract float ReturnRadiusChangeMultiplier();
    public abstract float ReturnDamageMultiplier();
    public abstract float ReturnRangeMultiplier();
    public abstract int ReturnHopNumber();
}
