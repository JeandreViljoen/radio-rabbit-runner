using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class HUDItemsManager : MonoBehaviour
{
    [SerializeField] private List<WeaponHUDItem> WeaponHUDItems;
    private Dictionary<WeaponType, WeaponHUDItem> LinkedWeaponHUDItems = new();
    [SerializeField] private List<WeaponHUDItem> PerkHUDItems;
    private Dictionary<PerkGroup, WeaponHUDItem> LinkedPerkHUDItems = new();
    
    private LazyService<UpgradesManager> _upgradesManager;
    void Start()
    {
        _upgradesManager.Value.OnUpgradeActivated += OnWeaponUpgrade;
        _upgradesManager.Value.OnPerkActivated += OnPerkUpgrade;
    }
    
    private void OnWeaponUpgrade(UpgradeType upgradeType)
    {
        UpgradeData data = _upgradesManager.Value.GetUpgradeData(upgradeType);
        Weapon wInstance = _upgradesManager.Value.GetWeaponInstance(data.Type);

        int level = wInstance.Level;

        //Init new upgrade
        if (level == 1)
        {
            TryRegisterNewWeapon(data);
        }
        else
        {
            UpdateWeaponDetails(wInstance);
        }
    }
    
    
    private bool TryRegisterNewWeapon(UpgradeData data)
    {
        if (!LinkedWeaponHUDItems.ContainsKey(data.Type))
        {
            foreach (var hudItem in WeaponHUDItems)
            {
                if (!hudItem.IsInit)
                {
                    hudItem.InitHUDItem(data.HudIcon);
                    LinkedWeaponHUDItems.Add(data.Type, hudItem);
                    return true;
                }
            }
        }
        
        Help.Debug(GetType(), "TryRegisterNewWeapon", "Tried to register a new weapon, but not enough open slots on HUD to show the visuals");
        return false;
    }

    private void UpdateWeaponDetails(Weapon instance)
    {
        LinkedWeaponHUDItems[instance.Type].SetLevelField(instance.Level);
    }
    
    
    
    private void OnPerkUpgrade(PerkType perkType)
    {
        PerkUpgradeInfo data = _upgradesManager.Value.GetPerkInfo(perkType);
        Perk pInstance = _upgradesManager.Value.GetPerkInstance(data.GroupType);

        int level = pInstance.Level;

        //Init new upgrade
        if (level == 1)
        {
            TryRegisterNewPerk(data);
        }
        else
        {
            UpdatePerkDetails(pInstance);
        }
    }
    
    private bool TryRegisterNewPerk(PerkUpgradeInfo data)
    {
        if (!LinkedPerkHUDItems.ContainsKey(data.GroupType))
        {
            foreach (var hudItem in PerkHUDItems)
            {
                if (!hudItem.IsInit)
                {
                    hudItem.InitHUDItem(data.HudIcon);
                    LinkedPerkHUDItems.Add(data.GroupType, hudItem);
                    return true;
                }
            }
        }
        
        Help.Debug(GetType(), "TryRegisterNewPerk", "Tried to register a new perk, but not enough open slots on HUD to show the visuals");
        return false;
    }
    
    private void UpdatePerkDetails(Perk instance)
    {
        LinkedPerkHUDItems[instance.PerkGroup].SetLevelField(instance.Level);
    }

}
