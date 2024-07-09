using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunBullet : ProjectileBase
{
    public override void DoOnKillEffects()
    {
        // ProjectileBase projectile = _prefabPool.Value.Get(ProjectilePrefab).GetComponent<ProjectileBase>();
        // projectile.transform.parent = _projectileManager.Value.gameObject.transform;
        // projectile.transform.position = SpawnPoint.position;
        // projectile.Damage = Damage;
        // projectile.Speed = ProjectileSpeed;
        // projectile.Spread = Spread;
        // projectile.TargetEntity = targetEntity;
        // projectile.PierceCount = PierceCount;
        //
        // projectile.Renderer.color = Help.GetColorBasedOnTargetType(TargetType);
    }
}
