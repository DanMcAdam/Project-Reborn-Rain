using MoreMountains.Feedbacks;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class JumpSlam : CharacterAbility
{
    public override SOCharacterAbility ScriptableObject { get; set; }

    public override Vector3 PlayerPosition { get => _moveController.transform.position; }

    public float HorizontalDiv = 1.05f;
    public float VerticalDiv = .62f;
    public bool LineRenderDebug = false;

    public LineRenderer LineRenderer;

    public float LaunchPower { get => ScriptableObject.MovementPower; }

    private int _extraLaunches = 1;
    private int _extraLaunchesAvailable;
    private bool _showLineAnyway => _abilitycontroller.Inputs.SecondarySpecialHeld && _extraLaunchesAvailable > 0;

    private float _verticalAddition = .5f;

    private int _lineResolution = 500;
    private bool _started;
    private int _startupBuffer = 0;
    private float _gravity;

    private int _linePoints = 25;
    private float _timeBetweenPoint = 0.1f;
    private int _playerCollisionMask;
    private GameObject _projectionSphere;

    private PlayerAttack _playerAttack;
    private ParticleSystem _vfx { get => ScriptableObject.ParticleSystems[0]; }

    public override List<BaseUpgrade> AvailableUpgrades { get; set; }

    public override List<BaseUpgrade> UsedUpgrades { get; set; }



private MMMiniObjectPooler _explosionPool;
    public override void InitializeAbility(ThirdPersonController thirdPersonController, PlayerAbilityController playerAbilityController)
    {
        base.InitializeAbility(thirdPersonController, playerAbilityController);
        LineRenderer = playerAbilityController.LineRenderer;
        _gravity = _moveController.Gravity;
        LineRenderer.positionCount = _lineResolution;
        _started = false;
        _startupBuffer = 0;

        _playerCollisionMask = _moveController.gameObject.layer;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(_playerCollisionMask, i))
            {
                _playerCollisionMask |= 1 << i;
            }
        }

        _projectionSphere = Instantiate(ScriptableObject.OtherStuff[0]);
        _projectionSphere.SetActive(false);

        _explosionPool = _abilitycontroller.ObjectPoolerObject.AddComponent<MMMiniObjectPooler>();
        _explosionPool.NestWaitingPool = true;
        _explosionPool.PoolSize = 5;
        _explosionPool.GameObjectToPool = ScriptableObject.AreaOfEffect.gameObject;
        _explosionPool.FillObjectPool();

        _playerAttack = new PlayerAttack(ScriptableObject.Damage, ScriptableObject.Force, Random.Range(0, 10000), true);
    }

    public override bool Tick(bool released)
    {

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Vector3 startPos = PlayerPosition;
        Vector3 xOffset = startPos + Camera.main.transform.right * .3f;

        Vector3 startVelocity = Camera.main.transform.forward * LaunchPower / HorizontalDiv;
        startVelocity.y = LaunchPower * (Camera.main.transform.forward.y + _verticalAddition) / VerticalDiv;
        if (!released)
        {
            _extraLaunchesAvailable = _extraLaunches;
            ShowJumpPredictionLine(startPos, xOffset, startVelocity);
        }
        else
        {
            _projectionSphere.SetActive(false);
            LineRenderer.enabled = false;
            if (_showLineAnyway) ShowJumpPredictionLine(startPos, xOffset, startVelocity);
            _startupBuffer++;

            if (_moveController.IsDashing)
            {
                ResetValues(true);
                return true;
            }

            if (_moveController.Grounded && _startupBuffer > 5)
            {
                ResetValues(false);
                PlayLandingVFX();

                AudioManager.Instance.PlayAudioInstance(ScriptableObject.AudioRef[0], "JumpSlam", _moveController.transform, 1);

                _playerAttack.ID = UnityEngine.Random.Range(0, 10000);

                _playerAttack.HitPosition = PlayerPosition;
                AreaOfEffect aoeObj = _explosionPool.GetPooledGameObject().GetComponent<AreaOfEffect>();
                if (!aoeObj.ParticlesInstantiated) aoeObj.SetupParticles(_vfx);
                aoeObj.PlayerAttack = _playerAttack;
                aoeObj.AbilityController = _abilitycontroller;
                aoeObj.AOE = ScriptableObject.AOE;
                aoeObj.SetOff();

                if (_abilitycontroller.Inputs.SecondarySpecialHeld && _extraLaunchesAvailable > 0) 
                {
                    _extraLaunchesAvailable--;
                    return false; 
                }
                else
                { return true; }
            }

            if (!_started)
            {
                AudioManager.Instance.PlayAudioInstance(ScriptableObject.AudioRef[0], "JumpSlam", _moveController.transform, 0);
                EffectManager.Instance.PlayMotionBlur(_moveController.transform.position, 3);
                _moveController.AddImpulse(LaunchPower, new Vector3(Camera.main.transform.forward.x, Camera.main.transform.forward.y + _verticalAddition, Camera.main.transform.forward.z), true);
                _started = true;
            }
        }
        return false;



    }

    private void ShowJumpPredictionLine(Vector3 startPos, Vector3 xOffset, Vector3 startVelocity)
    {
        if (!LineRenderer.enabled) LineRenderer.enabled = true;
        if (!_projectionSphere.activeInHierarchy) _projectionSphere.SetActive(true);

        LineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenPoint) + 1;

        int i = 0;
        LineRenderer.SetPosition(i, xOffset);
        Vector3 xCorrection = xOffset;
        _projectionSphere.transform.localScale = (ScriptableObject.AOE * 2) * Vector3.one;
        for (float time = 0; time < _linePoints; time+=_timeBetweenPoint)
        {
            i++;
            xCorrection.x = Mathf.Lerp(xCorrection.x, startPos.x, time);
            xCorrection.z = Mathf.Lerp(xCorrection.z, startPos.z, time);
            Vector3 point = xCorrection + time * startVelocity;
            point.y = xCorrection.y + startVelocity.y * time + (_gravity / 2f * time * time);
            LineRenderer.SetPosition(i, point);

            Vector3 lastPosition = LineRenderer.GetPosition(i - 1);
            if (Physics.Raycast(lastPosition, (point - lastPosition).normalized, out RaycastHit hit, 
                (point - lastPosition).magnitude, _playerCollisionMask))
            {
                LineRenderer.SetPosition(i, hit.point);
                LineRenderer.positionCount = i + 1;
                _projectionSphere.transform.position = hit.point;
                return;
            }    
        }
    }

    private void PlayLandingVFX()
    {
        EffectManager.Instance.PlayCameraShakeGroundSlam(_moveController.transform.position);
        EffectManager.Instance.PlayMotionBlur(_moveController.transform.position);
    }

    private void ResetValues(bool retainMomentum)
    {
        if (retainMomentum)
        {
            _moveController.TurnOffNormalMove = false;
        }
        else
        {
            _moveController.StopMomentum();
        }

        _started = false;
        _startupBuffer = 0;
    }

    public override void ApplyUpgrade(BaseUpgrade upgrade)
    {
        throw new System.NotImplementedException();
    }
}
