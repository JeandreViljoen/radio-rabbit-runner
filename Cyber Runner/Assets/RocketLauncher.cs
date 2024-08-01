using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class RocketLauncher : Weapon
{
    protected override void UpgradesLogic(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.RocketLauncher_Unlock:
                UnlockWeapon();
                break;
            case UpgradeType.RocketLauncher_FireRate1:
                IncreaseFireRateMultiplicative(_upgradesData.GetValue(upgrade));
                break;
            case UpgradeType.RocketLauncher_IncreaseDamage1:
                IncreaseDamageMultiplicative((int)_upgradesData.GetValue(upgrade));
                break;
            case UpgradeType.RocketLauncher_BiggerExplosion1:
                break;
            case UpgradeType.RocketLauncher_SecondExplosion:
                break;
            case UpgradeType.RocketLauncher_IncreaseDamage2:
                IncreaseDamageMultiplicative((int)_upgradesData.GetValue(upgrade));
                break;
            case UpgradeType.RocketLauncher_BiggerExplosion2:
                break;
            case UpgradeType.RocketLauncher_FireRate2:
                IncreaseFireRateMultiplicative(_upgradesData.GetValue(upgrade));
                break;
            case UpgradeType.RocketLauncher_MiniExplosions:
                break;
            case UpgradeType.RocketLauncher_ExplosiveRounds:
                //TODO
                break;
            default:
                break;
        }
    }
    
}
