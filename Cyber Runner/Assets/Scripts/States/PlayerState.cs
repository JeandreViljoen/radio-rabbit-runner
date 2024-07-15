using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    protected PlayerController _player;
    [SerializeField] protected AnimationClip Animation;
    
    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }

    public virtual void OnExit(PlayerState state)
    {
        if (state == _player.DeadState)
        {
            _player.DeadState.PreDeathState = this;
        }
    }

    public virtual void OnInit() { }

    public void Init(PlayerController playerController)
    {
        _player = playerController;
        OnInit();
    }
    
    protected void DisableDash()
    {
        _player.IsDashing = false;
        _player.Gravity = true;
    }

    protected void SetAnimation()
    {
        _player.SpriteAnim.Play(Animation);
    }
    
    protected void SetAnimation(AnimationClip clipToSet)
    {
        _player.SpriteAnim.Play(clipToSet);
    }
}
