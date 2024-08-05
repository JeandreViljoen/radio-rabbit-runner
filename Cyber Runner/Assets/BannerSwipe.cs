using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BannerSwipe : MonoBehaviour
{
    private Tween _swipeTween;
    [SerializeField] private TextMeshProUGUI _textField;

    private Vector3 _startPos;
    private Vector3 _showPosA;
    private Vector3 _showPosB;
    private Vector3 _hidePos;

    public float Width;
    public float PanWidth;

    public float ShowTime;

    private void Awake()
    {
        _showPosA = new Vector3(transform.localPosition.x + PanWidth/2, transform.localPosition.y);
        _showPosB = new Vector3(_showPosA.x - PanWidth, _showPosA.y);
        _startPos = new Vector3(_showPosA.x + Width/2, _showPosA.y);
        _hidePos = new Vector3(_showPosB.x - Width/2, _showPosB.y);

        transform.localPosition = _startPos;
    }

    private void Start()
    {
        _swipeTween = transform.DOLocalMove(_startPos, 0.000001f).SetUpdate(true);
    }

    public void DoSwipe()
    {
        if (_swipeTween.IsPlaying()) return;
        
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMove(_showPosA, 0.2f).SetEase(Ease.Linear));
        s.Append(transform.DOLocalMove(_showPosB, ShowTime).SetEase(Ease.Linear));
        s.Append(transform.DOLocalMove(_hidePos, 0.5f).SetEase(Ease.InCubic));
        s.AppendCallback(() => { transform.localPosition = _startPos; });

        _swipeTween = s;
    }

    public void SetText(string text)
    {
        _textField.text = text;
    }
}
