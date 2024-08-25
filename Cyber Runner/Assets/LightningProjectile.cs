using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.Utilities;
using UnityEngine;

public class LightningProjectile : ProjectileBase
{
    public int Bounces;
    public float StunChance;
    public float LifeTime = 0.5f;
    public float StunDuration;
   // [HideInInspector] public float AlreadyElectrocutedTimer;
    
    private List<Enemy> ProximityTargets = new List<Enemy>();
    [SerializeField] private ProximityDetector _proximityDetector;

    private LazyService<ProjectileManager> _projectileManager;
    private LazyService<UpgradesManager> _upgradesManager;
    private LazyService<PlayerController> _player;

    [SerializeField] private LightningVFX _vfx;
    public FlatLineTester flt;
    
    public void DoFire()
    {
        Enemy oldTarget = TargetEntity.GetComponent<Enemy>();
        
        DoLightningDamage(oldTarget);
        TryStun(oldTarget);
        DoImpact();
        
        
        _proximityDetector.EnableProximityDetector(TargetEntity);
        float arcDelay = ServiceLocator.GetService<UpgradesManager>().GetWeaponInstance(WeaponType.Lightning)
            .GetComponent<LightningGun>().ArcDelay;
        StartCoroutine(WaitForTargets());
        

        IEnumerator WaitForTargets()
        {
            yield return null;
            yield return new WaitForSeconds(arcDelay);
            
            ProximityTargets = _proximityDetector.GetProximityTargets();

            if (!ProximityTargets.IsNullOrEmpty() && Bounces-1 > 0)
            {
                if (_upgradesManager.Value.HasUpgrade(UpgradeType.Lightning_DamageIncreasePerBounce))
                {
                    float newDamageMultiplier = 1 + (_upgradesManager.Value
                        .GetUpgradeData(UpgradeType.Lightning_DamageIncreasePerBounce).Value / 100);

                    float newDamage = Damage * newDamageMultiplier;

                    Damage = (int)newDamage;
                }

                foreach (var newTarget in ProximityTargets)
                {
                   // Debug.Log($"Lightning | {TargetEntity.name} -> {newTarget.name}");
                    _projectileManager.Value.SpawnLightningNode(newTarget.gameObject, Damage, Bounces-1, StunChance, StunDuration);
                    DoLightningVFX(oldTarget , newTarget);
                }
            }
        }

       
        
    }

    private void TryStun(Enemy e)
    {
        float rng = UnityEngine.Random.Range(0f, 1f);

        if (rng <= StunChance)
        {
            e.Stun(StunDuration);
        }
    }

    public void DoLightningVFX(Enemy old, Enemy target)
    {
        StartCoroutine(VFXTimer());
        
        IEnumerator VFXTimer()
        {
            flt.A = old;
            flt.B = target;
            flt.gameObject.SetActive(true);
            yield return new WaitForSeconds(LifeTime);
            flt.gameObject.SetActive(false);
        }
    }
    
    public void DoLightningVFXStart(Enemy old, Transform target)
    {
        StartCoroutine(VFXTimer());
        
        IEnumerator VFXTimer()
        {
            flt.GunStartPos = target;
            flt.A = old;
            flt.gameObject.SetActive(true);
            yield return new WaitForSeconds(LifeTime);
            flt.gameObject.SetActive(false);
        }
    }

    private void DoLightningDamage(Enemy e)
    {
        int modifiedDamage = Damage;
        
        if (_upgradesManager.Value.HasPerkGroup(PerkGroup.IncreaseDamageFromMissingHealth, out float value))
        {
            int healthDifference = _player.Value.Health.MaxHealth - _player.Value.Health.CurrentHealth;
            float damageIncrease = Help.PercentToMultiplier(value * healthDifference);

            
            modifiedDamage = (int)Math.Round(modifiedDamage * damageIncrease, MidpointRounding.AwayFromZero);
        }

        e.SetElectrocutionCooldown(LifeTime);
        
        if (e.Health.RemoveHealth(modifiedDamage))
        {
            DoOnKillEffects();
        }
        
        Debug.Log( $"Lightning Damage:    {modifiedDamage}" );

        
        DoOnHitEffects();
    }

    public override void DoOnKillEffects()
    {
        //_proximityDetector.ForceReturn();
        //flt.gameObject.SetActive(false);
        //TargetEntity = null;
        flt.A = null;
        flt.B = null;
    }

    protected virtual void DoImpact()
    {

        StartCoroutine(CooldownTillReturn());

        IEnumerator CooldownTillReturn()
        {
            yield return new WaitForSeconds(LifeTime+ 0.1f);
            TargetEntity = null;
            flt.A = null;
            flt.B = null;
            _prefabPool.Value.Return(gameObject);
            
        }
        
    }


    protected new void Update()
    {
        if (TargetEntity == null)
        {
            return;
        }
        
        transform.position = TargetEntity.transform.position;
    }
}
