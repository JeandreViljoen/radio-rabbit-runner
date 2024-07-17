using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDashState : PlayerState
{
    public Vector2 AirDashForce;
    public float AirDashCooldown = 1f;
    public float AirDashDisableBuffer;
    private Coroutine _airDashHandle;
    private bool _airDashCooldownActive;
    public override void OnEnter()
    {
        _player.Health.SetInvulnerable(true);
        _player.IsDashing = true;
        _player.RB.velocity = new Vector2(_player.CurrentRunSpeed, 0f);
        _player.Gravity = false;
        _player.RB.AddForce(AirDashForce);
        StartAirDash();
        SetAnimation();
    }

    public override void OnUpdate()
    {
        float theoreticalMaxSpeed = _player.ConstantForce.force.x / _player.RB.drag;

        if ( Math.Abs( _player.CurrentRunSpeed - theoreticalMaxSpeed) < AirDashDisableBuffer  && !_airDashCooldownActive)
        {
            _player.Gravity = true;
            _player.ActiveState = _player.FallState;
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