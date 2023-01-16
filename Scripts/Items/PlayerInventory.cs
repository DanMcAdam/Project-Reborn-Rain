using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public LootPoint SpawnForDroppedGun;

    public Gun Weapon;

    private PlayerAbilityController _player;

    public void SetPlayer(PlayerAbilityController player)
    { 
        _player = player; 
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
