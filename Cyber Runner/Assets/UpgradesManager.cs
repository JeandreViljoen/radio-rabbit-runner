using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class UpgradesManager : MonoService
{  
    private WeaponLibrary WeaponLibrary;
    private List<WeaponType> _activeWeapons = new ();
    private List<UpgradeType> _activeUpgrades= new ();

    private List<WeaponType> _availableComboUpgrades = new ();

    public event Action<UpgradeType> OnUpgradeActivated; 
    
    void Start()
    {
        
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
        return _activeWeapons.Contains(weapon);
    }
    
    public bool HasUpgrade(UpgradeType upgrade)
    {
        return _activeUpgrades.Contains(upgrade);
    }
    
    public bool HasCombo(WeaponType weapon)
    {
        return _availableComboUpgrades.Contains(weapon);
    }

    public void RegisterWeapon(WeaponType weapon)
    {
        if (!HasWeapon(weapon))
        {
            _activeWeapons.Add(weapon);
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
