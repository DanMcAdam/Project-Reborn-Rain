using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerOnlyLayerMask, _environmentLayerMask;
    [SerializeField]
    private float _overlapSphereRadius;

    public IEnemyBehavior EnemyBehavior;

    private Collider[] _colliders;
    private bool _hitPlayer;
    private Ray _ray;
    private void OnEnable()
    {
        _ray = new Ray();
    }

    public void BeginInvoke(IEnemyBehavior behavior)
    {
        EnemyBehavior = behavior;
        InvokeRepeating("CheckForEnemies", 1f, .5f);
    }

    public void EndInvoke()
    {
        CancelInvoke();
    }
    private void CheckForEnemies()
    {
        _colliders = Physics.OverlapSphere(transform.position, _overlapSphereRadius, _playerOnlyLayerMask);

        foreach (Collider collider in _colliders)
        {
            PlayerAbilityController player = collider.GetComponentInParent<PlayerAbilityController>();
            if (player is not null)
            {
                if (Physics.Raycast(EnemyBehavior.VisionTransform.position, collider.transform.position - EnemyBehavior.VisionTransform.position, out RaycastHit _raycastHit,50f, _environmentLayerMask ))
                {
                    if (_raycastHit.collider == collider)
                    {
                        EnemyBehavior.EnemyDetected(player);
                        CancelInvoke(); 
                    }
                }
            }
        }


    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

            //Gizmos.DrawWireSphere(transform.position, _overlapSphereRadius);

    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        MoreMountains.Tools.MMDebug.DrawGizmoPoint(transform.position, _overlapSphereRadius, Color.red);

    }
}
