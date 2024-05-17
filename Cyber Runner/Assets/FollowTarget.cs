using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothing;
    [SerializeField] private float _smoothingMax;
    [SerializeField] private float _smoothingMin;
    private Vector3 _currentVelocity = Vector3.zero;
    [SerializeField] private float _smoothingRatio;

    private void Awake()
    {
       
    }

    void Start()
    {
        
    }

    private void Update()
    {
        if (_player.ConstantForce.force.x / _player.RB.drag >= 1)
        {
            //TODO: ISSUE if drag is set to 0
            float smooth = Mathf.Max(_smoothingMin,_smoothingRatio / (_player.ConstantForce.force.x / _player.RB.drag));
            _smoothing = Mathf.Min(_smoothingMax, smooth);
        }
        else
        {
            _smoothing = _smoothingMax;
        }
        
    }


    void LateUpdate()
    {
        Vector3 targetPos = _player.gameObject.transform.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position ,targetPos, ref _currentVelocity, _smoothing);
    }
}
