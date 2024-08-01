using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    #region ShowSpeedAttributes
    
    [HorizontalGroup("MAIN", MarginLeft = 0.2f, MarginRight = 0.2f)]
    [VerticalGroup("MAIN/Left")]
    [BoxGroup("MAIN/Left/Speeds")]
    [HorizontalGroup("MAIN/Left/Speeds/one")]
    [LabelWidth(100)]
    [ShowInInspector]
    
    #endregion
    public float ShowSpeed { get; private set; } = 0.1f;
    #region ShowEaseAttributes

    [HorizontalGroup("MAIN/Left/Speeds/one")]
    [ShowInInspector]

    #endregion
    public Ease ShowEase { get; private set; } = Ease.InOutSine;
    #region HideSpeedAttributes

    [BoxGroup("MAIN/Left/Speeds")]
    [HorizontalGroup("MAIN/Left/Speeds/two")]
    [LabelWidth(100)]
    [ShowInInspector]

    #endregion
    public float HideSpeed = 0.1f;
    #region HideEaseAttributes

    [HorizontalGroup("MAIN/Left/Speeds/two")]
    [ShowInInspector] 

    #endregion
    public Ease HideEase { get; private set; } = Ease.InOutSine;
    #region HighlightSpeedAttributes

    [BoxGroup("MAIN/Left/Speeds")]
    [HorizontalGroup("MAIN/Left/Speeds/three")]
    [LabelWidth(100)]
    [ShowInInspector] 

    #endregion
    public float HighlightSpeed { get; private set; } = 0.1f;
    #region HighlightEaseAttributes

    [HorizontalGroup("MAIN/Left/Speeds/three")]
    [ShowInInspector] 

    #endregion
    public Ease HighlightEase { get; private set; } = Ease.InOutSine;
    [ShowInInspector] public bool ActiveWhilePaused { get; private set; } = true;
    private Vector3 _showPosition;
    private Vector3 _hidePosition;
    private Vector3 _highlightPosition;
    public Vector3 RelativeHidePosition;
    public Vector3 RelativeHighlightPosition;
    
    private Tween _moveTween;
    void Awake()
    {
        _showPosition = transform.localPosition;
        _hidePosition = _showPosition + RelativeHidePosition;
        _highlightPosition = _showPosition + RelativeHighlightPosition;
    }

    private void Start()
    {
        transform.localPosition = _hidePosition;
    }

    public event Action OnShowStart;
    public event Action OnShowEnd;
    public event Action OnHideStart;
    public event Action OnHideEnd;

    public void Show()
    {
        _moveTween?.Kill();

        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => { OnShowStart?.Invoke(); });
        s.Append(transform.DOLocalMove(_showPosition, ShowSpeed).SetEase(ShowEase).SetUpdate(ActiveWhilePaused));
        s.AppendCallback(() => { OnShowEnd?.Invoke(); });
        _moveTween = s;
    }

    public void Hide()
    {
        _moveTween?.Kill();
        
        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => { OnHideStart?.Invoke(); });
        s.Append(transform.DOLocalMove(_hidePosition, HideSpeed).SetEase(HideEase).SetUpdate(ActiveWhilePaused));
        s.AppendCallback(() => { OnHideEnd?.Invoke(); });
        _moveTween = s;
    }
    
    public void Highlight()
    {
        _moveTween?.Kill();
        _moveTween = transform.DOLocalMove(_highlightPosition, HighlightSpeed).SetEase(HighlightEase).SetUpdate(ActiveWhilePaused);
    }
    
    public void InstantShow()
    {
        _moveTween?.Kill();
        transform.localPosition = _showPosition;
    }

    public void InstantHide()
    {
        _moveTween?.Kill();
        transform.localPosition = _hidePosition;
    }
    
    public void InstantHighlight()
    {
        _moveTween?.Kill();
        transform.localPosition = _highlightPosition;
    }
    
}
