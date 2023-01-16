using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIWeaponSelection : MonoBehaviour
{
    public Gun NewGun;
    public Gun CurrentGun;
    PlayerInventory _inventory { get => PlayerManager.Instance.Player.Inventory; }
    LootPoint _lootPoint;
    [SerializeField]
    private Image _weaponImage;
    [SerializeField]
    private TextMeshProUGUI _gunDamage;
    [SerializeField]
    private TextMeshProUGUI _gunDamageChange;
    [SerializeField]
    private TextMeshProUGUI _gunShotsPerMinute;
    [SerializeField]
    private TextMeshProUGUI _gunShotsPerMinuteChange;
    [SerializeField]
    private TextMeshProUGUI _gunBasePiercing;
    [SerializeField]
    private TextMeshProUGUI _gunPiercingChange;
    [SerializeField]
    private TextMeshProUGUI _gunMagazineSize;
    [SerializeField]
    private TextMeshProUGUI _gunMagazineSizeChange;

    public void PopulateStats(Gun newGun, LootPoint lootPoint)
    {
        _lootPoint = lootPoint;
        NewGun = newGun;
        CurrentGun = _inventory.ReturnCurrentWeapon();

        _weaponImage.sprite = NewGun.WeaponImage;

        _gunDamage.text = NewGun.BaseDamage.ToString();
        AssignIntText(_gunDamageChange, NewGun.BaseDamage, CurrentGun.BaseDamage);
        _gunShotsPerMinute.text = NewGun.AttacksPerMinute.ToString();
        AssignFloatText(_gunShotsPerMinuteChange, NewGun.AttacksPerMinute, CurrentGun.AttacksPerMinute);
        _gunBasePiercing.text = NewGun.BasePiercing.ToString();
        AssignIntText(_gunPiercingChange, NewGun.BasePiercing, CurrentGun.BasePiercing);
        _gunMagazineSize.text = NewGun.MagazineSize.ToString();
        AssignIntText(_gunMagazineSizeChange, NewGun.MagazineSize, CurrentGun.MagazineSize);
    }

    private void AssignIntText(TextMeshProUGUI text, int newGunStat, int currentGunStat)
    {
        int intCalc;
        string calcString;
        intCalc = newGunStat - currentGunStat;
        if (intCalc > 0)
        {
            calcString = " + " + UIManager.Instance.ReturnString(intCalc);
            text.color = Color.green;
        }
        else if (intCalc < 0)
        {
            calcString = UIManager.Instance.ReturnString(intCalc);
            text.color = Color.red;
        }
        else
        {
            calcString = "+- 0";
            text.color = Color.white;
        }
        text.text = calcString;
    }

    private void AssignFloatText(TextMeshProUGUI text, float newGunStat, float currentGunStat)
    {
        float floatCalc;
        string calcString;
        floatCalc = newGunStat - currentGunStat;
        if (floatCalc > 0)
        {
            calcString = " + " + floatCalc.ToString();
            text.color = Color.green;
        }
        else if (floatCalc < 0)
        {
            calcString = floatCalc.ToString();
            text.color = Color.red;
        }
        else
        {
            calcString = "+- 0";
            text.color = Color.white;
        }
        text.text = calcString;
    }

    public void OnThisChoiceSelected()
    {
        Debug.Log("Clicked on choice");
        _inventory.ReplaceCurrentWeapon(NewGun);
        _lootPoint.DisableThisLootPoint();
    }
}
