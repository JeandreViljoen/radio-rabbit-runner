using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Lazer : Weapon
{
    [SerializeField]private GameObject _trailRendererPrefab;
    [SerializeField] private float _trailRenderSpeed = 0.2f;
    
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
        
        projectile.Renderer.color = Help.GetColorBasedOnTargetType(TargetType);

        InvokeOnFireEvent();
        projectile.DoFire();
        _lastFireTime = Time.time;

        GameObject trail = _prefabPool.Value.Get(_trailRendererPrefab);
        trail.transform.parent = _projectileManager.Value.gameObject.transform;
        trail.transform.position = SpawnPoint.position;

        Sequence s = DOTween.Sequence();
        
        s.Append(trail.transform.DOLocalMove(projectile.TargetLocation, _trailRenderSpeed));
        s.AppendCallback(() => { _prefabPool.Value.Return(trail.gameObject); });

    }
}
