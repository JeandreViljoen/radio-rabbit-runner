using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Services;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Weapon : SerializedMonoBehaviour
{
    [Title("WEAPON", "$GetSubtitle", TitleAlignments.Centered)]

    [SerializeField, GUIColor("blue")] protected bool _showGizmos = false;

    [EnumToggleButtons, GUIColor("@GetTitleColor(TargetType)")] public TargetingType TargetType;
    [GUIColor("grey")]public Transform SpawnPoint;

    public WeaponType Type;
    public int Level = 0;
    public int Damage;
    public int PierceCount = 0;
    public float ProjectileSpeed;
    public float FireRatePerSecond = 1;
    public int Spread = 0;
    protected bool _isMaxLevel = false;
    [OdinSerialize] public bool IsUnlocked { get; private set; } = false;
    

    [SerializeField]private bool _useCullingDistanceAsRange = true;

    protected float _range = 1f;

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
    [GUIColor("grey")] public GameObject ProjectilePrefab;

    public event Action<WeaponType> OnFire;

    protected LazyService<PrefabPool> _prefabPool;
    protected LazyService<PlayerController> _player;
    protected LazyService<ProjectileManager> _projectileManager;
    protected LazyService<UpgradesManager> _upgradesManager;

    protected WeaponUpgradeData _upgradesData;

    protected float _lastFireTime = 0;
    private void Awake()
    {
        
    }

    void Start()
    {
        Init();
        RegisterUpgradeEffects();
    }

    private void RegisterUpgradeEffects()
    {
        _upgradesManager.Value.OnUpgradeActivated += UpgradesLogic;
    }
    
    protected virtual void UpgradesLogic(UpgradeType upgrade)
    {
        //Each weapon extending from this class needs to implement its own upgrade logic.
        return;
    }

    private void Init()
    {
        _upgradesData = _upgradesManager.Value.GetWeaponUpgradeData(Type);
        RegisterSelfToUpgradesManager();
    }

    void RegisterSelfToUpgradesManager()
    {
        _upgradesManager.Value.RegisterWeaponOnStart(this);
    }

    
    protected void Update()
    {
        if (!IsUnlocked)
        {
            return;
        }
        
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

    protected void InvokeOnFireEvent()
    {
        OnFire?.Invoke(Type);
    }
    
    protected void ReduceSpread(float percent)
    {
        Spread = (int)(Spread * (percent / 100f));
    }

    protected void IncreaseFireRateMultiplicative(float percent)
    {
        FireRatePerSecond *= 1 + (percent / 100f);
    }

    protected void IncreaseDamageAdditive(int damage)
    {
        Damage += damage;
    }
    
    protected void IncreasePierceCount(int pierce)
    {
        PierceCount += pierce;
    }


    protected virtual void TryFire()
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
        _lastFireTime = Time.time;
    }
    
    protected virtual void TryFireOverride(int damage, float projectileSpeed, int spread, int pierceCount)
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
        projectile.Damage = damage;
        projectile.Speed = projectileSpeed;
        projectile.Spread = spread;
        projectile.TargetEntity = targetEntity;
        projectile.PierceCount = pierceCount;
        
        projectile.Renderer.color = Color.grey;

        InvokeOnFireEvent();
        _lastFireTime = Time.time;
    }

    public void LevelUp()
    {
        if (_upgradesData == null)
        {
            _upgradesData = _upgradesManager.Value.GetWeaponUpgradeData(Type);
        }
        //Max level reached
        if (_upgradesData.UpgradeCount == Level+1)
        {
            _isMaxLevel = true;
            //Unlock weapon combo options
            _upgradesManager.Value.AddWeaponToComboDrafts(Type);
            return;
        }
        
        Level++;
        _upgradesManager.Value.RegisterUpgrade(_upgradesData.GetUpgradeAtID(Level));
        Debug.Log($"Upgraded {Type} with {_upgradesData.GetUpgradeData(Level).DisplayName}");

    }
    
#region HELPERS
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

    public UpgradeType GetNextUpgrade()
    {
        UpgradeType t = _upgradesData.GetUpgradeAtID(Level+1);
        return t;
    }

    protected void UnlockWeapon()
    {
        IsUnlocked = true;
    }

    private string GetSubtitle => $"{Type} - {TargetType}";
#endregion

}

