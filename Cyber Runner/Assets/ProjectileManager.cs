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

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
    
    void OnDrawGizmos()
    {

        if (!_showGizmos)
        {
            return;
        }

        //if (Application.isPlaying)
        //{
            Gizmos.color = Color.white;
            Help.DrawGizmoCircle(transform.position, CullDistance);
        //}
        
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
        //projectile.TargetEntity = targetEntity;
        projectile.PierceCount = pierceCount;
        
        projectile.Renderer.color = color;
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

}
