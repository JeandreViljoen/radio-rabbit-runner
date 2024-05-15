using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    //TODO: ADD Double Jump (Perhaps only reset by dash)
    //TODO: SPeed up camera depending on Constant Force
    
    
    
    
    
    
    
    
    
    
    
    
    
    [SerializeField] private PlayerState _activeState;
    public PlayerState ActiveState
    {
        get
        {
            return _activeState;
        }
        set
        {
            if (value == null)
            {
                Help.Debug(GetType(), "ActiveState", "Cannot Transition to null state.");
            }
            if (_activeState == null)
            {
                Help.Debug(GetType(), "ActiveState", "Previous state was null. Make sure it is initialised before referencing it.");
            }
            
            _activeState.OnExit(value);
            _activeState = value;
            _activeState.OnEnter();
        }
    }

    [SerializeField] public RunState RunState;
    [SerializeField] public JumpState JumpState;
    [SerializeField] public FallState FallState;
    [SerializeField] public DashState DashState;
    [SerializeField] public AirDashState AirDashState;
    
    [HideInInspector] public Rigidbody2D RB;
    [HideInInspector] public Collider2D Collider;
    [HideInInspector] public ConstantForce2D ConstantForce;
    
    //public Collider2D FloorTrigger;
 

    public float CurrentRunSpeed => RB.velocity.x;
    [SerializeField] private float _speed;

    public bool IsDashing = false;
    public bool IsJumping = false;

    private bool _gravity;
    public bool Gravity
    {
        get
        {
            return RB.gravityScale != 0;
        }
        set
        {
            if (value)
            {
                RB.gravityScale = _startingGravityScale;
            }
            else
            {
                RB.gravityScale = 0;
            }
        }
    }

    private float _startingGravityScale;

    private Coroutine DashHandle;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        ConstantForce = GetComponent<ConstantForce2D>();
        
        _startingGravityScale = RB.gravityScale;
    }

    void Start()
    {
        RunState.Init(this);
        JumpState.Init(this);
        FallState.Init(this);
        DashState.Init(this);
        AirDashState.Init(this);

        _activeState = RunState;
        ActiveState = RunState;
        
        if (RB == null || Collider == null || ConstantForce == null)
        {
           Help.Debug(GetType(), "Start", "Rigidbody, Constant FOrce or collider is null");
        }
    }

    void Update()
    {
        //For Inspector display only
        _speed = CurrentRunSpeed;
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (IsJumping)
            {
                return;
            }
            ActiveState = JumpState;
        }
        
        if (Input.GetKeyDown(KeyCode.X))
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
        
        
        _activeState.OnUpdate();
    }
    
    private void FixedUpdate()
    {
        _activeState.OnFixedUpdate();
    }

    public event Action OnLanded;
    private bool _ignoreFirstFloorTriggerFlag = true;
    private void OnTriggerEnter2D(Collider2D col)
    {
        
        if (col.CompareTag("Floor"))
        {
            if (_ignoreFirstFloorTriggerFlag)
            {
                _ignoreFirstFloorTriggerFlag = false;
                return;
            }
            OnLanded?.Invoke();
        }
    }

   

    
}
