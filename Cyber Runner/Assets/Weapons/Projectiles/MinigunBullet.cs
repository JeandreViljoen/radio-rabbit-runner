using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class MinigunBullet : ProjectileBase
{
    [SerializeField] private int TargetSplitAmount = 60;
    
    public override void DoOnKillEffects()
    {
        //Bullet Split effect
        if (ServiceLocator.GetService<UpgradesManager>().HasUpgrade(UpgradeType.Minigun_BulletSplit))
        {
            Vector2 directionOverride = Quaternion.AngleAxis(TargetSplitAmount/2, Vector3.forward) * _direction;
            _projectileManager.Value.SpawnMinigunProjectile(transform.position, Damage/2, Speed, 0, directionOverride, PierceCount, Color.black);
        
            directionOverride = Quaternion.AngleAxis(-TargetSplitAmount/2, Vector3.forward) * _direction;
            _projectileManager.Value.SpawnMinigunProjectile(transform.position, Damage/2, Speed, 0, directionOverride, PierceCount, Color.black);
        }
        
        _projectileManager.Value.SpawnMinigunSpiral(8,transform.position, 1,Speed,PierceCount, Color.blue);
        
        
    }
}
