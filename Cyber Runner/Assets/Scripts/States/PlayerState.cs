using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    protected PlayerMovement _player;
    
    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnExit(PlayerState state) { }

    public virtual void OnInit() { }

    public void Init(PlayerMovement playerMovement)
    {
        _player = playerMovement;
        OnInit();
    }
    
    protected void DisableDash()
    {
        _player.IsDashing = false;
        _player.Gravity = true;
    }
}
