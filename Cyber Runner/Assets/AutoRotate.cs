using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class AutoRotate : MonoBehaviour
{
    public float Speed = 10;
    

    private void OnEnable()
    {
        
       transform.Rotate(new Vector3(0f,0f,UnityEngine.Random.Range(0,360)));
    }

    void Update()
    {
        transform.Rotate(new Vector3(0f,0f,Speed * Time.deltaTime));
    }

    
}
