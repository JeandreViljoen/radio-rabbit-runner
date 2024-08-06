using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class HUDManager : MonoService
{
    [SerializeField] private TextMeshProUGUI _speedField;
    [SerializeField] private TextMeshProUGUI _healthTextField;
    [SerializeField] private TextMeshProUGUI _levelTextField;
    
    [SerializeField] private BannerSwipe _safeZoneIncomingBanner;
    [SerializeField] private BannerSwipe _getReadyBanner;

    public void ShowSafeZoneIncomingBanner(float preDelay = 0f)
    {
        _safeZoneIncomingBanner.DoSwipe(preDelay);
    }
    
    public void ShowGetReadyBanner(int level,float preDelay = 0f)
    {
        _getReadyBanner.SetText($"LEVEL {level} - GET READY!");
        _getReadyBanner.DoSwipe(preDelay);
    }

    void Start()
    {
       
    }

    void Update()
    {
        
    }

    public void SetSpeedValue(float speed)
    {
        _speedField.text = "Speed: " + speed;
    }

    public void SetHealthDisplay(int amount)
    {
        _healthTextField.text = "HEALTH: " + amount;
    }
    
    public void SetLevelDisplay(int lvl)
    {
        _levelTextField.text = "LEVEL: " + lvl;
    }
}
