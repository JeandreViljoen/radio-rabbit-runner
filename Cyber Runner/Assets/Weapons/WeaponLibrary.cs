using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponLibrary", menuName = "Custom Assets/Weapons/Weapon Library")]
public class WeaponLibrary : SerializedScriptableObject
{
    public List<WeaponUpgradeData> WeaponList;
    public List<PerkUpgradeData> PerkList;

    public WeaponUpgradeData GetWeaponData(WeaponType weapon)
    {
        WeaponUpgradeData data = WeaponList.Find(w => w.WeaponType == weapon);

        if (data == null)
        {
            Help.Debug(GetType(), "GetWeaponData", $"Could not find weapon upgrade data for weapon {weapon}. Make sure the weapon Upgrade Data scriptable object is listed in the weapon library.");
        }
        return data;
    }
    
    public PerkUpgradeData GetPerkData(PerkGroup group)
    {
        PerkUpgradeData data = PerkList.Find(p => p.GroupType == group);

        if (data == null)
        {
            Help.Debug(GetType(), "GetPerkData", $"Could not find perk upgrade data for perk {group}. Make sure the perk Upgrade Data scriptable object is listed in the weapon library.");
        }
        return data;
    }

#if UNITY_EDITOR
    [Button(ButtonSizes.Gigantic), GUIColor("blue")]
    public void RegenerateEnums()
    {
        //Get all tooltip names (to turn into enum entries)
        //List<TooltipObject> tooltipsList = GlobalLocalization.Instance.TooltipObjects;
        
        // Define temp holder objects
        List<string> upgradeNames = new List<string>();
        HashSet<string> uniqueUpgrades = new HashSet<string>();

        //for each object
        foreach (var weapon in WeaponList)
        {
            foreach (var upgrade in weapon.Upgrades)
            {
                
                //Error check to ensure type is valid
                if (upgrade.Name != null && upgrade.Name != "")
                {
                    //Add object to unique list - returns true or false
                    if (uniqueUpgrades.Add(weapon.WeaponType + "_" +  upgrade.Name))
                    {
                        //if its not in there already add o list.
                        upgradeNames.Add(weapon.WeaponType + "_" + upgrade.Name);
                    }
                    else
                    {
                        Help.Debug(GetType(), "RegenerateEnums", $"Duplicate upgrade type {weapon.WeaponType + "_" + upgrade.Name}. Not adding it to generated UpgradeType enum.");
                    }
                }
            }
        }

        string path = "Assets/Weapons/Upgrades/UpgradeType.cs";

        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.WriteLine("public enum UpgradeType");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\t" + "None,");
            for (int i = 0; i < upgradeNames.Count; i++)
            {
                streamWriter.WriteLine("\t" + upgradeNames[i] + ",");
            }

            streamWriter.WriteLine("}");
        }

        AssetDatabase.Refresh(); 
    }
    
    [Button(ButtonSizes.Gigantic), GUIColor("green")]
    public void RegenerateEnumsPerks()
    {
        RegeneratePerkGroupEnums();
        
        // Define temp holder objects
        List<string> perkNames = new List<string>();
        HashSet<string> uniquePerkNames = new HashSet<string>();

        //for each object
        foreach (var perk in PerkList)
        {
            foreach (var upgrade in perk.Upgrades)
            {
                
                //Error check to ensure type is valid
                if (!string.IsNullOrEmpty(perk.BaseEnumName))
                {
                    //Add object to unique list - returns true or false
                    if (uniquePerkNames.Add(perk.BaseEnumName + "_" +  upgrade.ID))
                    {
                        //if its not in there already add to list.
                        perkNames.Add(perk.BaseEnumName + "_" + upgrade.ID);
                    }
                    else
                    {
                        Help.Debug(GetType(), "RegenerateEnumsPerks", $"Duplicate upgrade type {perk.BaseEnumName + "_" +  upgrade.ID}. Not adding it to generated PerkType enum.");
                    }
                }
            }
        }
        
        string path = "Assets/Perks/Upgrades/PerkType.cs";

        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.WriteLine("public enum PerkType");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\t" + "None,");
            for (int i = 0; i < perkNames.Count; i++)
            {
                streamWriter.WriteLine("\t" + perkNames[i] + ",");
            }

            streamWriter.WriteLine("}");
        }

        AssetDatabase.Refresh(); 
    }
    
    public void RegeneratePerkGroupEnums()
    {
        
        List<string> perkGroups = new List<string>();
        HashSet<string> uniquePerkGroups = new HashSet<string>();
        
        //for each object
        foreach (var perk in PerkList)
        {
            //Error check to ensure type is valid
            if (!string.IsNullOrEmpty(perk.BaseEnumName))
            {
                //Add object to unique list - returns true or false
                if (uniquePerkGroups.Add(perk.BaseEnumName))
                {
                    //if its not in there already add to list.
                    perkGroups.Add(perk.BaseEnumName);
                }
                else
                {
                    Help.Debug(GetType(), "RegenerateEnumsPerks", $"Duplicate perk group {perk.BaseEnumName}. Not adding it to generated PerkGroup enum.");
                }
            }
        }
        
        string path = "Assets/Perks/Upgrades/PerkGroup.cs";

        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.WriteLine("public enum PerkGroup");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\t" + "None,");
            for (int i = 0; i < perkGroups.Count; i++)
            {
                streamWriter.WriteLine("\t" + perkGroups[i] + ",");
            }

            streamWriter.WriteLine("}");
        }

        AssetDatabase.Refresh();

       
    }
    
#endif
    
    [Button(ButtonSizes.Large), GUIColor("green")]
    public void RefreshScriptableObjectData()
    {
        foreach (var perk in PerkList)
        { 
            Enum.TryParse(perk.BaseEnumName, out PerkGroup result);
            if (result == PerkGroup.None)
            {
                Debug.LogError("Trying to refresh perk scriptable object data: Encountered a NONE type perk - THis shouldnt happen");
            }
            perk.ForceSetGroupType(result);
        }
    }
}

public enum WeaponType
{
    Minigun,
    RocketLauncher,
    Railgun,
    Lazer,
    Grenade,
    Drones,
    Boomerang,
    RecursionTree,
    Lightning,
    DVDPidgeon,
    ThrowingStars,
    Bow
}


