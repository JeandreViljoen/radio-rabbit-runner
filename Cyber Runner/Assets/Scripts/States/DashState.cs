using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class DashState : PlayerState
{
    public Vector2 DashForce;
    public float DashCooldown = 1f;
    public float DashDisableBuffer;
    private Coroutine _dashHandle;
    private bool _dashCooldownActive;
    private LazyService<UpgradesManager> _upgradesManager;

    public override void OnEnter()
    {
        _player.Health.SetInvulnerable(true);
        _player.IsDashing = true;

        Vector2 modifiedDashForce = DashForce;
        if (_upgradesManager.Value.HasPerkGroup(PerkGroup.DashDistance, out float val))
        {
            modifiedDashForce = new Vector2(modifiedDashForce.x * Help.PercentToMultiplier(val), modifiedDashForce.y);
            Debug.Log($"Dash force base :  {DashForce}         |      modified:   {modifiedDashForce}");
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
}