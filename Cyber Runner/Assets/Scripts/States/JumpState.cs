using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class JumpState : PlayerState
{
    
    [SerializeField] private Vector2 _jumpForce;

    public override void OnInit()
    {
        _player.OnLanded += LandBehavior;
    }

    private void LandBehavior()
    {
        if (_player.ActiveState == this)
        {
            _player.ActiveState = _player.RunState;
        }
    }

    public void Jump()
    {
        _player.Gravity = true;
        _player.IsJumping = true;
        _player.RB.velocity = new Vector2(_player.CurrentRunSpeed, 0f);
        _player.RB.AddForce(_jumpForce);
        ServiceLocator.GetService<StatsTracker>().JumpAmount++;
    }

    public override void OnEnter()
    {
        Jump();
        SetAnimation();
    }

    public override void OnUpdate()
    {
        if (_player.RB.velocity.y < -3) //was 6
        {
            _player.ActiveState = _player.FallState;
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnExit(PlayerState next)
    {
        return;
    }
    
}
