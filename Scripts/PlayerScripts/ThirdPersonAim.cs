using Cinemachine;
using MoreMountains.Feedbacks;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
public class ThirdPersonAim : MonoBehaviour
{
    [SerializeField]
    private Rig _WholeRig;
    [SerializeField]
    private TwoBoneIKConstraint _offHandRig;
    [SerializeField]
    private MultiAimConstraint _bodyRig;
    [SerializeField]
    private MultiAimConstraint _aimRig;
    [SerializeField]
    private Transform _offHandTransform;
    [SerializeField]
    private Transform _offHandConstraintTransform;
    [SerializeField]
    private Transform _offHandAnimatedTransform;
    [SerializeField]
    private Transform _magazineTransform;
    [SerializeField]
    private Transform _gunTransform;
    [SerializeField]
    private CinemachineVirtualCamera _aimVirtualCamera;
    [SerializeField]
    private float _normalCameraSensitivity = 1;
    [SerializeField]
    private float _aimCameraSensitivity = .5f;
    [SerializeField]
    private float _aimTurnCharacterTurnSpeed = 20f, _slowFallGravity;
    [SerializeField]
    private LayerMask _aimColliderMask = new LayerMask();
    [SerializeField]
    private LayerMask _interactionColliderMask = new LayerMask();
    [SerializeField]
    private Image _crossHair;
    [SerializeField]
    private Transform _debugTransform;
    [SerializeField]
    private float _shotsPerMinute;
    [SerializeField]
    private PlayerAbilityController _abilityController;
    [SerializeField]
    PlayerAudio _audio;

    public Transform MuzzleEnd;

    public int MagazineSize;
    public int AmmoCount;

    private ParticleSystem _muzzleFlash;
    private ParticleSystem _bulletHit;
    private AreaOfEffect _areaOfEffect;
    private float _shakeIntensity;
    private float _range;
    private int _piercing;
    private FMODUnity.EventReference _audioEventReference;


    private ThirdPersonController _thirdPersonController;
    private StarterAssetsInputs _starterAssetsInputs;
    private Animator _animator;
    private TimerScript _shootTimer;
    private EffectApplier _effectApplier;

    private Camera _camera;

    //TODO make playerAttack from inventory gun
    PlayerAttack playerAttack;

    private bool _currentlyAiming = false;
    private bool _canShoot;

    private bool _lerpToOriginal = false;
    private bool _lerpToMagazine = false;
    private bool _isReloading = false;
    private bool _reloadingTrigger = false;

    private float _aimRigWeight;
    private float _aimAnimWeight;
    private float _shootingInterval => 1 / (_shotsPerMinute / 60);
    private float _initialGravity;

    private Vector3 _magazinePosition;
    private Vector3 _offHandConstraintOriginalPos;
    private Quaternion _magazineRotation;

    private MMMiniObjectPooler _hitEffectPooler;

    private void Awake()
    {
        if (TryGetComponent<StarterAssetsInputs>(out StarterAssetsInputs assetsInputs))
        {
            _starterAssetsInputs = assetsInputs;
        }
        if (TryGetComponent<ThirdPersonController>(out ThirdPersonController controller))
        {
            _thirdPersonController = controller;
        }
        if (TryGetComponent<Animator>(out Animator animator))
        {
            _animator = animator;
        }
        if (TryGetComponent<EffectApplier>(out EffectApplier effectApplier))
        {
            _effectApplier = effectApplier;
        }

        playerAttack = new PlayerAttack();
        playerAttack.Damage = 4; playerAttack.Force = 1000f;

        _camera = Camera.main;

        _shootTimer = new TimerScript(_shootingInterval);
        _magazinePosition = _magazineTransform.transform.localPosition;
        _magazineRotation = _magazineTransform.transform.localRotation;
        _offHandConstraintOriginalPos = _offHandConstraintTransform.localPosition;

        _initialGravity = _thirdPersonController.Gravity;
    }

    private void Start()
    {

    }

    public void SetWeaponStats(Gun gun)
    {
        MagazineSize = gun.MagazineSize;
        AmmoCount = gun.MagazineSize;
        _shotsPerMinute = gun.AttacksPerMinute;
        playerAttack.Damage = gun.BaseDamage;
        playerAttack.Force = gun.Force;
        playerAttack.GeneratedByPlayer = true;
        _bulletHit = gun.ParticleSystems[0];
        _muzzleFlash = Instantiate(gun.ParticleSystems[1], MuzzleEnd);
        _shakeIntensity = gun.ScreenShakeIntensity;
        _range = gun.AttackDistance;
        _piercing = gun.BasePiercing;
        if (gun.AOE)
        {
            _areaOfEffect = gun.AreaOfEffect;
            _areaOfEffect.AOE = gun.AOESize;
            _areaOfEffect.PlayerAttack = playerAttack;
        }
        _audioEventReference = gun.AudioRef[0];
        _thirdPersonController.ReloadSpeed = gun.ReloadTime;
        _shootTimer.CountdownTime = _shootingInterval;
        if (_hitEffectPooler == null)
        {
            _hitEffectPooler = _abilityController.ObjectPoolerObject.AddComponent<MMMiniObjectPooler>();
            _hitEffectPooler.NestWaitingPool = true;
        }
        _hitEffectPooler.DestroyObjectPool();
        _hitEffectPooler.GameObjectToPool = gun.ParticleSystems[0].gameObject;
        _hitEffectPooler.FillObjectPool();
        _shootTimer.Reset();
    }

    private void Update()
    {
        if (_shootingInterval != _shootTimer.CountdownTime) { _shootTimer.CountdownTime = _shootingInterval; }
        if (!_canShoot && !_isReloading)
        {

            if (_shootTimer.Tick(Time.deltaTime))
            {
                _canShoot = true;
            }
        }

        HandleAim();
        if (_lerpToOriginal == true)
        {
            LerpVector3(_offHandConstraintTransform, _offHandConstraintOriginalPos);
            if (_offHandConstraintTransform.localPosition == _offHandConstraintOriginalPos) { _lerpToOriginal = false; }
        }
        if (_lerpToMagazine == true)
        {
            LerpVector3(_offHandConstraintTransform, _offHandAnimatedTransform.localPosition);
            if (_offHandConstraintTransform.localPosition == _offHandAnimatedTransform.localPosition) _lerpToMagazine = false;
        }
        _bodyRig.weight = Mathf.Lerp(_aimRig.weight, _aimRigWeight, Time.deltaTime * 20f);
        _aimRig.weight = Mathf.Lerp(_aimRig.weight, _aimRigWeight, Time.deltaTime * 20f);
        if (_aimAnimWeight == 1) _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), _aimAnimWeight, Time.deltaTime * 15f));
        else if (_aimAnimWeight < 1) _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), _aimAnimWeight, Time.deltaTime * 30f));
    }

    private void LerpVector3(Transform transform, Vector3 targetVector3)
    {
        float deltaMultipler = 10f;
        Vector3 newPosition = Vector3.zero;
        newPosition.x = Mathf.Lerp(transform.localPosition.x, targetVector3.x, Time.deltaTime * deltaMultipler);
        newPosition.y = Mathf.Lerp(transform.localPosition.y, targetVector3.y, Time.deltaTime * deltaMultipler);
        newPosition.z = Mathf.Lerp(transform.localPosition.z, targetVector3.z, Time.deltaTime * deltaMultipler);
        transform.localPosition = newPosition;
    }

    IInteractable interactableHold = null;
    private void HandleAim()
    {
        bool startedAiming = _starterAssetsInputs.Aim && _currentlyAiming == false;
        bool stopppedAiming = _starterAssetsInputs.Aim == false && _currentlyAiming == true;
        bool animateAim = _isReloading || _starterAssetsInputs.Aim;
        bool rigOn = _starterAssetsInputs.Aim || _canShoot == false;

        Vector3 worldAimPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = _camera.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, _aimColliderMask))
        {

            _debugTransform.position = raycastHit.point;
            worldAimPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }
        else
        {
            worldAimPosition = ray.GetPoint(20f);
            _debugTransform.position = worldAimPosition;
        }

        if (raycastHit.distance < _range)
        {
            if (_debugTransform.gameObject.activeInHierarchy == false)
            {
                _debugTransform.gameObject.SetActive(true);
            }
        }
        else _debugTransform.gameObject.SetActive(false);

        if (_starterAssetsInputs.Aim)
        {
            Vector3 worldAimTarget = worldAimPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * _aimTurnCharacterTurnSpeed);
        }


        if (startedAiming)
        {
            _currentlyAiming = true;
            _aimVirtualCamera.gameObject.SetActive(true);
            _thirdPersonController.SetSensitivity(_aimCameraSensitivity);
            _bodyRig.data.offset = new Vector3(-50, 0, 0);

            if (_abilityController.HasSlowfallAiming)
            { _thirdPersonController.Gravity = _slowFallGravity; }

            Invoke("SetCrosshair", .05f);
        }
        else if (stopppedAiming)
        {
            _currentlyAiming = false;
            _aimVirtualCamera.gameObject.SetActive(false);
            _thirdPersonController.SetSensitivity(_normalCameraSensitivity);
            _bodyRig.data.offset = new Vector3(-20, 0, 0);

            if (_abilityController.HasSlowfallAiming)
            { _thirdPersonController.Gravity = _initialGravity; }

            SetCrosshair();
        }

        if (animateAim)
        {
            _aimAnimWeight = 1f;
        }
        else if (_reloadingTrigger)
        {
            _aimAnimWeight = 1f;
        }
        else
        {
            _aimAnimWeight = .8f;
        }

        if (rigOn)
        {
            _aimRigWeight = 1f;
        }

        if (_thirdPersonController.IsDashing)
        {
            _aimAnimWeight = 0f;
            _WholeRig.weight = 0f;
        }
        else
        {
            _WholeRig.weight = 1f;
        }
;
        HandleInteraction();

        if (AmmoCount == 0 || _starterAssetsInputs.Reload)
        {
            _starterAssetsInputs.Reload = false;
            if (!_isReloading)
            {
                HandleReload();
            }
        }
        else if (_starterAssetsInputs.Shoot && interactableHold == null)
        {
            if (hitTransform != null && _canShoot && raycastHit.distance < _range)
            {
                if (_piercing > 0)
                {
                    RaycastHit[] raycastHitAll = Physics.RaycastAll(ray, _range, Physics.AllLayers);
                    playerAttack.ID = UnityEngine.Random.Range(0, 10000);
                    for (int i = 0; i < _piercing + 1; i++)
                    {
                        if (raycastHitAll.Length <= i)
                        {
                            break;
                        }
                        if (raycastHitAll[i].transform.TryGetComponent(out IDamageable damageable))
                        {
                            Shot(raycastHitAll[i].point, raycastHitAll[i].point, raycastHitAll[i].normal, damageable, true);
                        }
                        else
                        {
                            Shot(raycastHitAll[i].point, raycastHitAll[i].point, raycastHitAll[i].normal, true);
                        }
                    }
                    Shot(transform.position);
                }
                else if (hitTransform.TryGetComponent(out IDamageable damageable))
                {
                    Shot(raycastHit.point, raycastHit.point, raycastHit.normal, damageable);
                }
                else
                {
                    Shot(raycastHit.point, raycastHit.point, raycastHit.normal);
                }
            }
            else if (_canShoot)
            {
                Shot(worldAimPosition);
            }
        }
    }

    private void Shot(Vector3 shakePosition)
    {
        _muzzleFlash.Play();
        _audio.PlayGunShot(_audioEventReference);
        EffectManager.Instance.PlayCameraShakeShoot(shakePosition, _shakeIntensity);
        AmmoCount--;
        _aimRigWeight = 1;
        _canShoot = false;
    }

    private void Shot(Vector3 shakePosition, Vector3 hitPosition, Vector3 hitNormal)
    {
        PlayHitEffect(hitPosition, hitNormal);
        Shot(shakePosition);
    }

    private void Shot(Vector3 shakePosition, Vector3 hitPosition, Vector3 hitNormal, bool check)
    {
        PlayHitEffect(hitPosition, hitNormal);
    }

    private void Shot(Vector3 shakePosition, Vector3 hitPosition, Vector3 hitNormal, IDamageable damageable, bool check)
    {
        playerAttack.HitPosition = hitPosition;
        _effectApplier.ApplyAttackEffects(playerAttack, damageable);
        PlayHitEffect(hitPosition, hitNormal);
    }

    private void Shot(Vector3 shakePosition, Vector3 hitPosition, Vector3 hitNormal, IDamageable damageable)
    {
        playerAttack.HitPosition = hitPosition;
        playerAttack.ID = UnityEngine.Random.Range(0, 10000);
        _effectApplier.ApplyAttackEffects(playerAttack, damageable);
        Shot(shakePosition, hitPosition, hitNormal);
    }

    private void PlayHitEffect(Vector3 hitPosition, Vector3 hitNormal)
    {
        GameObject hitEffect = _hitEffectPooler.GetPooledGameObject();
        hitEffect.transform.position = hitPosition;
        hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
        hitEffect.SetActive(true);
    }

    private void HandleInteraction()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = _camera.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, _interactionColliderMask))
        {
            hitTransform = raycastHit.transform;
        }
        if (hitTransform != null
            && hitTransform.TryGetComponent(out IInteractable interactable)
            && Vector3.Distance(transform.position, hitTransform.position) <= interactable.ReturnInteractionDistance()
            && interactable.ThisIsInteractable())
        {
            interactableHold = interactable;
            interactableHold.ShowInteractionPrompt();

            if (_starterAssetsInputs.Shoot)
            {
                interactableHold.Interact();
            }
        }
        else if (interactableHold != null)
        {
            interactableHold.PlayerStoppedLooking();
            interactableHold = null;
        }
    }

    private void HandleReload()
    {
        _animator.SetBool("Reloading", true);
        _canShoot = false;
        _isReloading = true;
        _reloadingTrigger = true;

    }

    public void ReloadFinished()
    {
        _isReloading = false;
        _canShoot = true;
        AmmoCount = MagazineSize;
        _animator.SetBool("Reloading", false);
    }

    public void PlayReloadSound(int sequence)
    {
        _audio.PlayReload(sequence);
    }

    public void StartLerp()
    {
        _lerpToMagazine = true;
    }

    public void MagazineReloadAnimation(int MagazineStuckToHand)
    {


        if (MagazineStuckToHand == 1)
        {
            _magazineTransform.SetParent(_offHandTransform);
        }
        else
        {
            _lerpToOriginal = true;
            _magazineTransform.SetParent(_gunTransform);
            _magazineTransform.localPosition = _magazinePosition;
            _magazineTransform.transform.localRotation = _magazineRotation;
        }
    }

    public void InterruptReload()
    {
        if (_isReloading)
        {
            _isReloading = false;
            _animator.SetBool("Reloading", false);
            _lerpToMagazine = false;
            MagazineReloadAnimation(0);
        }
    }

    private void SetCrosshair()
    {
        if (_currentlyAiming)
        {
            _crossHair.gameObject.SetActive(true);
        }
        else
        {
            _crossHair.gameObject.SetActive(false);
        }
    }
}
