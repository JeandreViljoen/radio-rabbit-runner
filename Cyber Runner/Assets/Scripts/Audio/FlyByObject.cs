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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            AudioManager.PostEvent(AudioEvent.PL_GENERAL_WHOOSH);
        }
    }
}
