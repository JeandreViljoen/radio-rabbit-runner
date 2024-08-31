using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class DamageBorder : MonoBehaviour
{
    [SerializeField] private List<Sprite> Sprites;
    [SerializeField] private Image _renderer;
    [SerializeField] private float _duration = 0.5f;

    private Tween _flashTween;

    private LazyService<PlayerController> _player;
    void Start()
    {
        _renderer.color = new Color(1f, 1f, 1f, 0f);
        _player.Value.Health.OnHealthLost += Flash;
    }

   
    void Update()
    {
        
    }

    public void Flash(int _)
    {
        _flashTween?.Kill();
        _renderer.sprite = Sprites[UnityEngine.Random.Range(0, Sprites.Count)];
        _renderer.color = Color.white;
        _flashTween = _renderer.DOFade(0f, _duration).SetEase(Ease.InOutSine);
    }
}
