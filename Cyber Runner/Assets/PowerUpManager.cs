using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class PowerUpManager : MonoService
{
    private LazyService<PlayerController> _player;
    [HideInInspector] public float AttackSpeedMultiplier = 1f;
    private LazyService<EXPManager> _expManager;
    private LazyService<HUDManager> _hudManager;

    [SerializeField] private float AttackSpeedPowerupDuration;
    
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
       
        _player.Value.Health.AddHealth((int)value);
        _hudManager.Value.PowerUpPopup.ShowPopup(PowerupType.Health);
    }
    
    private void EXPPowerup(float value)
    {
        
        _expManager.Value.AddLevel();
        _hudManager.Value.PowerUpPopup.ShowPopup(PowerupType.EXP);
    }

    private Coroutine _attackSpeedHandle;
    private void AttackSpeedPowerup(float value)
    {
        
        _hudManager.Value.PowerUpPopup.ShowPopup(PowerupType.AttackSpeed, AttackSpeedPowerupDuration);
        
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
            yield return new WaitForSeconds(AttackSpeedPowerupDuration);
            IsAttackSpeedActive = false;
            AttackSpeedMultiplier = 1f;
        }
    }

    private Coroutine _shieldHandle;
    private void ShieldPowerup(float value)
    {
        _hudManager.Value.PowerUpPopup.ShowPopup(PowerupType.Shield, value);
        
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
