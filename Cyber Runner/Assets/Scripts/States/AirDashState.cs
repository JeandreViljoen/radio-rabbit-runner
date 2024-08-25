using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class AirDashState : PlayerState
{
    public Vector2 AirDashForce;
    public float AirDashCooldown = 1f;
    public float AirDashDisableBuffer;
    public float KnockbackObjectActiveTime = 0.5f;
    private Coroutine _airDashHandle;
    private bool _airDashCooldownActive;
    private LazyService<UpgradesManager> _upgradesManager;
    private LazyService<VFXManager> _vfx;
    private LazyService<PowerUpManager> _powerUpManager;
    public override void OnEnter()
    {
        _player.InvokeOnDashEnter();
        if(!_powerUpManager.Value.IsShieldPowerUpActive) _player.ActivateDashKnockbackObject(KnockbackObjectActiveTime);
        _player.Collider.excludeLayers = (1<<12);
        Vector2 modifiedDashForce = AirDashForce;
        if (_upgradesManager.Value.HasPerkGroup(PerkGroup.DashDistance, out float val))
        {
            modifiedDashForce = new Vector2(modifiedDashForce.x * Help.PercentToMultiplier(val), modifiedDashForce.y);
            Debug.Log($"Airdash force base :  {AirDashForce}         |      modified:   {modifiedDashForce}");
        }
        
        AudioManager.PostEvent(AudioEvent.PL_DASH);
        _vfx.Value.DashVortex(_player.transform.position);
        _player.Health.SetInvulnerable(true);
        _player.IsDashing = true;
        _player.RB.velocity = new Vector2(_player.CurrentRunSpeed, 0f);
        _player.Gravity = false;
        _player.RB.AddForce(modifiedDashForce);
        StartAirDash();
        SetAnimation();
        ServiceLocator.GetService<StatsTracker>().DashAmount++;
    }

    public override void OnUpdate()
    {
        float theoreticalMaxSpeed = _player.ConstantForce.force.x / _player.RB.drag;
        
        if (_player.CurrentRunSpeed <= 1)
        {
            DisableDash();
            _player.ActiveState = _player.RunState;
        }

        if ( Math.Abs( _player.CurrentRunSpeed - theoreticalMaxSpeed) < AirDashDisableBuffer  && !_airDashCooldownActive)
        {
            _player.Gravity = true;
            if (_player.IsGrounded)
            {
                _player.ActiveState = _player.RunState;
            }
            else
            {
                _player.ActiveState = _player.FallState;
            }
            
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnExit(PlayerState next)
    {
        base.OnExit(next);
        _player.Health.SetInvulnerable(false);
        if(!_powerUpManager.Value.IsShieldPowerUpActive) _player.ForceDisableDashKnockbackObject();
        _player.Collider.excludeLayers &= ~(1<<12);
        return;
    }
    
    public void StartAirDash()
    {
        _airDashHandle = StartCoroutine(AirDashCooldownRoutine());
    }

    private IEnumerator AirDashCooldownRoutine()
    {
        _airDashCooldownActive = true;
        yield return new WaitForSecondsRealtime(AirDashCooldown);
        _airDashCooldownActive = false;
        _airDashHandle = null;
    }
    
    // private void ActivateDashKnockbackObject()
    // {
    //     if (_dashKnockbackHandle != null)
    //     {
    //         StopCoroutine(_dashKnockbackHandle);
    //     }
    //     _dashKnockbackHandle = StartCoroutine(DashKnockbackObjectCooldownRoutine(KnockbackObjectActiveTime));
    // }
    //
    // private Coroutine _dashKnockbackHandle;
    //
    // private IEnumerator DashKnockbackObjectCooldownRoutine(float activeTime)
    // {
    //     _player.PlayerVisuals.StartDodgeShieldVFX();
    //     _player.PlayerVisuals.DashKnockBackCollider.SetActive(true);
    //     yield return new WaitForSeconds(activeTime);
    //     _player.PlayerVisuals.DashKnockBackCollider.SetActive(false);
    //     _player.PlayerVisuals.StopDodgeShieldVFX();
    //     _dashKnockbackHandle = null;
    // }
}