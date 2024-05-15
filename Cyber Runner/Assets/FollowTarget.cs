using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothing;
    private Vector3 _currentVelocity = Vector3.zero;

    private void Awake()
    {
       
    }

    void Start()
    {
        
    }

    
    void LateUpdate()
    {
        Vector3 targetPos = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position ,targetPos, ref _currentVelocity, _smoothing);
    }
}
