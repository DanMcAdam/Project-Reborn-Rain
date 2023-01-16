using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBehavior
{
    public void EnemyDetected(PlayerAbilityController abilityController);

    public void EnterIdle();

    public Transform VisionTransform { get; }

}
