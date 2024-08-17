using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class ParticleVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem VFX;
    private LazyService<PrefabPool> _prefabPool;
    private Transform _followTarget = null;

    public bool PlayOnSpawn = true;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        if (PlayOnSpawn)
        {
           Play();
        }
    }

   
    private void Update()
    {
        if (_followTarget != null)
        {
            transform.position = _followTarget.position;
        }
    }

    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }

    public void Play(Vector3 direction)
    {
        var _direction = direction.normalized;
        
        Quaternion targetRotation;
        var zAngle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        // Store the target rotation
        targetRotation = Quaternion.Euler(0,0, zAngle);


        transform.localRotation = targetRotation;
        Play();
    }

    public void Play()
    {
        StartCoroutine(PlayAndReturn());
        IEnumerator PlayAndReturn()
        {
            VFX.Play();
            yield return new WaitUntil(HasStopped);
            _followTarget = null;
            _prefabPool.Value.Return(gameObject);
        }
    }

   

    private bool HasStopped()
    {
        return !VFX.isPlaying;
    }
}

