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

    [SerializeField] private float _zDistanceMin;
    [SerializeField] private float _zDistanceMax;

    private float zOffsetBasedOnSpeed = 0;
    private float xOffsetBasedOnSpeed = 0;

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

        zOffsetBasedOnSpeed = Help.Map(_player.CurrentRunSpeed, 20, 30, 20f, 30f, true);
        xOffsetBasedOnSpeed = Help.Map(_player.CurrentRunSpeed, 0, 35, 5, 17, true);
    }


    void LateUpdate()
    {
        Vector3 targetPos = _player.gameObject.transform.position + _offset;
        targetPos = new Vector3(targetPos.x + xOffsetBasedOnSpeed, targetPos.y, -zOffsetBasedOnSpeed);
        transform.position = Vector3.SmoothDamp(transform.position ,targetPos, ref _currentVelocity, _smoothing);
    }
}
