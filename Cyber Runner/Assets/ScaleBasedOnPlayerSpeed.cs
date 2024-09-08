using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;

public class ScaleBasedOnPlayerSpeed : MonoBehaviour
{
    private LazyService<PlayerController> _player;

    [SerializeField] private float _thresholdSpeed;
    [SerializeField] private float _scaleAmount;

    private bool _triggered = false;

    void Update()
    {
        if (_triggered)
        {
            return;
        }

        if (_player.Value.TheoreticalMaxSpeed > _thresholdSpeed)
        {
            transform.DOScale(gameObject.transform.localScale * _scaleAmount, 10f).SetEase(Ease.InOutSine);
            _triggered = true;
        }
    }
}
