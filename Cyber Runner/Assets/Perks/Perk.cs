using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class Perk
{
    public PerkGroup PerkGroup;
    protected bool _isMaxLevel = false;
    public int Level { get; private set; } = 0;
    public PerkUpgradeData Data;
    
    protected LazyService<UpgradesManager> _upgradesManager;

    public Perk(PerkUpgradeData data)
    {
        Data = data;
        PerkGroup = Data.GroupType;
    }
    
    public PerkType GetNextUpgrade()
    {
        if (_isMaxLevel)
        {
            return PerkType.None;
        }
        
        PerkType t = Data.GetUpgradeAtID(Level+1);
        return t;
    }
    
    public PerkType GetCurrentUpgrade()
    {
        PerkType t = Data.GetUpgradeAtID(Level);
        return t;
    }

    public void LevelUp()
    {
        if (_isMaxLevel) return;

        Level++;
        
        if (Level == Data.UpgradeCount)
        {
            _isMaxLevel = true;
        }
        
       _upgradesManager.Value.RegisterPerkUpgrade(Data.GetUpgradeAtID(Level));
    }
}

