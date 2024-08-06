using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class ParticleVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem VFX;
    private LazyService<PrefabPool> _prefabPool;

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

    public void Play()
    {
        StartCoroutine(PlayAndReturn());
        IEnumerator PlayAndReturn()
        {
            VFX.Play();
            yield return new WaitUntil(HasStopped);
            _prefabPool.Value.Return(gameObject);
        }
        
        VFX.Play();
    }

    private bool HasStopped()
    {
        return !VFX.isPlaying;
    }
}

