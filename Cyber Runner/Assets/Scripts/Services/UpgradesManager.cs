using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class UpgradesManager : MonoService
{
    [SerializeField] private WeaponLibrary WeaponLibrary;
    private Dictionary<WeaponType, Weapon> _weaponInstances = new();
    private List<UpgradeType> _activeUpgrades = new();
    private List<PerkType> _activePerks = new();
    private Dictionary<PerkGroup, Perk> _perkGroupInstances = new();

    private HashSet<WeaponType> _uniqueWeapons = new HashSet<WeaponType>();
    private HashSet<PerkGroup> _uniquePerks = new HashSet<PerkGroup>();
    public int MaxWeaponCount;
    public int MaxPerkCount;

    private void loadPerks()
    {
        foreach (var perk in WeaponLibrary.PerkList)
        {
            if (!perk.IsImplemented)
            {
                continue;
            }

            Perk p = new Perk(perk);
            _perkGroupInstances.Add(p.PerkGroup, p);
        }
    }

    private List<WeaponType> _availableComboUpgrades = new();

    public event Action<UpgradeType> OnUpgradeActivated;
    public event Action<PerkType> OnPerkActivated;

    void Start()
    {
        if (WeaponLibrary == null)
        {
            Help.Debug(GetType(), "!!!! WARNING !!!!",
                "WeaponLibrary not assigned - MAKE SURE IT IS ASSIGNED IN INSPECTOR OR EVERYTHING WILL BREAK");
        }

        loadPerks();
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
            Debug.LogWarning(
                $"Tried to remove {weapon} from available combo upgrades list but did not find it. - This shouldnt really happen");
        }
    }

    public bool HasWeaponUnlocked(WeaponType weapon)
    {
        return _weaponInstances[weapon].IsUnlocked;
    }

    public bool HasUpgrade(UpgradeType upgrade)
    {
        return _activeUpgrades.Contains(upgrade);
    }

    public bool HasPerk(PerkType perk)
    {
        return _activePerks.Contains(perk);
    }

    public bool HasPerk(PerkType perk, out PerkUpgradeInfo info)
    {
        if (_activePerks.Contains(perk))
        {
            info = WeaponLibrary.GetPerkData(GetPerkGroupFromPerkType(perk)).GetUpgradeData(perk);
            return true;
        }

        info = null;
        return false;
    }

    public bool HasCombo(WeaponType weapon)
    {
        return _availableComboUpgrades.Contains(weapon);
    }

    public void RegisterWeaponOnStart(Weapon weapon)
    {
        if (!_weaponInstances.ContainsKey(weapon.Type))
        {
            _weaponInstances.Add(weapon.Type, weapon);
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
            if (_uniqueWeapons.Count < MaxWeaponCount)
            {
                _uniqueWeapons.Add(GetUpgradeData(upgrade).Type);
            }
            OnUpgradeActivated?.Invoke(upgrade);
            
        }
        else
        {
            Debug.LogWarning($"Tried to register {upgrade} upgrade but it already exists. Not adding again.");
        }
    }

    public void RegisterPerkUpgrade(PerkType perk)
    {
        if (!HasPerk(perk))
        {
            _activePerks.Add(perk);
            if (_uniquePerks.Count < MaxPerkCount)
            {
                _uniquePerks.Add(GetPerkInfo(perk).GroupType);
            }
            
            OnPerkActivated?.Invoke(perk);
        }
        else
        {
            Debug.LogWarning($"Tried to register {perk} perk but it already exists. Not adding again.");
        }
    }

    public Perk GetPerkInstance(PerkGroup perk)
    {
        return _perkGroupInstances[perk];
    }

    public PerkType GetNextPerkUpgrade(PerkGroup perk)
    {
        return _perkGroupInstances[perk].GetNextUpgrade();
    }

    public Weapon GetWeaponInstance(WeaponType type)
    {
        if (_weaponInstances.ContainsKey(type))
        {
            return _weaponInstances[type];
        }

        Help.Debug(GetType(), "GetWeaponInstance",
            $"Tried to get a weapon instance of type {type} but no such weapon exists in the 'instanced weapons' dictionary");
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
        WeaponType wpn = GetWeaponTypeFromUpgrade(upgrade);

        return GetWeaponUpgradeData(wpn).GetUpgradeData(upgrade);
    }

    public WeaponUpgradeData GetWeaponUpgradeData(WeaponType weapon)
    {
        return WeaponLibrary.GetWeaponData(weapon);
    }

    public PerkUpgradeData GetPerkData(PerkGroup perkGroup)
    {
        return WeaponLibrary.GetPerkData(perkGroup);
    }

    public PerkUpgradeInfo GetPerkInfo(PerkType perkUpgrade)
    {
        PerkGroup group = GetPerkGroupFromPerkType(perkUpgrade);

        return WeaponLibrary.GetPerkData(group).GetUpgradeData(perkUpgrade);
    }
    
    public WeaponType GetWeaponTypeFromUpgrade(UpgradeType upgrade)
    {
        string wpn = upgrade.ToString().Split('_')[0];
        Enum.TryParse(wpn, out WeaponType weaponType);

        return weaponType;
    }

    public PerkGroup GetPerkGroupFromPerkType(PerkType perkType)
    {
        string perk = perkType.ToString().Split('_')[0];
        Enum.TryParse(perk, out PerkGroup perkGroup);

        return perkGroup;
    }

    public PerkType GetRandomUnOwnedPerk()
    {
        List<PerkType> availablePerks = GetPossiblePerkUpgrades();

        int rng = UnityEngine.Random.Range(0, availablePerks.Count);

        return availablePerks[rng];
    }

    public List<PerkType> GetPossiblePerkUpgrades()
    {
        //TODO: ADD CHECK FOR WHEN SLOTS ARE FULL
        List<PerkType> availablePerks = new List<PerkType>();

        foreach (var perk in _perkGroupInstances)
        {
            
            if (_uniquePerks.Count == MaxPerkCount)
            {
                if (!_uniquePerks.Contains(perk.Value.PerkGroup)) continue;
            }
            
            PerkType p = perk.Value.GetNextUpgrade();

            //Null check
            if (p != PerkType.None)
            {
                // //If max unique perks have been reached
                // if (_uniquePerks.Count == MaxPerkCount)
                // {
                //     //Compare and add, only if the perk is part of the existing unique groups
                //     foreach (var pGroup in _uniquePerks)
                //     {
                //         if (GetPerkInfo(p).GroupType == pGroup)
                //         {
                //             availablePerks.Add(p);
                //             break;
                //         }
                //     }
                // }
                // else
                // {
                    //If cap not reached, just add the perk
                    availablePerks.Add(p);
                //}

                if (_uniquePerks.Count > MaxPerkCount)
                {
                    Help.Debug(GetType(), "GetPossiblePerkUpgrades", "THe hashset uniquePerks, had mmore entries than the max perk allowance. This is bad and shouldnt happen. fix immediately");
                }
            }
        }

        return availablePerks;
    }

    public List<UpgradeType> GetPossibleWeaponUpgrades()
    {
        //TODO: ADD CHECK FOR WHEN SLOTS ARE FULL
        List<UpgradeType> options = new List<UpgradeType>();

        foreach (var weapon in _weaponInstances)
        {
            if (_uniqueWeapons.Count == MaxWeaponCount)
            {
                if (!_uniqueWeapons.Contains(weapon.Value.Type)) continue;
            }
            
            UpgradeType current = weapon.Value.GetNextUpgrade();
            if (current != UpgradeType.None)
            {
                // if (_uniqueWeapons.Count == MaxWeaponCount)
                // {
                //     //Compare and add, only if the perk is part of the existing unique groups
                //     foreach (var wType in _uniqueWeapons)
                //     {
                //         if (GetUpgradeData(current).Type == wType)
                //         {
                //             options.Add(current);
                //             break;
                //         }
                //     }
                // }
                // else
                // {
                    options.Add(current);
                //}
                
                if (_uniqueWeapons.Count > MaxWeaponCount)
                {
                    Help.Debug(GetType(), "GetPossibleWeaponUpgrades", "THe hashset uniqueWeapons, had mmore entries than the max weapon allowance. This is bad and shouldnt happen. fix immediately");
                }
                
            }
        }

        return options;
    }

    public UpgradeType GetRandomUnownedWeaponUpgrade()
    {
        List<UpgradeType> options = GetPossibleWeaponUpgrades();

        int rng = UnityEngine.Random.Range(0, options.Count);

        return options[rng];
    }

    public bool HasPerkGroup(PerkGroup group, out float value)
    {
        //Search perk group in reverse
        for (int index = _perkGroupInstances[group].Data.Upgrades.Count - 1; index >= 0; index--)
        {
            var perkUpgrade = _perkGroupInstances[group].Data.Upgrades[index];

            Enum.TryParse(perkUpgrade.Name, out PerkType type);

            //If perk is present, return true and the value
            if (HasPerk(type))
            {
                value = perkUpgrade.Value;
                return true;
            }

        }

        //else return false and default value to 0
        value = 0;
        return false;
    }

    public List<UpgradeType> GetStarterWeaponDraft(int amount)
    {
        List<UpgradeType> weaponsForThisDraft = GetPossibleWeaponUpgrades();
        List < UpgradeType > drafted = new();

        for (int i = 0; i < amount; i++)
        {
            DoWeapon();
        }
        return drafted;
        
        void DoWeapon()
        {
            var item = weaponsForThisDraft[UnityEngine.Random.Range(0, weaponsForThisDraft.Count)];
            drafted.Add(item);
            weaponsForThisDraft.Remove(item);
        }
    }

    public GenericUpgrades GetFullDraft(int amount)
    {
        GenericUpgrades u = new();

        List<UpgradeType> weaponsForThisDraft = GetPossibleWeaponUpgrades();
        List<PerkType> perksForThisDraft = GetPossiblePerkUpgrades();

        //If no Drafts
        if (weaponsForThisDraft.Count == 0 && perksForThisDraft.Count == 0)
        {
            Help.Debug(GetType(), "GetFullDraft", "Tried to draft but not upgrades or perks found" );
            return u;
        }

        for (int i = 0; i < amount; i++)
        {
            
            int typeSelectRoll = UnityEngine.Random.Range(1,101);

            if (weaponsForThisDraft.Count == 0)
            {
                typeSelectRoll = 100;
            }

            if (perksForThisDraft.Count == 0)
            {
                typeSelectRoll = 0;
            }
            
        
            //Draft Weapon
            if (typeSelectRoll <= 50)
            {
                DoWeapon();
            }
            //Draft Perk
            else if(typeSelectRoll >= 50)
            {
                DoPerk();
            }
            
        }
        
        void DoPerk()
        {
            if (perksForThisDraft.Count <= 0)
            {
                if (weaponsForThisDraft.Count > 0)
                {
                    DoWeapon();
                }
                return;
            }
            
            var item = perksForThisDraft[UnityEngine.Random.Range(0, perksForThisDraft.Count)];
            u.Perks.Add(item);
            perksForThisDraft.Remove(item);
        }
        
        void DoWeapon()
        {
            if (weaponsForThisDraft.Count <= 0)
            {
                if (perksForThisDraft.Count > 0)
                {
                    DoPerk();
                }
                return;
            }
            
            var item = weaponsForThisDraft[UnityEngine.Random.Range(0, weaponsForThisDraft.Count)];
            u.Weapons.Add(item);
            weaponsForThisDraft.Remove(item);
        }

        return u;

    }

}

public class GenericUpgrades
{
    public List<UpgradeType> Weapons = new List<UpgradeType>();
    public List<PerkType> Perks = new List<PerkType>();
    
}


