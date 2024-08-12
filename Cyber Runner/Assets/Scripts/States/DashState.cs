using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class DashState : PlayerState
{
    public Vector2 DashForce;
    public float KnockbackForce = 5f;
    public int KnockbackDamage = 5;
    public float KnockbackObjectActiveTime = 0.5f;
    public float DashCooldown = 1f;
    public float DashDisableBuffer;
    private Coroutine _dashHandle;
    private bool _dashCooldownActive;
    private LazyService<UpgradesManager> _upgradesManager;
    private LazyService<VFXManager> _vfx;

    public override void OnEnter()
    {
        _player.InvokeOnDashEnter();
        ActivateDashKnockbackObject();
        _player.Collider.excludeLayers = (1<<12);
        _player.Health.SetInvulnerable(true);
        _player.IsDashing = true;
        _player.Gravity = false;
        _vfx.Value.DashDust(_player.transform.position);
        _vfx.Value.DashVortex(_player.transform.position);
        AudioManager.PostEvent(AudioEvent.PL_DASH);

        Vector2 modifiedDashForce = DashForce;
        if (_upgradesManager.Value.HasPerkGroup(PerkGroup.DashDistance, out float val))
        {
            modifiedDashForce = new Vector2(modifiedDashForce.x * Help.PercentToMultiplier(val), modifiedDashForce.y);
            
        }

        _player.RB.AddForce(modifiedDashForce);
        StartDash();
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

        if ( Math.Abs( _player.CurrentRunSpeed - theoreticalMaxSpeed) < DashDisableBuffer  && !_dashCooldownActive)
        {
            DisableDash();
            _player.ActiveState = _player.RunState;
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
        _player.PlayerVisuals.DashKnockBackCollider.SetActive(false);
        _player.Gravity = true;
        _player.Collider.excludeLayers &= ~(1<<12);
        
        
    }
    
    public void StartDash()
    {
        _dashHandle = StartCoroutine(DashCooldownRoutine());
    }

    private IEnumerator DashCooldownRoutine()
    {
        _dashCooldownActive = true;
        yield return new WaitForSecondsRealtime(DashCooldown);
        _dashCooldownActive = false;
        _dashHandle = null;
    }

    private void ActivateDashKnockbackObject()
    {
        if (_dashKnockbackHandle != null)
        {
            StopCoroutine(_dashKnockbackHandle);
        }
        _dashKnockbackHandle = StartCoroutine(DashKnockbackObjectCooldownRoutine(KnockbackObjectActiveTime));
    }

    private Coroutine _dashKnockbackHandle;

    private IEnumerator DashKnockbackObjectCooldownRoutine(float activeTime)
    {
        _player.PlayerVisuals.DashKnockBackCollider.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        _player.PlayerVisuals.DashKnockBackCollider.SetActive(false);
        _dashKnockbackHandle = null;
    }
}