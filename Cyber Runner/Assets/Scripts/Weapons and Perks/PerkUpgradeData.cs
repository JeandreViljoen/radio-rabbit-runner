using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "PerkUpgradeData", menuName = "Custom Assets/Perks/Perk Upgrade Data")]
public class PerkUpgradeData : SerializedScriptableObject
{
    public bool IsImplemented = false;
    [GUIColor("grey"), ReadOnly][OdinSerialize, PropertyOrder(1), OnValueChanged("SetType")] public PerkGroup GroupType { get; private set; }
    [SerializeField, PropertyOrder(2), OnValueChanged("SetIDs")] public List<PerkUpgradeInfo> Upgrades;
    public string BaseEnumName;

    public void ForceSetGroupType(PerkGroup type)
    {
        GroupType = type;

        foreach (var perk in Upgrades)
        {
            perk.GroupType = GroupType;
        }
    }

    #region Attributes

    public int UpgradeCount => Upgrades.Count;
    
    private void SetIDs()
    {
        for (int i = 0; i < Upgrades.Count; i++)
        {
            Upgrades[i].ID = i + 1;
        }
    }
    
    private void SetType()
    {
        for (int i = 0; i < Upgrades.Count; i++)
        {
            Upgrades[i].GroupType = GroupType;
        }
    }

    #endregion

    public PerkUpgradeInfo GetUpgradeData(int id)
    {
        if (id == 0)
        {
            Help.Debug(GetType(), "GetUpgradeData", "ID of 0 requested. Id starts at 1. returning null - fix ASAP");
            return null;
        }
        
        if (id > Upgrades.Count)
        {
            Help.Debug(GetType(), "GetUpgradeData", "Tried to get an upgrade number that does not exist. returning null for safety - fix this ASAP");
            return null;
        }
        return Upgrades[id-1];
    }
    
    public PerkUpgradeInfo GetUpgradeData(PerkType perk)
    {
        PerkUpgradeInfo data = Upgrades.Find(u => GroupType + "_" + u.ID == perk.ToString());

        if (data == null)
        {
            Help.Debug(GetType(), "GetUpgradeData", $"Could not find upgrade : {perk} in the {GroupType} Upgrade data scriptable object.");
            return null;
        }

        return data;
    }
    
    public float GetValue(int id)
    {
        return GetUpgradeData(id).Value;
    }
    
    public float GetValue(PerkType type)
    {
        return GetUpgradeData(type).Value;
    }
    
    public PerkType GetUpgradeAtID(int id)
    {
        Enum.TryParse(GroupType + "_" + id, out PerkType upgrade);

        if (upgrade == 0)
        {
            Help.Debug(GetType(), "GetUpgradeAtID", "Enum parsing did not find correct enum - This is bad");
        }
        return upgrade;
    }
    
    [Button(ButtonSizes.Gigantic), GUIColor("blue")]
    private void AutoFillDataFromFirstIndex()
    {
        
        string baseDisplayName = Upgrades[0].DisplayName;
        string baseDescription = Upgrades[0].Description;
        
        foreach (var upgrade in Upgrades)
        {
            upgrade.Name = BaseEnumName + "_" + upgrade.ID;
            upgrade.DisplayName = baseDisplayName + GetDisplayNameNumbering(upgrade.ID);
            upgrade.Description = baseDescription;
        }
    }
    
    [Button(ButtonSizes.Large), GUIColor("blue")]
    private void AutoFillIcons()
    {
        foreach (var upgrade in Upgrades)
        {
            upgrade.Icon = Upgrades[0].Icon;
            upgrade.HudIcon = Upgrades[0].HudIcon;
        }
    }

    private string GetDisplayNameNumbering(int number)
    {
        string output = " ";
        for (int i = 0; i < number; i++)
        {
            output += "I";
        }

        return output;
    }
}

[Serializable]
public class PerkUpgradeInfo
{
    [Title("Upgrade", "$ID", TitleAlignments.Centered)]
    [GUIColor("grey"), ReadOnly] public string Name;
    public string DisplayName;
    public float Value;
    public Sprite Icon;
    public Sprite HudIcon;
    [TextArea(4,10)] public string Description;
    
    [GUIColor("grey"), ReadOnly, HorizontalGroup()] public int ID = 0;
    [GUIColor("grey"), ReadOnly, HorizontalGroup()] public PerkGroup GroupType;
}