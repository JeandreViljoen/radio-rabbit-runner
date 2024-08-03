using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    
    public int Damage = 0;
    private Vector3 _startScale;
    private LazyService<PrefabPool> _prefabPool;
    private LazyService<UpgradesManager> _upgradesManager;
    private LazyService<ProjectileManager> _projectileManager;
    private LazyService<PlayerController> _player;

    private void Awake()
    {
        _startScale = transform.localScale;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            int modifiedDamage = Damage;

            if (_upgradesManager.Value.HasPerkGroup(PerkGroup.IncreaseDamageFromMissingHealth, out float value))
            {
                int healthDifference = _player.Value.Health.MaxHealth - _player.Value.Health.CurrentHealth;
                float damageIncrease = Help.PercentToMultiplier(value * healthDifference);

                //modifiedDamage = (int)(modifiedDamage * damageIncrease);
                modifiedDamage = (int)Math.Round(modifiedDamage * damageIncrease, MidpointRounding.AwayFromZero);
            }
            
            if (col.gameObject.GetComponent<Enemy>().Health.RemoveHealth(modifiedDamage))
            {
                if (_upgradesManager.Value.HasUpgrade(UpgradeType.RocketLauncher_MiniExplosions))
                {
                    Vector3 pos = new Vector3();
                    pos = col.transform.position;
                    
                    var data = _upgradesManager.Value.GetUpgradeData(UpgradeType.RocketLauncher_MiniExplosions);
                    _projectileManager.Value.SpawnDelayedExplosion(0.0f, pos, (int)(Damage * (data.Value/100)), 1f);
                }
            }
        }
    }

    public void DoExplosion()
    {
        StartCoroutine(ReturnToPool(0.5f));
    }

    IEnumerator ReturnToPool(float timeout)
    {
        yield return new WaitForSeconds(timeout);
        transform.localScale = _startScale;
        _prefabPool.Value.Return(this.gameObject);
    }
}
