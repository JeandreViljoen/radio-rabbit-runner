using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textField;
    private Tween _moveTween;
    private Tween _fadeTween;
    public float Height;
    public float Duration;
    private LazyService<PrefabPool> _prefabPool;
    public float RandomRadius;

    void Start()
    {
        _textField.text = "";
    }
    
    public void SetDamageAndShow(int damage)
    {
        Vector3 targetLocation = Vector3.up * Height;
        float randomX = UnityEngine.Random.Range(-RandomRadius , RandomRadius);
        float randomY = UnityEngine.Random.Range(0 , RandomRadius);
        targetLocation = new Vector3(targetLocation.x + randomX, targetLocation.y + randomY, targetLocation.z);
        
        _textField.text = damage.ToString();
        _textField.color = Color.white;
        _textField.transform.localPosition = Vector3.zero;
        _moveTween.Kill();
        Sequence s = DOTween.Sequence();
        s.Append(_textField.transform.DOLocalMove( targetLocation , Duration/2)
            .SetEase(Ease.OutSine));
        s.Append(_textField.DOFade(0f, Duration/2).SetEase(Ease.InOutSine));
        s.AppendCallback(() => { _prefabPool.Value.Return(gameObject);});
        _moveTween = s;
    }

    private void OnEnable()
    {
       
        
    }
    
    
}
