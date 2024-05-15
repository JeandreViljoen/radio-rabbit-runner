using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private ConstantForce2D _force;
    [SerializeField] private Collider2D _floorTrigger;
    public Vector2 JumpForce;
    public Vector2 DashForce;
    public float CurrentRunSpeed;
    public float DashDisableBuffer;
    [SerializeField]public float _dashCooldown;


    private bool _isDashing = false;
    private bool _dashCoolDownActive = false;

    private bool _isJumping = false;

    private float _startingGravityScale;

    private Coroutine DashHandle;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _force = GetComponent<ConstantForce2D>();
        
        _startingGravityScale = _rb.gravityScale;

        if (_rb == null || _collider == null || _force == null)
        {
           Help.Debug(GetType(), "Start", "Rigidbody or collider is null");
        }
    }

    
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_isJumping)
            {
                return;
            }

            _isJumping = true;
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
            _rb.AddForce(JumpForce);
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (_isDashing)
            {
                return;
            }
            _isDashing = true;

            if (_isJumping)
            {
                _rb.gravityScale = 0;
                _rb.velocity = new Vector2(_rb.velocity.x, 0f);
            }
            else
            {
                
            }
            
            _rb.AddForce(DashForce);
            DashHandle = StartCoroutine(DashCooldown());
        }
        
    }

    private IEnumerator DashCooldown()
    {
        _dashCoolDownActive = true;
        yield return new WaitForSecondsRealtime(_dashCooldown);
        _dashCoolDownActive = false;
        DashHandle = null;
    }

    private void FixedUpdate()
    {
        CurrentRunSpeed = _rb.velocity.x;
        float theoreticalMaxSpeed = _force.force.x / _rb.drag;

        if ( Math.Abs( CurrentRunSpeed - theoreticalMaxSpeed) < DashDisableBuffer  && !_dashCoolDownActive)
        {
            DisableDash();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Floor"))
        {
            _isJumping = false;
        }
    }

    private void DisableDash()
    {
        _isDashing = false;
        _rb.gravityScale = _startingGravityScale;
    }
}
