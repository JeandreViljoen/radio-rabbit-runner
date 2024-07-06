using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _height;
    private Vector3 _startPosition;
    private Vector3 _endPosition;


    private void Awake()
    {
        _startPosition = transform.localPosition;
        _endPosition = new Vector3(_startPosition.x, _startPosition.y + _height, _startPosition.z);
    }

    void Start()
    {
        DoHover();
    }

    
    void Update()
    {
        
    }

    void DoHover()
    {
        transform.DOLocalMove(_endPosition, _speed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        
    }
    
   
}
