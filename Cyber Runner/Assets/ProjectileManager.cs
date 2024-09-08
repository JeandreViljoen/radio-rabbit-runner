using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class ProjectileManager : MonoService
{
    [SerializeField, GUIColor("grey")]private bool _showGizmos = false;
    [OdinSerialize, ShowInInspector, GUIColor("red")] public float CullDistance { get; private set; }

    private LazyService<PrefabPool> _prefabPool;

    void OnDrawGizmos()
    {

        if (!_showGizmos)
        {
            return;
        }
        
        Gizmos.color = Color.white;
        Help.DrawGizmoCircle(transform.position, CullDistance);
    }

    public void SpawnMinigunProjectile(Vector3 spawnPos, int damage, float speed, int spread, Vector2 direction, int pierceCount, Color color)
    {
        GameObject projectilePrefab = ServiceLocator.GetService<UpgradesManager>().GetWeaponInstance(WeaponType.Minigun)
            .ProjectilePrefab;
        
        ProjectileBase projectile = _prefabPool.Value.Get(projectilePrefab).GetComponent<ProjectileBase>();
        projectile.transform.parent = gameObject.transform;
        projectile.transform.position = spawnPos;
        projectile.Damage = damage;
        projectile.Speed = speed;
        projectile.Spread = spread;
        projectile.OverrideDirectionTarget(direction);
        projectile.PierceCount = pierceCount;
    }

    public void SpawnMinigunSpiral(int bulletCount, Vector3 spawnPos, int damage, float speed, int pierceCount, Color color)
    {
        int directionDelta = 360 / bulletCount;

        StartCoroutine(DelayedSpawn());
        
        IEnumerator DelayedSpawn()
        {
            for (int i = 0; i < bulletCount; i++)
            {
                Vector2 direction = Quaternion.AngleAxis((i * directionDelta), Vector3.forward) * Vector2.right;
                SpawnMinigunProjectile(spawnPos, damage, speed, 0, direction, pierceCount, color);

                yield return new WaitForSeconds(1/bulletCount);
            }
        }

        
    }
    
    public void SpawnRailgunProjectile(Vector3 spawnPos, int damage, float speed, int spread, Vector2 direction, int pierceCount, Color color)
    {
        GameObject projectilePrefab = ServiceLocator.GetService<UpgradesManager>().GetWeaponInstance(WeaponType.Railgun)
            .ProjectilePrefab;

        
        
        ProjectileBase projectile = _prefabPool.Value.Get(projectilePrefab).GetComponent<ProjectileBase>();
        projectile.transform.parent = gameObject.transform;
        projectile.transform.position = spawnPos;
        projectile.Damage = damage;
        projectile.Speed = speed;
        projectile.Spread = spread;
        projectile.OverrideDirectionTarget(direction);
        projectile.PierceCount = pierceCount;
    }
    
    public GameObject ExplosionPrefab;
    public void SpawnExplosion(Vector3 position ,int damage, float sizeMultiplier = 1f, float knockback = 0f)
    {
        Explosion exp = _prefabPool.Value.Get(ExplosionPrefab).GetComponent<Explosion>();
        exp.Damage = damage;
        exp.Knockback = knockback;
        exp.transform.localScale *= sizeMultiplier;
        exp.transform.position = position;
        exp.DoExplosion();
    }
    
    public void SpawnDelayedExplosion(float delay, Vector3 position ,int damage, float sizeMultiplier = 1f, float knockback = 0f)
    {
        StartCoroutine(Explode());
        IEnumerator Explode()
        {
            yield return new WaitForSeconds(delay);
            SpawnExplosion(position, damage, sizeMultiplier, knockback);
        }
    }

    public LightningProjectile SpawnLightningNode(GameObject targetEntity, int damage, int bounces, float stunChance, float stunDuration)
    {
        if (bounces == 0)
        {
            Debug.Log($"Lightning | {targetEntity.name} -> END");
            return null;
        }
        
        GameObject ProjectilePrefab = ServiceLocator.GetService<UpgradesManager>().GetWeaponInstance(WeaponType.Lightning)
            .ProjectilePrefab;
       
        
        LightningProjectile projectile = _prefabPool.Value.Get(ProjectilePrefab).GetComponent<LightningProjectile>();

        projectile.transform.parent = gameObject.transform;
        projectile.transform.position = targetEntity.transform.position;
        projectile.Damage = damage;
        projectile.TargetEntity = targetEntity;
        projectile.Bounces = bounces;
        projectile.StunChance = stunChance;
        projectile.StunDuration = stunDuration;
        
        projectile.DoFire();
        return projectile;

    }

}
