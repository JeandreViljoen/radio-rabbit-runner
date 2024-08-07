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
    private Coroutine _airDashHandle;
    private bool _airDashCooldownActive;
    private LazyService<UpgradesManager> _upgradesManager;
    private LazyService<VFXManager> _vfx;
    public override void OnEnter()
    {
        Vector2 modifiedDashForce = AirDashForce;
        if (_upgradesManager.Value.HasPerkGroup(PerkGroup.DashDistance, out float val))
        {
            modifiedDashForce = new Vector2(modifiedDashForce.x * Help.PercentToMultiplier(val), modifiedDashForce.y);
            Debug.Log($"Airdash force base :  {AirDashForce}         |      modified:   {modifiedDashForce}");
        }
        
        
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
}