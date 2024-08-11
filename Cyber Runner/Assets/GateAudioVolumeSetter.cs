using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAudioVolumeSetter : MonoBehaviour
{
    [SerializeField][Range(0f,10f)] private float Volume;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            AudioManager.SetRTPCValue("GateVolume", Volume);
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            AudioManager.SetRTPCValue("GateVolume", 0);
        }
    }
}
