using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PowerTools;
using Services;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoService
{
    
    
    
    
    [ShowInInspector, GUIColor("blue")] private PlayerState _activeState;
    public PlayerState ActiveState
    {
        get
        {
            return _activeState;
        }
        set
        {
            if (_stateLock)
            {
                return;
            }
            
            
            if (value == null)
            {
                Help.Debug(GetType(), "ActiveState", "Cannot Transition to null state.");
            }
            if (_activeState == null)
            {
                Help.Log(GetType(), "ActiveState", $"Previous state was null. Initialised with {value}");
            }
            if (_activeState == value)
            {
                Help.Debug(GetType(), "ActiveState", $"!!!!!!!!!!!!!!!!!!!!!! - Tried to enter {value} state that is already active, returning early");
                return;
            }
            
            if (_activeState != null) _activeState.OnExit(value);
            _activeState = value;
            _activeState.OnEnter();
        }
    }
    
    [SerializeField] private float _killHeight;
    [FoldoutGroup("State References")]
    [SerializeField] public RunState RunState;
    [FoldoutGroup("State References")]
    [SerializeField] public JumpState JumpState;
    [FoldoutGroup("State References")]
    [SerializeField] public FallState FallState;
    [FoldoutGroup("State References")]
    [SerializeField] public DashState DashState;
    [FoldoutGroup("State References")]
    [SerializeField] public AirDashState AirDashState;
    [FoldoutGroup("State References")]
    [SerializeField] public CornerState CornerState;
    [FoldoutGroup("State References")]
    [SerializeField] public DeadState DeadState;

    [HideInInspector] public Rigidbody2D RB;
    [HideInInspector] public Collider2D Collider;
    [HideInInspector] public ConstantForce2D ConstantForce;

    [FoldoutGroup("Visual References")]
    public PlayerVisuals PlayerVisuals;
    [FoldoutGroup("Visual References")]
    public SpriteAnim SpriteAnim;
    [FoldoutGroup("Visual References")]
    public SpriteAnim PlayerRenderer;

    public Health Health;

    public float CurrentRunSpeed => RB.velocity.x;
    [SerializeField]private float _speed;

    private LazyService<VFXManager> _vfx;
    private LazyService<StatsTracker> _stats;

    public bool IsDashing = false;

    private bool _isJumping = false;
    public bool IsJumping
    {
        get => _isJumping;
        set
        {
            if (!value)
            {
                _hasDoubleJumped = false;
            }
            _isJumping = value;
        }
    }
    public bool CanDoubleJump = true;
    private bool _hasDoubleJumped = false;

    private bool _gravity;
    public bool Gravity
    {
        get
        {
            return ((RB.constraints & RigidbodyConstraints2D.FreezePositionY) != 0);
            //return RB.gravityScale != 0;
        }
        set
        {
            if (value)
            {
                RB.constraints = RigidbodyConstraints2D.None;
                //RB.gravityScale = _startingGravityScale;
            }
            else
            {
                RB.constraints = RigidbodyConstraints2D.FreezePositionY;
                //RB.gravityScale = 0;
            }
        }
    }

    public float SpeedDelta
    {
        get
        {
            if (CurrentRunSpeed - TheoreticalMaxSpeed < 1 && CurrentRunSpeed - TheoreticalMaxSpeed > 1)
            {
                return 0;
            }
            else
            {
                return CurrentRunSpeed - TheoreticalMaxSpeed;
            }
        }
    }

    public float TheoreticalMaxSpeed
    {
        get
        {
            return ConstantForce.force.x / RB.drag;
        }
    }

    private float _startingGravityScale;

    private Coroutine DashHandle;
    public event Action OnDashEnter;

    public void InvokeOnDashEnter()
    {
        OnDashEnter?.Invoke();
    }

    public Targets Targets;


    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        ConstantForce = GetComponent<ConstantForce2D>();
        Health = GetComponent<Health>();
        
        _startingGravityScale = RB.gravityScale;
    }

    private bool _stateLock = false;
    private void SetDeathState()
    {
        ActiveState = DeadState;
        _stateLock = true;
    }

    void Start()
    {
        RunState.Init(this);
        JumpState.Init(this);
        FallState.Init(this);
        DashState.Init(this);
        AirDashState.Init(this);
        CornerState.Init(this);
        DeadState.Init(this);
        
        ActiveState = RunState;

        Health.OnHealthZero += SetDeathState;

            if (RB == null || Collider == null || ConstantForce == null)
        {
           Help.Debug(GetType(), "Start", "Rigidbody, Constant Force or collider is null");
        }
    }

    public bool IsDead()
    {
        return ActiveState == DeadState;
    }

    void Update()
    {
        if (transform.position.y < _killHeight)
        {
            Health.RemoveHealth(99999);
        }
        
        PlayerVisuals.transform.position = this.transform.position;

        if (!IsDead())
        {
            PlayerRenderer.Speed = Help.Map(CurrentRunSpeed, 0, 50, 0f, 2f);
            ServiceLocator.GetService<TVController>().TVBaseAnim.Speed = Help.Map(CurrentRunSpeed, 0, 50, 0f, 2f);
            ServiceLocator.GetService<TVController>().TVFaceAnim.Speed = Help.Map(CurrentRunSpeed, 0, 50, 0f, 2f);
        }
        else
        {
            PlayerRenderer.Speed = 1f;
            ServiceLocator.GetService<TVController>().TVBaseAnim.Speed = 1f;
        }
           
        
        
        //For Inspector display only
        _speed = CurrentRunSpeed;
        ServiceLocator.GetService<HUDManager>().SetSpeedValue(CurrentRunSpeed);
        _stats.Value.CheckFastestSpeed(CurrentRunSpeed);
        
        if (Input.GetKeyDown(GlobalGameAssets.Instance.JumpKey) && !IsDead())
        {
            if (IsJumping && _hasDoubleJumped)
            {
                return;
            }
            
            if (CanDoubleJump && IsJumping && !_hasDoubleJumped)
            {
                if (ActiveState!= JumpState)
                {
                    ActiveState = JumpState;
                }
                else
                {
                    JumpState.Jump();
                }
                _hasDoubleJumped = true;
            }
            else
            {
                ActiveState = JumpState;
            }
            
        }
        
        if (Input.GetKeyDown(GlobalGameAssets.Instance.DashKey) && !IsDead())
        {
            if (IsDashing)
            {
                return;
            }
            
            if (IsJumping)
            {
                ActiveState = AirDashState;
            }
            else
            {
                ActiveState = DashState;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ConstantForce.force += new Vector2(5,0);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ConstantForce.force -= new Vector2(5,0);
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            Health.RemoveHealth(99999);
        }
        
        
        if(_activeState) _activeState.OnUpdate();
    }
    
    private void FixedUpdate()
    {
        if(_activeState) _activeState.OnFixedUpdate();
        AudioManager.SetRTPCValue("RunSpeed", CurrentRunSpeed);
    }

    public event Action OnLanded;
    private bool _ignoreFirstFloorTriggerFlag = true;
    [ShowInInspector] public bool IsGrounded { get; private set; } = true;


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("DamageTile"))
        {
            Health.RemoveHealth(2);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            _hasDoubleJumped = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        

        if (col.CompareTag("SpeedTile"))
        {
            RB.AddForce(Vector2.right*900f);
        }
        
        if (col.CompareTag("Floor"))
        {
            if (_ignoreFirstFloorTriggerFlag)
            {
                _ignoreFirstFloorTriggerFlag = false;
                return;
            }
            OnLanded?.Invoke();
            AudioManager.PostEvent(AudioEvent.PL_LAND);
            IsGrounded = true;
            _vfx.Value.LandingDust(transform.position);
        }

        if (col.CompareTag("Corner"))
        {
            if(ActiveState != CornerState)
            {
                ActiveState = CornerState;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Floor"))
        {
            IsGrounded = false;
        }
    }

    private void OnDestroy()
    {
        Health.OnHealthZero -= SetDeathState;
    }

    public enum DamageSource
    {
        PlayerBody,
        PlayerDash,
        PlayerProjectile
        
    }

    
}
