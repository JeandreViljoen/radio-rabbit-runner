using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Services;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class Weapon : SerializedMonoBehaviour
{
    [Title("WEAPON", "$TargetType", TitleAlignments.Centered)]

    [SerializeField, GUIColor("blue")] private bool _showGizmos = false;

    [EnumToggleButtons, GUIColor("@GetTitleColor(TargetType)")] public TargetingType TargetType;
    [GUIColor("grey")]public Transform SpawnPoint;

    public int Level = 1;
    public int Damage;
    public int PierceCount = 0;
    public float ProjectileSpeed;
    public float FireRatePerSecond = 1;

    [SerializeField]private bool _useCullingDistanceAsRange = true;

    private float _range = 1f;

    [HideIf("_useCullingDistanceAsRange"), OdinSerialize, ShowInInspector]
    public float Range
    {
        get
        {
            if (_useCullingDistanceAsRange)
            {
                if (!_projectileManager.HasService())
                {
                    return 0f;
                }
                return _projectileManager.Value.CullDistance;
            }
            
            return _range;
        }
        private set => _range = value;
    }
    [GUIColor("grey")]public GameObject ProjectilePrefab;

    public event Action OnFire;

    private LazyService<PrefabPool> _prefabPool;
    private LazyService<PlayerMovement> _player;
    private LazyService<ProjectileManager> _projectileManager;

    private float _lastFireTime = 0;
    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    

    
    void Update()
    {
        if (Time.time - _lastFireTime >= 1/FireRatePerSecond)
        {
            TryFire();
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
                if (_player.Value.Targets.ClosestEnemy != null)
                    target = _player.Value.Targets.ClosestEnemy.gameObject;
                break;
            case TargetingType.Furthest:
                if (_player.Value.Targets.FurthestEnemy != null)
                    target = _player.Value.Targets.FurthestEnemy.gameObject;
                break;
            case TargetingType.HighestHealth:
                if (_player.Value.Targets.HighestHealth != null)
                    target = _player.Value.Targets.HighestHealth.gameObject;
                break;
            case TargetingType.LowestHealth:
                if (_player.Value.Targets.LowestHealth != null)
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


    private void TryFire()
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
        projectile.TargetEntity = targetEntity;
        projectile.PierceCount = PierceCount;
        
        projectile.Renderer.color = Help.GetColorBasedOnTargetType(TargetType);

        OnFire?.Invoke();
        _lastFireTime = Time.time;
    }
    
    void OnDrawGizmos()
    {
        if (!_showGizmos)
        {
            return;
        }
        
        if (Application.isPlaying)
        {
            GameObject targetEntity = GetTarget(TargetType);
    
            if (targetEntity == null)
            {
                return;
            }
            
            Gizmos.color = Help.GetColorBasedOnTargetType(TargetType);
            Gizmos.DrawLine(SpawnPoint.transform.position,targetEntity.transform.position);
            Help.DrawGizmoCircle(SpawnPoint.transform.position, Range);
        }
    }
    
    [UsedImplicitly]
    static Color GetTitleColor(TargetingType type)
    {
        Color c = Help.GetColorBasedOnTargetType(type);
      
        return new Color(c.r, c.g, c.b, 1f);
    }
}