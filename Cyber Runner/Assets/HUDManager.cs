using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class HUDManager : MonoService
{
    [SerializeField] private TextMeshProUGUI _speedField;
    [SerializeField] private TextMeshProUGUI _healthTextField;
    [SerializeField] private TextMeshProUGUI _levelTextField;
    
    [SerializeField] private Image _healthBar;
    
    [SerializeField] private BannerSwipe _safeZoneIncomingBanner;
    [SerializeField] private BannerSwipe _getReadyBanner;
    [SerializeField] private BannerSwipe _deathReadyBanner;

    [SerializeField] public SafezoneTracker SafezoneTracker;
    [SerializeField] public PowerUpPopup PowerUpPopup;

    private LazyService<PlayerController> _player;
    public ColorCurves ColorCurves;
    public VolumeProfile VolumeProfile;

    public void ShowSafeZoneIncomingBanner(float preDelay = 0f)
    {
        _safeZoneIncomingBanner.DoSwipe(preDelay);
    }
    
    public void ShowGetReadyBanner(int level,float preDelay = 0f)
    {
        _getReadyBanner.SetText($"ROUND {level} - GET READY!");
        _getReadyBanner.DoSwipe(preDelay);
    }
    
    public void ShowDeathBanner(float preDelay = 0f)
    {
        _deathReadyBanner.DoSwipe(preDelay);
    }

    void Start()
    {
       SetHealthDisplay(100);
       if (VolumeProfile.TryGet<ColorCurves>(out ColorCurves)) ;
    }

    void Update()
    {
        
    }

    public void SetSpeedValue(float speed)
    {
        _speedField.text =   (int)(speed)   + " K/ph";
    }

    public void SetHealthDisplay(int amount, float speed = 0.3f)
    {
        _healthTextField.text = "HEALTH: " + amount;
        UpdateHealthBar(speed);
    }
    
    public void SetLevelDisplay(int lvl)
    {
        _levelTextField.text = "ROUND: " + lvl;
    }

    private Tween _barFillTween;
    private void UpdateHealthBar(float speed = 0.3f)
    {
        float cur = _player.Value.Health.CurrentHealth;
        float max = _player.Value.Health.MaxHealth;
        float amt = cur / max;
        
        _barFillTween?.Kill();
        _barFillTween = _healthBar.DOFillAmount(amt, speed).SetEase(Ease.InOutCubic);

    }

    private Coroutine _flashHandle;
    public void FlashRed()
    {
        if (_flashHandle != null)
        {
            StopCoroutine(_flashHandle);
        }
        
        _flashHandle = StartCoroutine(flashRedDelayed());
        
        IEnumerator flashRedDelayed()
        {
            ColorCurves.active = true;
            yield return new WaitForSeconds(0.1f);
            ColorCurves.active = false;
            _flashHandle = null;
        }
        
    }
}
