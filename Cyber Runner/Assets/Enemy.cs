using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;

public enum EnemyState
{
    Active,
    Dead
}

[RequireComponent(typeof(ProjectileReceptor))]
public class Enemy : MonoBehaviour
{

    public Vector2 Target;
    public float MoveSpeed = 1;
    private float additionalMoveSpeed = 0;
    public float MomentumMultiplier = 2;
    public Health Health;
    [SerializeField] private int _expValue;
    public SpriteRenderer Renderer;
    private ProjectileReceptor _projectileReceptor;
    public float DistanceFromPlayer { get; private set; }

    private int _calculationFrameSkips = 10;
    private int _frameSkipCounter = 0;

    private LazyService<PlayerMovement> _player;
    private LazyService<EXPManager> _expManager;

    public EnemyState State = EnemyState.Active;

    private void Awake()
    {
        _projectileReceptor = GetComponent<ProjectileReceptor>();
        Debug.Assert(_projectileReceptor != null, $"Could not find Projectile Receptor on Enemy {name} during Awake method.");
    }

    void Start()
    {
        Health.OnHealthZero += StartOnDeathBehavior;

        _projectileReceptor.OnHit += OnHit;
    }

    private void OnHit(ProjectileBase projectile)
    {
        Health.RemoveHealth(projectile.Damage); 
        projectile.DoImpact();
    }

    private void OnEnable()
    {
        gameObject.layer = 10;
        Renderer.color = Color.white;
        State = EnemyState.Active;
        _frameSkipCounter = 0;
        Health.InitHealth();
        //CheckMaxHealthTargeting();
    }

    private void CheckMaxHealthTargeting()
    {
        Targets targets = ServiceLocator.GetService<PlayerMovement>().Targets;
        if (targets.HighestHealth == null)
        {
            targets.SetHighestHealthEnemy(this);
            return;
        }

        if (targets.LowestHealth == null)
        {
            targets.SetLowestHealthEnemy(this);
            return;
        }

        if (Health.MaxHealth > targets.HighestHealth.Health.MaxHealth)
        {
            targets.SetHighestHealthEnemy(this);
        }
        
        if (Health.MaxHealth < targets.LowestHealth.Health.MaxHealth)
        {
            targets.SetLowestHealthEnemy(this);
        }
    }

    public void Update()
    {
        if (_frameSkipCounter <= _calculationFrameSkips)
        {
            _frameSkipCounter++;
        }
        else
        {
            _frameSkipCounter = 0;
            CalculateDistances();
        }
    }

    private void CalculateDistances()
    {
        if (State != EnemyState.Active)
        {
            return;
        }
        
        Targets targets = _player.Value.Targets;
        
        if (DistanceFromPlayer < targets.HighestHealthTargetOuterRange)
        {
            CheckMaxHealthTargeting();
        }
        
        DistanceFromPlayer = Vector2.Distance(transform.position, _player.Value.transform.position);
        if (targets.ClosestEnemy == null)
        {
            targets.SetClosestEnemy(this);
            return;
        }
        if (targets.FurthestEnemy == null)
        {
            targets.SetFurthestEnemy(this);
            return;
        }
        
        if (DistanceFromPlayer < targets.ClosestEnemy.DistanceFromPlayer)
        {
            targets.SetClosestEnemy(this);
        }
        if (DistanceFromPlayer > targets.FurthestEnemy.DistanceFromPlayer && DistanceFromPlayer < targets.FurthestTargetOuterRange && DistanceFromPlayer > targets.FurthestTargetInnerRange)
        {
            targets.SetFurthestEnemy(this);
        }

    }

    void FixedUpdate()
    {
        if (State == EnemyState.Dead )
        {
            //transform.position =
                //Vector2.MoveTowards(this.transform.position, _player.Value.transform.position, (MoveSpeed +additionalMoveSpeed) * Time.deltaTime);
        }
        else
        {
            additionalMoveSpeed = (_player.Value.SpeedDelta * -1 / _player.Value.TheoreticalMaxSpeed) * MomentumMultiplier;
        
            //Vector2 targetUnitVector = (_player.Value.transform.position - transform.position);
            transform.position =
                Vector2.MoveTowards(this.transform.position, _player.Value.transform.position, (MoveSpeed + Math.Abs(additionalMoveSpeed)) * Time.deltaTime);
        }
      
    }

    private void ReturnToPool()
    {
        ServiceLocator.GetService<PrefabPool>().Return(gameObject);
    }

    private void StartOnDeathBehavior()
    {
        
        
        State = EnemyState.Dead;
        StartCoroutine(OnDeathBehavior(0.3f));
    }

    IEnumerator OnDeathBehavior (float preDeathTime)
    {
        _expManager.Value.AddEXP(_expValue);
        gameObject.layer = 11;
        transform.parent = null;
        ClearTargets();
        Tween t = Renderer.transform.DOShakePosition(preDeathTime, 0.3f, 15);
        Renderer.color = Color.red;
        
        yield return new WaitForSeconds(preDeathTime/2);
        Renderer.DOFade(0f, preDeathTime / 2);
        
        yield return new WaitForSeconds(preDeathTime/2);
       
        ReturnToPool();
    }

    private void ClearTargets()
    {
        if (_player.Value.Targets.ClosestEnemy == this)
        {
            _player.Value.Targets.SetClosestEnemy(null);
        }
        if (_player.Value.Targets.FurthestEnemy == this)
        {
            _player.Value.Targets.SetFurthestEnemy(null);
        }
        if (_player.Value.Targets.HighestHealth == this)
        {
            _player.Value.Targets.SetHighestHealthEnemy(null);
        }
        if (_player.Value.Targets.LowestHealth == this)
        {
            _player.Value.Targets.SetLowestHealthEnemy(null);
        }
        DistanceFromPlayer = 99999f;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Health.RemoveHealth(99999);
            return;
        }
        
        // if (col.gameObject.CompareTag("Projectile"))
        // {
        //     ProjectileBase projectile = col.gameObject.GetComponent<ProjectileBase>();
        //     
        //     if (projectile == null)
        //         Help.Debug(GetType(), "OnTriggerEnter2D", "A collision with a projectile has been detected but could not find ProjectileBase component, this is very bad. fix ASAP");
        //     else
        //         Health.RemoveHealth(projectile.Damage);
        // }
        
        
    }

    private void OnDestroy()
    {
        Health.OnHealthZero -= StartOnDeathBehavior;
    }
}

