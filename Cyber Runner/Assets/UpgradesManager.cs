using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class UpgradesManager : MonoService
{  
    [SerializeField] private WeaponLibrary WeaponLibrary;
    private Dictionary<WeaponType, Weapon> _activeWeapons = new ();
    private List<UpgradeType> _activeUpgrades= new ();

    private List<WeaponType> _availableComboUpgrades = new ();

    public event Action<UpgradeType> OnUpgradeActivated; 
    
    void Start()
    {
        if (WeaponLibrary == null)
        {
           Help.Debug(GetType(), "!!!! WARNING !!!!", "WeaponLibrary not assigned - MAKE SURE IT IS ASSIGNED IN INSPECTOR OR EVERYTHING WILL BREAK");
        }
    }
    
    void Update()
    {
       
    }

    public void AddWeaponToComboDrafts(WeaponType weapon)
    {
        if (!HasCombo(weapon))
        {
            _availableComboUpgrades.Add(weapon);
        }
        else
        {
            Debug.LogWarning($"Tried to register {weapon} to combo upgrades but it already exists. Not adding again.");
        }
    }
    
    public void RemoveWeaponFromComboDrafts(WeaponType weapon)
    {
        if (HasCombo(weapon))
        {
            _availableComboUpgrades.Remove(weapon);
        }
        else
        {
            Debug.LogWarning($"Tried to remove {weapon} from available combo upgrades list but did not find it. - This shouldnt really happen");
        }
    }

    public bool HasWeapon(WeaponType weapon)
    {
        return _activeWeapons.ContainsKey(weapon);
    }
    
    public bool HasUpgrade(UpgradeType upgrade)
    {
        return _activeUpgrades.Contains(upgrade);
    }
    
    public bool HasCombo(WeaponType weapon)
    {
        return _availableComboUpgrades.Contains(weapon);
    }

    public void RegisterWeapon(Weapon weapon)
    {
        if (!HasWeapon(weapon.Type))
        {
            _activeWeapons.Add(weapon.Type, weapon);
        }
        else
        {
            Debug.LogWarning($"Tried to register {weapon} weapon but it already exists. Not adding again.");
        }
    }
    
    public void RegisterUpgrade(UpgradeType upgrade)
    {
        if (!HasUpgrade(upgrade))
        {
            _activeUpgrades.Add(upgrade);
            OnUpgradeActivated?.Invoke(upgrade);
        }
        else
        {
            Debug.LogWarning($"Tried to register {upgrade} upgrade but it already exists. Not adding again.");
        }
    }

    public Weapon GetWeaponInstance(WeaponType type)
    {
        if (_activeWeapons.ContainsKey(type))
        {
            return _activeWeapons[type];
        }

        Help.Debug(GetType(), "GetWeaponInstance", $"Tried to get a weapon instance of type {type} but no such weapon exists in the 'active weapons' dictionary");
        return null;
    }

    public UpgradeType GetNextUpgradeForWeapon(WeaponType weapon)
    {
        Weapon currentWeapon = GetWeaponInstance(weapon);
        return currentWeapon.GetNextUpgrade();
    }

    public List<UpgradeData> GetValidUpgrades(WeaponType weapon)
    {
        List<UpgradeData> allUpgrades = WeaponLibrary.GetWeaponData(weapon).Upgrades;

        List<UpgradeData> tempValidList = new List<UpgradeData>();

        foreach (var upgrade in allUpgrades)
        {
            UpgradeType toCompare;
            Enum.TryParse(upgrade.Name, out toCompare);
            
            if (HasUpgrade(toCompare))
            {
                continue;
            }
            
            tempValidList.Add(upgrade);
        }
        
        return tempValidList;
    }

    public UpgradeData GetUpgradeData(UpgradeType upgrade)
    {
        WeaponType wpn =  GetWeaponTypeFromUpgrade(upgrade);

        return GetWeaponUpgradeData(wpn).GetUpgradeData(upgrade);
    }

    public WeaponUpgradeData GetWeaponUpgradeData(WeaponType weapon)
    {
        return WeaponLibrary.GetWeaponData(weapon);
    }

   
    public WeaponType GetWeaponTypeFromUpgrade( UpgradeType upgrade)
    {
        string wpn = upgrade.ToString().Split( '_' )[0];
        Enum.TryParse(wpn, out WeaponType weaponType);
        
        return weaponType;
    }
}
