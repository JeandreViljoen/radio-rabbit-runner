using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState
{
    public Vector2 DashForce;
    public float DashCooldown = 1f;
    public float DashDisableBuffer;
    private Coroutine _dashHandle;
    private bool _dashCooldownActive;
    

    public override void OnEnter()
    {
        _player.Health.SetInvulnerable(true);
        _player.IsDashing = true;
        _player.RB.AddForce(DashForce);
        StartDash();
        SetAnimation();
    }

    public override void OnUpdate()
    {
        float theoreticalMaxSpeed = _player.ConstantForce.force.x / _player.RB.drag;

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