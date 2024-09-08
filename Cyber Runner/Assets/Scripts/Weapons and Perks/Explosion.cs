using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    
    public int Damage = 0;
    public float Knockback = 0f;
    public float MiniExplosionKnockback = 1f;
    private Vector3 _startScale;
    private LazyService<PrefabPool> _prefabPool;
    private LazyService<UpgradesManager> _upgradesManager;
    private LazyService<ProjectileManager> _projectileManager;
    private LazyService<PlayerController> _player;
    [SerializeField] private ParticleSystem VFX;

    private void Awake()
    {
        _startScale = transform.localScale;
    }

    void Start()
    {
        AudioManager.RegisterGameObj(gameObject);
    }
    
    void Update()
    {
        AudioManager.SetObjectPosition(gameObject, transform);
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
                    _projectileManager.Value.SpawnDelayedExplosion(0.0f, pos, (int)(Damage * (data.Value/100)), 1f, MiniExplosionKnockback);
                }
            }

            Vector3 dir = col.gameObject.transform.position - gameObject.transform.position;
            col.gameObject.GetComponent<Enemy>().ApplyKnockback(dir, Knockback);
        }
    }

    public void DoExplosion()
    {
        
        StartCoroutine(ReturnToPool(0.5f));
    }

    IEnumerator ReturnToPool(float timeout)
    {
        VFX.Play();
        AudioManager.PostEvent(AudioEvent.WPN_EXPLOSION, gameObject);
        yield return new WaitUntil(() =>
        {
            return !VFX.isPlaying;
        });
        //yield return new WaitForSeconds(timeout);
        transform.localScale = _startScale;
        _prefabPool.Value.Return(this.gameObject);
    }
}
