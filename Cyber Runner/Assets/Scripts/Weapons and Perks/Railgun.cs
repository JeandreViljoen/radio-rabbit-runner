using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Railgun : Weapon
{
    [Title("SPECIAL PROPERTIES")]
    private bool _doubleFireFlag = false;
    private float _doubleFireChance = 0;
    private bool _damageStack = false;
    private bool _splitBeam = false;
    private bool _recursiveSplit = false;
    
    
    protected override void UpgradesLogic(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            
            case UpgradeType.Railgun_Unlock:
                UnlockWeapon();
                break;
            case UpgradeType.Railgun_FireRate1:
                IncreaseFireRateMultiplicative(_upgradesData.GetValue(upgrade));
                break;
            case UpgradeType.Railgun_DoubleShot1:
                _doubleFireFlag = true;
                _doubleFireChance = _upgradesData.GetValue(upgrade) / 100;
                break;
            case UpgradeType.Railgun_FireRate2:
                IncreaseFireRateMultiplicative(_upgradesData.GetValue(upgrade));
                break;
            case UpgradeType.Railgun_DamageStack:
                _damageStack = true;
                break;
            case UpgradeType.Railgun_FireRate3:
                IncreaseFireRateMultiplicative(_upgradesData.GetValue(upgrade));
                break;
            case UpgradeType.Railgun_DoubleShot2:
                _doubleFireChance = _upgradesData.GetValue(upgrade) / 100;
                break;
            case UpgradeType.Railgun_SplitBeam:
                _splitBeam = true;
                break;
            case UpgradeType.Railgun_RecursiveSplit:
                _recursiveSplit = true;
                break;
            case UpgradeType.Railgun_UnlimitedPierce:
                //TODO COMBO
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

        ProjectileBase projectile = _prefabPool.Value.Get(ProjectilePrefab).GetComponent<ProjectileBase>();
        projectile.transform.parent = _projectileManager.Value.gameObject.transform;
        projectile.transform.position = SpawnPoint.position;
        projectile.Damage = Damage;
        projectile.Speed = ProjectileSpeed;
        projectile.Spread = Spread;
        projectile.TargetEntity = targetEntity;
        projectile.PierceCount = PierceCount;
        projectile.Knockback = KnockBack;
        
        projectile.Renderer.color = Color.white;

        InvokeOnFireEvent();
        _lastFireTime = Time.time;

        if (_doubleFireFlag)
        {
            float rng = UnityEngine.Random.Range(0f, 1f);

            if (rng <= _doubleFireChance)
            {
                StartCoroutine(DoubleFire());
            }
        }
        
        IEnumerator DoubleFire()
        {
            yield return new WaitForSeconds(0.1f );
            base.TryFire();
        }
       
    }
}
