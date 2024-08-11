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
    private bool _flipFlag;
    
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
            _flipFlag = _flipSpriteSafe;
        }
        if (to == GameState.Playing)
        {
            SetTvPosition(TVPosition.Player, 3);
            _flipFlag = false;
        }
        if (to == GameState.Start)
        {
            SetTvPosition(TVPosition.Start, 3);
            _flipFlag = _flipSpriteStart;
        }
        if (to == GameState.StartDraft)
        {
            SetTvPosition(TVPosition.StartDraft, 3);
            _flipFlag = _flipSpriteStartDraft;
        }
        if (to == GameState.Dead)
        {
            SetTvPosition(TVPosition.Dead, 3);
            _flipFlag = _flipSpriteDead;
        }
    }

    private Tween _flipTween;
    public void SetTvPosition(TVPosition pos, float speed)
    {
        _moveTween?.Kill();
        Sequence s = DOTween.Sequence();
        s.Append(TVFollowPositionParent.DOLocalMove(GetTarget(pos), speed).SetEase(Ease.InOutCubic));
        _moveTween = s;
        
        _flipTween?.Kill();
        Sequence f = DOTween.Sequence();
        f.AppendInterval(speed * 0.7f);
        f.AppendCallback(() => { FlipLeftFlag?.Invoke(_flipFlag); });
        _flipTween = f;
    }

    public event Action<bool> FlipLeftFlag;

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
