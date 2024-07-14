using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class RailgunBullet : ProjectileBase
{
    private int _stackAmount = 0;
    [SerializeField] private float RecursiveSplitSpeedModifier = 40;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _stackAmount = 0;
    }


    public override void DoOnHitEffects()
    {
        if (ServiceLocator.GetService<UpgradesManager>().HasUpgrade(UpgradeType.Railgun_DamageStack))
        {
           UpgradeData data = ServiceLocator.GetService<UpgradesManager>().GetUpgradeData(UpgradeType.Railgun_DamageStack);
           Damage += (int)data.Value;
           _stackAmount++;
        }
    }

    public override void DoOnKillEffects()
    {
        if (ServiceLocator.GetService<UpgradesManager>().HasUpgrade(UpgradeType.Railgun_RecursiveSplit))
        {

            Vector2 leftDirection = Quaternion.AngleAxis(-90, Vector3.forward) * _direction;
            Vector2 rightDirection = Quaternion.AngleAxis(90, Vector3.forward) * _direction;

            _projectileManager.Value.SpawnRailgunProjectile(transform.position,
                (int) Damage, RecursiveSplitSpeedModifier, Spread, leftDirection, PierceCount,
                Color.black);
            _projectileManager.Value.SpawnRailgunProjectile(transform.position,
                (int) Damage, RecursiveSplitSpeedModifier, Spread, rightDirection, PierceCount,
                Color.black);
        }
    }
}
