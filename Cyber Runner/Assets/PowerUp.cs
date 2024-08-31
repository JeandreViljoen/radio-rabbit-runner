using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Collider2D _collider;
    [SerializeField][EnumToggleButtons] private PowerupType _powerupType;
    [SerializeField] private float _value;

    private LazyService<PowerUpManager> _powerUpManager;
    private LazyService<PrefabPool> _prefabPool;
    private LazyService<HUDManager> _hudManager;
    public event Action OnPickup;
   
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _powerUpManager.Value.DoPowerupEffect(_powerupType, _value);
            _prefabPool.Value.Return(gameObject);
            OnPickup?.Invoke();
        }
    }

    private Tween _hoverTween;
    public void Hover()
    {
        _hoverTween?.Kill();
        _hoverTween = transform.DOLocalMove(transform.localPosition + Vector3.up, 1f).SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        
        
    }

    private void OnDisable()
    {
        if (gameObject.activeSelf)
        {
            _prefabPool.Value.Return(gameObject);
        }
    }
}

public enum PowerupType
{
    Health,
    EXP,
    AttackSpeed,
    Shield
}
