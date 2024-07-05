using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [EnumToggleButtons]
    public TargetingType TargetType;
    public Transform SpawnPoint;

    public int Damage;
    public float ProjectileSpeed;
    public float FireRatePerSecond = 1;
    public GameObject ProjectilePrefab;

    public event Action OnFire;

    private LazyService<PrefabPool> _prefabPool;
    private LazyService<PlayerMovement> _player;

    private float _lastFireTime;
    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Time.deltaTime - _lastFireTime >= 1/FireRatePerSecond)
        {

            Fire();

        }
    }

    public GameObject GetTarget(TargetingType type)
    {
        GameObject target = null;
        
        switch (type)
        {
            case TargetingType.None:
                Help.Debug(GetType(), "GetTarget", $"{type} Targeting not yet implemented");
                break;
            case TargetingType.Closest:
                target = _player.Value.Targets.ClosestEnemy.gameObject;
                break;
            case TargetingType.Furthest:
                target = _player.Value.Targets.FurthestEnemy.gameObject;
                break;
            case TargetingType.HighestHealth:
                target = _player.Value.Targets.HighestHealth.gameObject;
                break;
            case TargetingType.LowestHealth:
                target = _player.Value.Targets.LowestHealth.gameObject;
                break;
            case TargetingType.Random:
                Help.Debug(GetType(), "GetTarget", $"{type} Targeting not yet implemented");
                break;
            case TargetingType.Direction:
                Help.Debug(GetType(), "GetTarget", $"{type} Targeting not yet implemented");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        return target;
    }


    private void Fire()
    {
        ProjectileBase projectile = _prefabPool.Value.Get(ProjectilePrefab).GetComponent<ProjectileBase>();
        projectile.transform.position = SpawnPoint.position;
        projectile.Damage = Damage;
        projectile.Speed = ProjectileSpeed;
        projectile.TargetEntity = GetTarget(TargetType);

    }
}
