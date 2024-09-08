using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class StatItemUI : MonoBehaviour
{
    [SerializeField] private StatType _type;
    
    [FoldoutGroup("References")][SerializeField] private GameObject _child;
    [FoldoutGroup("References")][SerializeField] private TextMeshProUGUI _labelField;
    [FoldoutGroup("References")][SerializeField] private TextMeshProUGUI _valueField;

    private Tween _moveTween;
    [SerializeField] private Vector3 _flyInOffset;
    private Vector3 _showPosition;
    private Vector3 _hidePosition;
    private LazyService<StatsTracker> _stats;

    private void Awake()
    {
        _showPosition = _child.transform.localPosition;
        SetFlyInOffset(_flyInOffset);
    }

    public void SetValueFieldManualAndShow(string value, float delay, float speed)
    {
        _valueField.text = value;
        Show(delay, speed);
    }
    
    public void SetValueFieldAndShow(float delay = 0f, float speed = 0.3f)
    {
        _valueField.text = _stats.Value.GetStatValue(_type).ToString();
        Show(delay, speed);
    }

    private void Show(float delay, float speed)
    {
        _moveTween?.Kill();
        Sequence m = DOTween.Sequence();
        m.AppendInterval(delay);
        m.Append(_child.transform.DOLocalMove(_showPosition, speed).SetEase(Ease.OutCubic));
        _moveTween = m;
    }
    
    public void Hide(float delay, float speed)
    {
        _moveTween?.Kill();
        Sequence m = DOTween.Sequence();
        m.AppendInterval(delay);
        m.Append(_child.transform.DOLocalMove(_hidePosition, speed).SetEase(Ease.OutCubic));
        _moveTween = m;
    }

    public void SetFlyInOffset(Vector3 offset)
    {
        _hidePosition = new Vector3(_showPosition.x + offset.x, _showPosition.y + offset.y);
        ResetPosition();
    }

    public void ResetPosition()
    {
        _child.transform.localPosition = _hidePosition;
    }
}

