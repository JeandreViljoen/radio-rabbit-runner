using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyByObject : MonoBehaviour
{
    
    void Start()
    {
        AudioManager.RegisterGameObj(gameObject);
    }
    
    void Update()
    {
        AudioManager.SetObjectPosition(gameObject,transform);
    }

    private void OnEnable()
    {
        //AudioManager.PostEvent(AudioEvent.AMB_FLYBY_START, gameObject);
        
    }

    private void OnDisable()
    {
        //AudioManager.PostEvent(AudioEvent.AMB_FLYBY_STOP, gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            AudioManager.PostEvent(AudioEvent.PL_GENERAL_WHOOSH);
        }
    }
}
