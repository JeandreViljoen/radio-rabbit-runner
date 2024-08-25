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

public class Enemy : MonoBehaviour
{

    private bool _isAlreadyElectrocuted = false;
    public Vector2 Target;
    public float MoveSpeed = 1;
    private float additionalMoveSpeed = 0;
    public float MomentumMultiplier = 2;
    public Health Health;
    public int Damage = 0;
    [SerializeField] private int _expValue;
    public SpriteRenderer Renderer;
    public float DistanceFromPlayer { get; private set; }

    private int _calculationFrameSkips = 10;
    private int _frameSkipCounter = 0;

    private LazyService<PlayerController> _player;
    private LazyService<EXPManager> _expManager;
    private LazyService<UpgradesManager> _upgradesManager;

    public EnemyState State = EnemyState.Active;
    private LazyService<VFXManager> _vfx;
    private LazyService<PrefabPool> _prefabPool;

    private void Awake()
    {

    }

    void Start()
    {
        Health.OnHealthZero += StartOnDeathBehavior;
        Health.OnHealthLost += DamageFlash;
        AudioManager.RegisterGameObj(gameObject);
    }

    private void OnHit(ProjectileBase projectile)
    {
        Health.RemoveHealth(projectile.Damage); 
        
    }

    private void OnEnable()
    {
        _isAlreadyElectrocuted = false;
        gameObject.layer = 10;
        Renderer.color = Color.white;
        State = EnemyState.Active;
        _frameSkipCounter = 0;
        Health.InitHealth();
        ServiceLocator.GetService<EnemiesManager>().RegisterEnemy(this);
        //CheckMaxHealthTargeting();
    }

    private void CheckMaxHealthTargeting()
    {
        Targets targets = ServiceLocator.GetService<PlayerController>().Targets;
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
            AudioManager.SetObjectPosition(gameObject, transform);
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
        if (State == EnemyState.Dead || _player.Value.IsDead())
        {
            //transform.position =
                //Vector2.MoveTowards(this.transform.position, _player.Value.transform.position, (MoveSpeed +additionalMoveSpeed) * Time.deltaTime);
        }
        else
        {
            additionalMoveSpeed = (_player.Value.SpeedDelta * -1 / _player.Value.TheoreticalMaxSpeed) * MomentumMultiplier;
        
            //Vector2 targetUnitVector = (_player.Value.transform.position - transform.position);
            if (!_stunned) transform.position =
                Vector2.MoveTowards(this.transform.position, _player.Value.transform.position, (MoveSpeed + Math.Abs(additionalMoveSpeed)) * Time.deltaTime);
        }
      
    }

    private Tween _knockbackTween;
    public void ApplyKnockback(Vector3 direction, float amount, float speed = 0.3f)
    {
        if (direction == Vector3.zero || amount == 0f)
        {
            return;
        }
        
        float KnockbackSpeed = 0.3f;

        _knockbackTween?.Kill();
        _knockbackTween = transform.DOLocalMove(transform.localPosition + direction.normalized * amount, KnockbackSpeed).SetEase(Ease.OutSine);

    }

    private void ReturnToPool()
    {
        ServiceLocator.GetService<EnemiesManager>().DeRegisterEnemy(this);
        ServiceLocator.GetService<PrefabPool>().Return(gameObject);
    }

    private void StartOnDeathBehavior()
    {
        
        _knockbackTween?.Kill();
        State = EnemyState.Dead;
        StartCoroutine(OnDeathBehavior(0.3f));
    }

    IEnumerator OnDeathBehavior (float preDeathTime)
    {
        AudioManager.PostEvent(AudioEvent.ENEMY_DEATH, gameObject);
        ServiceLocator.GetService<EnemiesManager>().InvokeEnemyKilled(this);
        ProcessEXP();
        CheckVampirism();
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

    private void ProcessEXP()
    {
        if (!ServiceLocator.GetService<LevelBlockManager>().ActiveBlock.IsSafeBlock)
        {
            _expManager.Value.AddEXP(_expValue);
        }
    }

    private void CheckVampirism()
    {
        if (_upgradesManager.Value.HasPerkGroup(PerkGroup.Vampirism, out float val))
        {
            int rng = UnityEngine.Random.Range(0,101);

            if (rng <= val)
            {
                _player.Value.Health.AddHealth(1);
            }
            
        }
    }

    private Tween _damageTween;
    private void DamageFlash()
    {
        _damageTween?.Kill();
        
        Sequence s = DOTween.Sequence();
        s.Append(Renderer.DOColor(Color.red, 0.001f));
        s.Append(Renderer.DOColor(Color.white, 0.2f));
        _damageTween = s;
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
        if (col.gameObject.CompareTag("DashKnockback"))
        {
            Vector3 dir = transform.position - col.gameObject.transform.position;
            ApplyKnockback(dir, _player.Value.DashState.KnockbackForce, 0.15f);
            Health.RemoveHealth(_player.Value.DashState.KnockbackDamage);
            AudioManager.PostEvent(AudioEvent.ENEMY_IMPACT_HOLLOW);
            _player.Value.PlayerVisuals.FlashDashShieldDamage2();
            _vfx.Value.OnHitShield(transform.position, _player.Value.PlayerVisuals.TransformForShieldVFX,transform.position - col.transform.position);
        }
    }
    
    

    private void OnCollisionEnter2D(Collision2D col)
    {
        
        if (col.gameObject.CompareTag("Player") && Health.CurrentHealth > 0)
        {
            Health.RemoveHealth(Health.CurrentHealth);
            col.gameObject.GetComponent<PlayerController>().Health.RemoveHealth(Damage);
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

    public void SetElectrocutionCooldown(float timer)
    {
        StartCoroutine(ElectrocutionTimeout());
        
        IEnumerator ElectrocutionTimeout()
        {
            _isAlreadyElectrocuted = true;
            yield return new WaitForSeconds(timer);
            _isAlreadyElectrocuted = false;

        }
    }

    public bool IsAlreadyElectrocuted()
    {
        return _isAlreadyElectrocuted;
    }

    private bool _stunned = false;
    private Coroutine stunHandle;
    public void Stun(float duration)
    {
        if (stunHandle != null)
        {
            StopCoroutine(StunRoutine());
        }
        
        stunHandle = StartCoroutine(StunRoutine());
        
        IEnumerator StunRoutine()
        {
            _stunned = true;
            Renderer.color = Color.blue;
            yield return new WaitForSeconds(duration);
            Renderer.color = Color.white;
            _stunned = false;
        }
    }

    private void OnDestroy()
    {
        Health.OnHealthZero -= StartOnDeathBehavior;
        Health.OnHealthLost -= DamageFlash;
    }
}

