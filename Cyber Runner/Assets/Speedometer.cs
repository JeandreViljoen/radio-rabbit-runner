using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    public Image Needle;
    public float MinSpeed = 0f;
    public float MaxSpeed = 50f;
    private LazyService<PlayerController> _player;
    public float Offset = 0f;
    public float Range = 180f;

    public float Dampening = 0.1f;

    private Tween _rotationTween;
    private void Update()
    {
        
        //Vector3 f = Quaternion.Euler(0f,0f, GetAngleFromSpeed());
        
        Vector3 rotation = new Vector3(0f, 0f, GetAngleFromSpeed());
        
        _rotationTween?.Kill();
        Needle.transform.DORotate(rotation, Dampening);
    }

    private float GetAngleFromSpeed()
    {
        float coeff = Help.Map01(_player.Value.CurrentRunSpeed, MinSpeed, MaxSpeed, true);

        return Range - (Range * coeff + Offset);
    }
}
