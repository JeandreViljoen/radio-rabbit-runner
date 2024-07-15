using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState
{
    public PlayerState PreDeathState = null;
    public SubDeathState SubDeathState;

    [SerializeField] protected AnimationClip Fall;
    [SerializeField] protected AnimationClip FloorEntry;
    [SerializeField] protected AnimationClip FloorLoop;
    [SerializeField] protected AnimationClip FloorEnd;
    [SerializeField] protected AnimationClip Wall;
    
    public override void OnEnter()
    {
        
        _player.ConstantForce.force = Vector2.zero;
        _player.Gravity = true;
        SetDeathAnimation();
    }
    
    // public override void OnInit()
    // {
    //     _player.OnLanded += LandBehavior;
    // }
    //
    // private void LandBehavior()
    // {
    //     if (_player.ActiveState == this)
    //     {
    //         _isGrounded = true;
    //     }
    // }

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
    
    private void SetDeathAnimation()
    {
        if (!IsGrounded())
        {
            StartCoroutine(QueueFallLoop());
        }
        else
        {
            StartCoroutine(QueueTripLoop());
        }
    }

    IEnumerator QueueTripLoop()
    {
        SetAnimation(FloorEntry);
        
        while (_player.SpriteAnim.IsPlaying())
        {
            yield return null;
        }
        
        SetAnimation(FloorLoop);

        yield return new WaitUntil(IsStationary);
        
        SetAnimation(FloorEnd);
    }
    
    IEnumerator QueueFallLoop()
    {
        SetAnimation(Fall);
        
        yield return new WaitUntil(IsGrounded);
        {
            yield return null;
        }
        
        SetAnimation(FloorLoop);

        yield return new WaitUntil(IsStationary);
        
        SetAnimation(FloorEnd);
    }

    private bool IsStationary()
    {
        return _player.CurrentRunSpeed < 1;
    }
    
    private bool IsGrounded()
    {
        return _player.IsGrounded;
    }
}

public enum SubDeathState
{
    Air,
    Floor,
    Wall
}
