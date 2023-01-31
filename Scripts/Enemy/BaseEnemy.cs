using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.ParticleSystem;

public class BaseEnemy : MonoBehaviour, IDamageable, IEnemyBehavior, IDamageGiver
{
    [FoldoutGroup("Assigned by Code")]
    public MMHealthBar _healthBar;
    [FoldoutGroup("Assigned by Code")]
    public DetectPlayer _detectPlayer;
    [FoldoutGroup("Assigned by Code")]
    public NavMeshAgent _navMeshAgent;
    [FoldoutGroup("Assigned by Code")]
    public Transform _target;
    public Queue<int> _previousAttackIDs = new Queue<int>(5);
    [FoldoutGroup("Agent Stats"), Range(0, 150)]
    public int AgentStartingHealth, Damage;
    [FoldoutGroup("Agent Status")]
    public int AgentCurrentHealth;
    [FoldoutGroup("Agent Status")]
    public EnemyBehaviourState _currentState;
    public Transform VisionTransform => this.transform;

    public virtual void Awake()
    {
        _detectPlayer = GetComponent<DetectPlayer>();
        _healthBar = GetComponent<MMHealthBar>();
        _navMeshAgent= GetComponent<NavMeshAgent>();
        AgentCurrentHealth = AgentStartingHealth;
    }

    public virtual void OnEnable()
    {
        TransitionState(EnemyBehaviourState.Idle);
    }

    private void Update()
    {

    }

    public virtual void TransitionState(EnemyBehaviourState newState)
    {
        if (_currentState == newState) return;
        EnemyBehaviourState oldState = _currentState;
        OnStateExit(newState, oldState);
        _currentState = newState;
        OnStateEnter(newState, oldState);
    }

    public virtual void OnStateEnter(EnemyBehaviourState newState, EnemyBehaviourState oldState)
    {

    }

    public virtual void OnStateExit(EnemyBehaviourState newState, EnemyBehaviourState oldState)
    {

    }

    public virtual void EnemyDetected(PlayerAbilityController abilityController)
    {
        _target = abilityController.PlayerTarget;
    }

    public virtual void EnterIdle()
    {
        _detectPlayer.BeginInvoke(this);
    }

    public virtual AttackData GiveDamage()
    {
        throw new System.NotImplementedException();
    }

    public virtual void TakeDamage(AttackData attack)
    {
        if (_previousAttackIDs.Contains(attack.ID))
        {
            Debug.Log("Previous Attack ID used");
        }
        else
        {
            AgentCurrentHealth -= attack.Damage;
            _healthBar.UpdateBar((float)AgentCurrentHealth, 0f, (float)AgentStartingHealth, true);
            EffectManager.Instance.GenerateFloatingText(attack.HitPosition, attack.Damage, this.transform, attack.WasCrit);
            if (AgentCurrentHealth < 0)
            {
                TransitionState(EnemyBehaviourState.Dying);
            }
            else if (_previousAttackIDs.Count == 5)
            {
                _previousAttackIDs.Dequeue();
            }
            _previousAttackIDs.Enqueue(attack.ID);
        }
    }
}

public enum EnemyBehaviourState
{
    Pursue, 
    Idle, 
    ChargingAttack, 
    AttackRecovery,
    Frozen, 
    AttackStyle1, 
    AttackStyle2,
    Stunned, 
    Interrupted, 
    Enraged, 
    Dying
}
