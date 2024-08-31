using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class PowerUpManager : MonoService
{
    private LazyService<PlayerController> _player;
    public float AttackSpeedMultiplier = 1f;
    private LazyService<EXPManager> _expManager;

    [SerializeField] private float PowerupDuration;
    
    public bool IsShieldPowerUpActive = false;
    public bool IsAttackSpeedActive = false;
    
    public void DoPowerupEffect(PowerupType type, float value)
    {
        switch (type)
        {
            case PowerupType.Health:
                HealthPowerup(value);
                break;
            case PowerupType.EXP:
                EXPPowerup(value);
                break;
            case PowerupType.AttackSpeed:
                AttackSpeedPowerup(value);
                break;
            case PowerupType.Shield:
                ShieldPowerup(value);
                break;
            default:
                break;
        }
    }

    private void HealthPowerup(float value)
    {
        Debug.LogError("HEALTH POWERUP!");
        _player.Value.Health.AddHealth((int)value);
    }
    
    private void EXPPowerup(float value)
    {
        Debug.LogError("EXP POWERUP!");
        _expManager.Value.AddLevel();
    }

    private Coroutine _attackSpeedHandle;
    private void AttackSpeedPowerup(float value)
    {
        Debug.LogError("ATTACK SPEED POWERUP!");
        
        if (_attackSpeedHandle != null)
        {
            StopCoroutine(AttackSpeedCountdown());
            _attackSpeedHandle = null;
        }

        _attackSpeedHandle = StartCoroutine(AttackSpeedCountdown());
        
        IEnumerator AttackSpeedCountdown()
        {
            IsAttackSpeedActive = true;
            AttackSpeedMultiplier = value;
            yield return new WaitForSeconds(PowerupDuration);
            IsAttackSpeedActive = false;
            AttackSpeedMultiplier = 1f;
        }
    }

    private Coroutine _shieldHandle;
    private void ShieldPowerup(float value)
    {
        Debug.LogError("SHIELD POWERUP!");
        if (_shieldHandle != null)
        {
            StopCoroutine(ShieldCountdown());
            _shieldHandle = null;
        }

        _shieldHandle = StartCoroutine(ShieldCountdown());
        
        IEnumerator ShieldCountdown()
        {
            _player.Value.ActivateDashKnockbackObject(value);
            IsShieldPowerUpActive = true;
            yield return new WaitForSeconds(value);
            IsShieldPowerUpActive = false;

        }
    }
    
    
}
