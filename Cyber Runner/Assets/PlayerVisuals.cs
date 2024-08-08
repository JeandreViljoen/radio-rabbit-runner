using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Transform TVFollowPositionParent;

    private Vector3 _playerFollowPosition;
    [SerializeField] private bool _flipSpriteSafe;
    [SerializeField] private Transform _safeZoneFollowPosition;
    
    [SerializeField] private bool _flipSpriteStart;
    [SerializeField] private Transform _startFollowPosition;
    
    [SerializeField] private bool _flipSpriteStartDraft;
    [SerializeField] private Transform _startDraftFollowPosition;
    
    [SerializeField] private bool _flipSpriteDead;
    [SerializeField] private Transform _deadFollowPosition;
    private LazyService<GameStateManager> _stateManager;

    private Tween _moveTween;
    
    private void Awake()
    {
        _playerFollowPosition = TVFollowPositionParent.localPosition;
    }

    void Start()
    {
        _stateManager.Value.OnStateChanged += UpdateTvPosition;
    }
    
    void Update()
    {
        
    }

    public void UpdateTvPosition(GameState from, GameState to)
    {
        
        //TODO: flip code
        if (to == GameState.Safe)
        {
            SetTvPosition(TVPosition.Safe, 3);
        }
        if (to == GameState.Playing)
        {
            SetTvPosition(TVPosition.Player, 3);
        }
        if (to == GameState.Start)
        {
            SetTvPosition(TVPosition.Start, 3);
        }
        if (to == GameState.StartDraft)
        {
            SetTvPosition(TVPosition.StartDraft, 3);
        }
        if (to == GameState.Dead)
        {
            SetTvPosition(TVPosition.Dead, 3);
        }
    }

    public void SetTvPosition(TVPosition pos, float speed)
    {
        _moveTween?.Kill();
        _moveTween = TVFollowPositionParent.DOLocalMove(GetTarget(pos), speed).SetEase(Ease.InOutCubic);
    }

    public Vector3 GetTarget(TVPosition pos)
    {
        switch (pos)
        {
            case TVPosition.Player:
                return _playerFollowPosition;
                break;
            case TVPosition.Safe:
                return _safeZoneFollowPosition.localPosition;
                break;
            case TVPosition.Start:
                return _startFollowPosition.localPosition;
                break;
            case TVPosition.StartDraft:
                return _startDraftFollowPosition.localPosition;
                break;
            case TVPosition.Dead:
                return _deadFollowPosition.localPosition;
                break;
            default:
                return _playerFollowPosition;
        }
    }
    
    
}

public enum TVPosition
{
    Player,
    Safe,
    Start,
    StartDraft,
    Dead
}
