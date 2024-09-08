using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class RocketLauncherBullet : ProjectileBase
{
   private LazyService<UpgradesManager> _upgradesManager;

   public override void DoOnHitEffects()
   {
      Vector3 pos = new Vector3();
      pos = transform.position;
      
      float scale = 1f;
      if (_upgradesManager.Value.HasUpgrade(UpgradeType.RocketLauncher_BiggerExplosion1))
      {
         scale *= (1 + (_upgradesManager.Value.GetUpgradeData(UpgradeType.RocketLauncher_BiggerExplosion1).Value / 100));
         
         if (_upgradesManager.Value.HasUpgrade(UpgradeType.RocketLauncher_BiggerExplosion2))
         {
            scale *= (1 + (_upgradesManager.Value.GetUpgradeData(UpgradeType.RocketLauncher_BiggerExplosion2).Value / 100));
         }
      }
      
      _projectileManager.Value.SpawnExplosion(pos, Damage, scale, Knockback);

      if (_upgradesManager.Value.HasUpgrade(UpgradeType.RocketLauncher_SecondExplosion))
      {

         var data = _upgradesManager.Value.GetUpgradeData(UpgradeType.RocketLauncher_SecondExplosion);
         int rng = Random.Range(0, 101);

         if (rng <= data.Value)
         {
            _projectileManager.Value.SpawnDelayedExplosion(0.2f,pos, Damage/2, scale*1.3f, Knockback/2);
           
         }
      }
   }

}
