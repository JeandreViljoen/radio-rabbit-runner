using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUpgradeData", menuName = "Custom Assets/Weapons/Weapon Upgrade Data")]

public class WeaponUpgradeData : SerializedScriptableObject
{
    [OdinSerialize, PropertyOrder(1)] public WeaponType WeaponType { get; private set; }
    [SerializeField, PropertyOrder(2)] public List<UpgradeData> Upgrades;
    public int UpgradeCount => Upgrades.Count;


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
        UpgradeData data = Upgrades.Find(u => u.Name == upgrade.ToString());

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

    public UpgradeType GetUpgradeAtID(int id)
    {
        Enum.TryParse(WeaponType + GetUpgradeData(id).Name, out UpgradeType upgrade);

        if (upgrade == 0)
        {
            Help.Debug(GetType(), "GetUpgradeAtID", "Enum parsing did not find correct enum - This is bad");
        }
        return upgrade;
    }
}

[Serializable]
public class UpgradeData
{
    [Title("Upgrade", "$ID" , TitleAlignments.Centered)] 
    [GUIColor("grey")] public int ID = 0;
    public string Name;
    public string DisplayName;
    public float Value;
    [TextArea(4,10)] public string Description;

}
