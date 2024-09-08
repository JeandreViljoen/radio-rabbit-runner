using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform TransformToFollow;

    [SerializeField] private float _smoothing;
    [ShowInInspector]private float _smoothingDelta;
    private Vector3 _currentVelocity = Vector3.zero;
   
    void Update()
    {
        _smoothingDelta = Help.Map(ServiceLocator.GetService<PlayerController>().CurrentRunSpeed, 30, 50, 0f, 0.05f,
            true); 
    }
    
    void LateUpdate()
    {
        Vector3 targetPos = TransformToFollow.position;
        transform.position = Vector3.SmoothDamp(transform.position ,targetPos, ref _currentVelocity, _smoothing);
    }
}
