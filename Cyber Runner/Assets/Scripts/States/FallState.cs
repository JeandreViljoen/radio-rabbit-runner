using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerState
{
    
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