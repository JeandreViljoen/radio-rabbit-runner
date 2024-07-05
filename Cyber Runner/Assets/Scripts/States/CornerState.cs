using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerState : PlayerState
{
    [SerializeField] private Vector2 _LiftAmount;
  
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
    
    public override void OnEnter()
    {
        _player.Gravity = true;
        _player.IsJumping = true;
        _player.RB.velocity = new Vector2(_player.CurrentRunSpeed, 0f);
        _player.RB.AddForce(_LiftAmount);
        SetAnimation();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
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