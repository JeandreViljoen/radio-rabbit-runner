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
    [SerializeField] private Vector3 _safeZoneFollowPosition;
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

    private void UpdateTvPosition(GameState from, GameState to)
    {
        if (to == GameState.Safe)
        {
            SetTvPosition(TVPosition.Safe, 3);
        }
        if (to == GameState.Playing)
        {
            SetTvPosition(TVPosition.Player, 3);
        }
    }

    public void SetTvPosition(TVPosition pos, float speed)
    {
        _moveTween?.Kill();
        _moveTween = TVFollowPositionParent.DOLocalMove(GetTarget(pos), speed).SetEase(Ease.InOutCubic);
    }

    private Vector3 GetTarget(TVPosition pos)
    {
        switch (pos)
        {
            case TVPosition.Player:
                return _playerFollowPosition;
                break;
            case TVPosition.Safe:
                return _safeZoneFollowPosition;
                break;
            default:
                return _playerFollowPosition;
        }
    }
    
    
}

public enum TVPosition
{
    Player,
    Safe
}
