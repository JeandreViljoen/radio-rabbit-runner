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

    public WeaponUpgradeData GetWeaponData(WeaponType weapon)
    {
        WeaponUpgradeData data = WeaponList.Find(w => w.WeaponType == weapon);

        if (data == null)
        {
            Help.Debug(GetType(), "GetWeaponData", $"Could not find weapon upgrade data for weapon {weapon}. Make sure the weapon Upgrade Data scriptable object is listed in the weapon library.");
        }
        return data;
    }

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
                    if (uniqueUpgrades.Add(weapon.WeaponType + upgrade.Name))
                    {
                        //if its not in there already add o list.
                        upgradeNames.Add(weapon.WeaponType + upgrade.Name);
                    }
                    else
                    {
                        Help.Debug(GetType(), "RegenerateEnums", $"Duplicate upgrade type {weapon.WeaponType + upgrade.Name}. Not adding it to generated UpgradeType enum.");
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