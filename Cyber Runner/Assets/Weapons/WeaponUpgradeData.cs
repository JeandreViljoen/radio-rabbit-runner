using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUpgradeData", menuName = "Custom Assets/Weapons/Weapon Upgrade Data")]

public class WeaponUpgradeData : SerializedScriptableObject
{
    [OdinSerialize, PropertyOrder(1), OnValueChanged("SetType")] public WeaponType WeaponType { get; private set; }
    [SerializeField, PropertyOrder(2), OnValueChanged("SetIDs")] public List<UpgradeData> Upgrades;
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
            Upgrades[i].Type = WeaponType;
        }
    }

    public UpgradeData GetUpgradeData(int id)
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
    
    public UpgradeData GetUpgradeData(UpgradeType upgrade)
    {
        UpgradeData data = Upgrades.Find(u => WeaponType + "_" + u.Name == upgrade.ToString());

        if (data == null)
        {
            Help.Debug(GetType(), "GetUpgradeData", $"Could not find upgrade : {upgrade} in the {WeaponType} Upgrade data scriptable object.");
            return null;
        }

        return data;
    }

    public float GetValue(int id)
    {
        return GetUpgradeData(id).Value;
    }
    
    public float GetValue(UpgradeType type)
    {
        return GetUpgradeData(type).Value;
    }

    public UpgradeType GetUpgradeAtID(int id)
    {
        Enum.TryParse(WeaponType + "_" + GetUpgradeData(id).Name, out UpgradeType upgrade);

        if (upgrade == 0)
        {
            Help.Debug(GetType(), "GetUpgradeAtID", "Enum parsing did not find correct enum - This is bad");
        }
        return upgrade;
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
}

[Serializable]
public class UpgradeData
{
    [Title("Upgrade", "$ID", TitleAlignments.Centered)]
    public string Name;
    public string DisplayName;
    public float Value;
    public Sprite Icon;
    public Sprite HudIcon;
    [TextArea(4,10)] public string Description;
    
    [GUIColor("grey"), ReadOnly, HorizontalGroup()] public int ID = 0;
    [GUIColor("grey"), ReadOnly, HorizontalGroup()] public WeaponType Type;
}
