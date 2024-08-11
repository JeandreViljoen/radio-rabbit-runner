using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Services;
using Unity.VisualScripting;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothing;
    [SerializeField] private float _smoothingMax;
    [SerializeField] private float _smoothingMin;
    private Vector3 _currentVelocity = Vector3.zero;
    [SerializeField] private float _smoothingRatio;

    [SerializeField] private float _zDistanceMin;
    [SerializeField] private float _zDistanceMax;

    private float zOffsetBasedOnSpeed = 0;
    private float xOffsetBasedOnSpeed = 0;


    [SerializeField] private Vector3 _playerOffset;
    [SerializeField] private Vector3 _safeOffset;
    [SerializeField] private Vector3 _deadOffset;
    [SerializeField] private Vector3 _startOffset;
    [SerializeField] private Vector3 _StartDraftOffset;

    private LazyService<GameStateManager> _stateManager;
    private float _startZ;

    private Camera cam;
    private Tween _zoomTween;

    private void Awake()
    {
        cam = Camera.main;
        _startZ = transform.position.z;
    }

    void Start()
    {
        _stateManager.Value.OnStateChanged += UpdateCameraPosition;
    }

    private void Update()
    {
        if (_player.ConstantForce.force.x / _player.RB.drag >= 1)
        {
            //TODO: ISSUE if drag is set to 0
            float smooth = Mathf.Max(_smoothingMin,_smoothingRatio / (_player.ConstantForce.force.x / _player.RB.drag));
            _smoothing = Mathf.Min(_smoothingMax, smooth);
        }
        else
        {
            _smoothing = _smoothingMax;
        }

        if (_stateManager.Value.ActiveState == GameState.Dead)
        {
            _smoothing = 0.3f;
        }

       // zOffsetBasedOnSpeed = Help.Map(_player.CurrentRunSpeed, 20, 30, 20f, 30f, true);
        zOffsetBasedOnSpeed = 30f;
        xOffsetBasedOnSpeed = Help.Map(_player.TheoreticalMaxSpeed, 0, 35, 10, 17, true);
    }


    void LateUpdate()
    {
        Vector3 targetPos = _player.gameObject.transform.position + _offset;
        
        targetPos = new Vector3(targetPos.x + xOffsetBasedOnSpeed, targetPos.y, _startZ); //-zOffsetBasedOnSpeed
        transform.position = Vector3.SmoothDamp(transform.position ,targetPos, ref _currentVelocity, _smoothing);
        
    }

   
    private float _zoomLevel;
    private void DoZoom(float speed)
    {
  
        _zoomTween?.Kill();
        cam.DOOrthoSize(_offset.z, speed).SetEase(Ease.InOutSine);

    }
    
    public void UpdateCameraPosition(GameState from, GameState to)
    {
        
        if (to == GameState.Safe)
        {
            _offset = _safeOffset;
            DoZoom(3f);
        }
        if (to == GameState.Playing)
        {
            _offset = _playerOffset;
            DoZoom(1.5f);
        }
        if (to == GameState.Start)
        {
            _offset = _startOffset;
            DoZoom(3f);
        }
        if (to == GameState.StartDraft)
        {
            _offset = _StartDraftOffset;
            DoZoom(3f);
        }
        if (to == GameState.Dead)
        {
            _offset = _deadOffset;
            DoZoom(5f);
        }
    }
    
}

