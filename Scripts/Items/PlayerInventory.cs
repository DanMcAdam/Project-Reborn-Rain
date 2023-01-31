using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public LootPoint SpawnForDroppedGun;

    public Gun Weapon;

    private PlayerAbilityController _player;

    private PlayerStats _playerStats;

    public Dictionary<BaseItem, TimerScript> CountdownItems;

    public List<BaseItem> ItemInventory;

    public BaseItem ItemToAdd;

    [SerializeField, Button]
    private void AddItem()
    {
        PickupItem(ItemToAdd);
    }
    public void SetPlayer(PlayerAbilityController player)
    {
        _player = player;
        _playerStats = player.PlayerStats;
        CountdownItems = new Dictionary<BaseItem, TimerScript>();
        ItemInventory = new List<BaseItem>();
    }


    public void PickupItem(BaseItem item)
    {
        if (item.ItemProperties.Contains(ItemProperties.OnTimerCooldown))
        {
            TimerScript newTimer = new TimerScript();
            newTimer.CountdownTime = item.Time;
            CountdownItems.Add(item, newTimer);
        }
        if (item.ItemProperties.Contains(ItemProperties.StatModifier))
        {
            item.StatModifier(_playerStats);
        }
        ItemInventory.Add(item);
    }

    public void DropItem(BaseItem item, bool destroyItem)
    {
        //TODO make item lootpoint, populate item. Remove Modifiers and cooldown dictionary position as part of process (add RemoveSelf method to item?)
    }

    public Gun ReturnCurrentWeapon()
    {
        return Weapon;
    }

    public void ReplaceCurrentWeapon(Gun newGun)
    {
        DropGun(Weapon);
        Weapon = newGun;
        _player.SetWeaponStats(Weapon);
    }

    private void DropGun(Gun oldGun)
    {
        LootPoint lootPoint = Instantiate(SpawnForDroppedGun, transform.position, Quaternion.identity);
        lootPoint.GunList.Add(oldGun);
        lootPoint.DestroyOnDisable = true;
    }
}
