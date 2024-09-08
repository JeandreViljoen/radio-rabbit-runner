using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LightningGun : Weapon
{
    [Title("SPECIAL PROPERTIES")]
    
    public TargetingType SecondaryTargetType;
    public int Bounces = 2;
    public float StunChance = 0.2f;
    public float ArcDelay = 0.3f;
    public float StunDuration;
    public float AlreadyElectrocutedTimer;
    
    protected override void UpgradesLogic(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Lightning_Unlock:
                UnlockWeapon();
                break;
            case UpgradeType.Lightning_BounceAmount1:
                Bounces += (int)_upgradesData.GetValue(upgrade);
                break;
            case UpgradeType.Lightning_IncreaseFireRate1:
                IncreaseFireRateMultiplicative(_upgradesData.GetValue(upgrade));
                break;
            case UpgradeType.Lightning_IncreaseStunChance1:
                StunChance += _upgradesData.GetValue(upgrade)/100;
                break;
            case UpgradeType.Lightning_DamageIncreasePerBounce:
                break;
            case UpgradeType.Lightning_IncreaseStunChance2:
                StunChance += _upgradesData.GetValue(upgrade)/100;
                break;
            case UpgradeType.Lightning_BounceAmount2:
                Bounces += (int)_upgradesData.GetValue(upgrade);
                break;
            case UpgradeType.Lightning_IncreaseDamage:
                IncreaseDamageMultiplicative((int)_upgradesData.GetValue(upgrade));
                break;
            case UpgradeType.Lightning_MultipleTargets:
                break;
            case UpgradeType.Lightning_ChanceToFork:
                break;
            default:
                return;
        }
    }

    protected override void TryFire()
    {
        GameObject targetEntity = GetTarget(TargetType);

        //Do not shoot if no target present
        if (targetEntity == null)
        {
            return;
        }
    
        //Do not shoot if target is not close enough (Barely on screen)
        if (Vector2.Distance(targetEntity.transform.position, transform.parent.position) >= Range)
        {
            return;
        }
        
       LightningProjectile p = _projectileManager.Value.SpawnLightningNode(targetEntity, Damage, Bounces, StunChance, StunDuration);
       p.DoLightningVFXStart(targetEntity.GetComponent<Enemy>(), SpawnPoint.gameObject.transform);

       InvokeOnFireEvent();
        _lastFireTime = Time.time;
    }

}
