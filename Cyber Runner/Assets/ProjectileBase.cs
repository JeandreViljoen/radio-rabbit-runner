using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Services;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = System.Random;

public enum ProjectileDetectionType
{
    Collision,
    Raycast,
    Proximity
}

public class ProjectileBase : MonoBehaviour
{
    
    [EnumToggleButtons]
    public ProjectileType Type;

    [EnumToggleButtons]
    public ProjectileDetectionType DetectionType;

    private Vector2 _lastPosition = Vector2.zero;

    [HideInInspector] public int PierceCount = 0;
    private bool _inPool = false;

    [SerializeField, CanBeNull] private Collider2D _collider;

    //Used to store already hit entities so that they don't take multiple instances of damage with wacky raycasts
    HashSet<string> _raycastHitBuffers = new HashSet<string>();

    private LazyService<UpgradesManager> _upgradesManager;
    private LazyService<PlayerController> _player;
    private LazyService<VFXManager> _vfx;
    
    

    private GameObject _targetEntity;
    public GameObject TargetEntity
    {
        get
        {
            return _targetEntity;
        }

        set
        {
            _targetEntity = value;

            if (_targetEntity == null)
            {
                TargetLocation = Vector2.zero;
            }
            else
            {
                TargetLocation = value.transform.localPosition;
                _direction = (TargetLocation - transform.localPosition).normalized;
                _direction = Quaternion.AngleAxis(UnityEngine.Random.Range(-Spread/2, Spread/2), Vector3.forward) * _direction;

                if (AutoRotate)
                {
                    //Vector3 _rotation = Quaternion.AngleAxis(Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg, Vector3.forward) * _direction;
                  
                    
                    Quaternion targetRotation;
                    var zAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                    // Store the target rotation
                    targetRotation = Quaternion.Euler(0,0, zAngle);


                    VisualsParent.transform.localRotation = targetRotation;
                }
            }
        }
    }

    private void Awake()
    {
        if(_collider == null) _collider = GetComponent<Collider2D>();

        if (_collider != null && DetectionType == ProjectileDetectionType.Raycast) _collider.enabled = false;
    }

    protected virtual void OnEnable()
    {
        _raycastHitBuffers.Clear();
        _inPool = false;
    }

    [HideInInspector] public float Speed;
    [HideInInspector] public int Damage;
    [HideInInspector] public int Spread = 0;
    [HideInInspector] public float Knockback = 1;
    [ShowIf("IsHoming")] public float Acceleration = 1f;

    private bool IsHoming()
    {
        if (Type == ProjectileType.Homing)
        {
            return true;
        }

        return false;
    }
    
    [HideInInspector]  public Vector3 TargetLocation { get; private set; }
    protected Vector2 _direction = Vector2.zero;
    protected LazyService<PrefabPool> _prefabPool;
    protected LazyService<ProjectileManager> _projectileManager;

    public bool AutoRotate = true;
    [FoldoutGroup("References")] public SpriteRenderer Renderer;
    [FoldoutGroup("References")] public GameObject VisualsParent;
    [FoldoutGroup("References")] public ParticleSystem Particles;

    protected void FixedUpdate()
    {
        if (DetectionType == ProjectileDetectionType.Raycast)
        {
            transform.localPosition += (Vector3) (_direction * Speed * Time.deltaTime);
            if (_lastPosition == Vector2.zero)
            {
                _lastPosition = transform.position;
                return;
            }

            //Sets collider to only check for Enemies
            int bitmask = (1 << 10);

            Vector2 currentPosition = transform.position;
            
            RaycastHit2D[] hits = Physics2D.RaycastAll( _lastPosition,(currentPosition - _lastPosition).normalized ,  Vector2.Distance(currentPosition , _lastPosition) , bitmask);
            _lastPosition = currentPosition;
            //Debug.DrawRay(transform.position, (currentPosition - _lastPosition).normalized *  Vector2.Distance(currentPosition , _lastPosition), Color.red, 0.1f);
            foreach (var hit in hits)
            {
                CompareAndApplyDamage(hit.collider);
            }
        }
    }

    protected void Update()
    {

        switch (Type)
        {
            case ProjectileType.Bullet:
                DoFire();
                break;
            case ProjectileType.Homing:
                if (TargetEntity == null)
                {
                    return;
                }
                UpdateTargets();
                DoFire();
                break;
            case ProjectileType.Direction:
                DoFire();
                break;
            case ProjectileType.Ray:

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    public void OverrideDirectionTarget(Vector2 direction)
    {
        _direction = direction.normalized;
        
        Quaternion targetRotation;
        var zAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        // Store the target rotation
        targetRotation = Quaternion.Euler(0,0, zAngle);


        VisualsParent.transform.localRotation = targetRotation;
    }

    private void UpdateTargets()
    {
        switch (Type)
        {
            case ProjectileType.Bullet:
                break;
            case ProjectileType.Homing:

                PlayerController player = ServiceLocator.GetService<PlayerController>();
                Weapon rocketLauncher = ServiceLocator.GetService<UpgradesManager>()
                    .GetWeaponInstance(WeaponType.RocketLauncher);
                Enemy lastTarget = player.Targets.GetTarget(rocketLauncher.TargetType);
                TargetEntity =  lastTarget != null ? lastTarget.gameObject : TargetEntity;
                break;
            case ProjectileType.Ray:
                VisualsParent.transform.position = TargetLocation;
                break;
            case ProjectileType.Proximity:
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual void DoImpact()
    {
        if (_inPool) return;
        
        PierceCount--;
        if (PierceCount < 0)
        {
            _lastPosition = Vector2.zero;
            _targetEntity = null;
            _prefabPool.Value.Return(gameObject);
            PierceCount = 0;
            _inPool = true;
        }
    }
    

    public void DoFire()
    {
        //Fire Range
        if (Vector2.Distance(transform.localPosition, transform.parent.localPosition) >= _projectileManager.Value.CullDistance && Type != ProjectileType.Proximity)
        {
            DoImpact();
            return;
        }

        switch (Type)
        {
            case ProjectileType.Bullet:
                transform.localPosition += (Vector3) (_direction * Speed * Time.deltaTime);
                break;
            //case ProjectileType.Direction:
                //transform.localPosition += (Vector3) (_direction * Speed * Time.deltaTime);
                //break;
            case ProjectileType.Homing:
                Speed *= Acceleration;
                transform.localPosition += (Vector3) (_direction * Speed * Time.deltaTime);
                break;
            case ProjectileType.Ray:

                int bitmask = (1 << 10);
                RaycastHit2D[] hits = Physics2D.RaycastAll( transform.position, _direction , _projectileManager.Value.CullDistance*2f, bitmask);
                Debug.DrawRay(transform.position, _direction*_projectileManager.Value.CullDistance, Color.red, 2f);
                foreach (var hit in hits)
                {
                    CompareAndApplyDamage(hit.collider);
                }
                break;
            case ProjectileType.Proximity:

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (DetectionType == ProjectileDetectionType.Raycast) return;
        
       CompareAndApplyDamage(col);
    }

    protected void CompareAndApplyDamage(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            _vfx.Value.OnHitRailgun(col.transform,_direction);
            AudioManager.PostEvent(AudioEvent.ENEMY_IMPACT_HOLLOW, col.gameObject);
            
            if (_raycastHitBuffers.Add(col.gameObject.GetInstanceID().ToString()))
            {
                int modifiedDamage = Damage;

                if (_upgradesManager.Value.HasPerkGroup(PerkGroup.IncreaseDamageFromMissingHealth, out float value))
                {
                    int healthDifference = _player.Value.Health.MaxHealth - _player.Value.Health.CurrentHealth;
                    float damageIncrease = Help.PercentToMultiplier(value * healthDifference);
                    
                    modifiedDamage = (int)Math.Round(modifiedDamage * damageIncrease, MidpointRounding.AwayFromZero);
                    
                    Debug.Log($"Base damage: {Damage}          |      Modified:    {modifiedDamage}");
                }
                
                if ( col.gameObject.GetComponent<Enemy>().Health.RemoveHealth(modifiedDamage))
                {
                    DoOnKillEffects();
                }
                col.gameObject.GetComponent<Enemy>().ApplyKnockback(_direction, Knockback);
                

                DoOnHitEffects();
                DoImpact();
            }
        }
    }

    public virtual void DoOnKillEffects()
    {
        
    }
    
    public virtual void DoOnHitEffects()
    {
        
    }
}

public enum ProjectileType
{
    Bullet,
    Rocket,
    Direction,
    Homing,
    Ray,
    Proximity

}
