using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractionUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _interactionOverlay;
    [SerializeField]
    private GameObject _twoChoicePrompt, _threeChoicePrompt;
    [SerializeField]
    private TextMeshProUGUI _testText;
    [SerializeField]
    private GameObject _interactionPrompt;
    [SerializeField]
    private UIWeaponSelection _weaponSelection;
    [SerializeField]
    private UIUpgrade _upgradeSelection;

    [SerializeField]
    private GameObject _escMenuOverlay;
    [SerializeField]
    private Slider _volumeSlider;

    public bool IsInteracting { get => _interactionOverlay.activeInHierarchy; }
    public bool EscapeMenuOpen { get => _escMenuOverlay.activeInHierarchy; }
    public bool InteractionPromptActive { get => _interactionPrompt.activeInHierarchy; }

    private LootPoint _interactionObject;
    private List<UIWeaponSelection> _weaponSelectionList;
    private List<UIUpgrade> _upgradesList;
    private StarterAssets.StarterAssetsInputs _inputs { get => PlayerManager.Instance.Player.Inputs; }

    FMOD.Studio.Bus Master;

    // Start is called before the first frame update
    void Start()
    {
        _weaponSelectionList = new List<UIWeaponSelection>();
        _upgradesList= new List<UIUpgrade>();
        Master = FMODUnity.RuntimeManager.GetBus("bus:/");
        Master.getVolume(out float volume);
        _volumeSlider.value = volume;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetVolume(float volume)
    {
        Master.setVolume(volume);
    }

    public void OnEnable()
    {
        StarterAssets.StarterAssetsInputs.EscPressed += EscPressed;
    }

    public void OnDisable()
    {
        StarterAssets.StarterAssetsInputs.EscPressed -= EscPressed;
    }

    public void ShowInteractionPrompt()
    {
        _interactionPrompt.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        _interactionPrompt.SetActive(false);
    }

    public void StartInteraction(LootPoint lootPoint, List<Gun> guns)
    {
        if (!IsInteracting)
        {
            FreeCursor();
            _interactionObject = lootPoint;
            _interactionOverlay.SetActive(true);

            if (_weaponSelectionList.Count > 0)
            {
                foreach (UIWeaponSelection gun in _weaponSelectionList)
                {
                    Destroy(gun.transform.gameObject);
                }
                _weaponSelectionList.Clear();
            }
            foreach (Gun gun in guns)
            {
                UIWeaponSelection newSelection = Instantiate(_weaponSelection, _twoChoicePrompt.transform);
                newSelection.PopulateStats(gun, lootPoint);
                _weaponSelectionList.Add(newSelection);
            }
        }
    }

    public void StartInteraction(LootPoint lootPoint, List<BaseUpgradeScriptableObject> upgradeScripts)
    {
        if (!IsInteracting)
        {

            FreeCursor();
            _interactionObject = lootPoint;
            _interactionOverlay.SetActive(true);
            if (_upgradesList.Count > 0)
            {
                foreach (UIUpgrade upgrade in _upgradesList)
                {
                    Destroy(upgrade.transform.gameObject);
                }
                _upgradesList.Clear();
            }
            foreach (BaseUpgradeScriptableObject upgradeScript in upgradeScripts)
            {
                UIUpgrade newSelection = Instantiate(_upgradeSelection, _threeChoicePrompt.transform);
                newSelection.PopulateStats(upgradeScript, lootPoint);
                _upgradesList.Add(newSelection);
            }
        }
    }

    public void EndInteraction()
    {
        _interactionObject = null;
        HideInteractionPrompt();
        _interactionOverlay.SetActive(false);
        LockCursor();
    }

    private void FreeCursor()
    {
        _inputs.cursorInputForLook = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputs.cursorInputForLook = true;
    }

    public void EscPressed(string s)
    {
        if (!EscapeMenuOpen && !IsInteracting)
        {
            _escMenuOverlay.SetActive(true);
            FreeCursor();
        }
        else if (EscapeMenuOpen)
        {
            _escMenuOverlay.SetActive(false);
            LockCursor();
        }
        else if (IsInteracting) EndInteraction();

    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
