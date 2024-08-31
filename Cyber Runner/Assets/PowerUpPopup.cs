using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpPopup : MonoBehaviour
{
    [SerializeField] private UIAnimation uiAnim;

    [SerializeField] private Image HealthIcon;
    [SerializeField] private Image ShieldIcon;
    [SerializeField] private Image LevelUpIcon;
    [SerializeField] private Image AttackSpeedIcon;

    [SerializeField] private TextMeshProUGUI HealthText;
    [SerializeField] private TextMeshProUGUI ShieldText;
    [SerializeField] private TextMeshProUGUI LevelUpText;
    [SerializeField] private TextMeshProUGUI AttackSpeedText;

    [SerializeField] private float _defaultTime;
    
    void Start()
    {
        uiAnim.OnHideEnd += DisableAll;
        DisableAll();
    }
    
    void Update()
    {
        
    }

    public void ShowPopup(PowerupType type, float time = 0)
    {
        if (TimerHandle != null)
        {
            StopCoroutine(TimerHandle);
            uiAnim.Hide();
        }

        StartCoroutine(DelayedFieldUpdates(type, 0.2f));
        TimerHandle = StartCoroutine(DelayedHide(time == 0 ? _defaultTime : time));
    }

    private Coroutine TimerHandle;
    
    IEnumerator DelayedHide(float time)
    {
        yield return new WaitForSeconds(time);
        uiAnim.Hide();
        TimerHandle = null;
    }

    IEnumerator DelayedFieldUpdates(PowerupType type, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        switch (type)
        {
            case PowerupType.Health:
                HealthIcon.GameObject().SetActive(true);
                HealthText.GameObject().SetActive(true);
                break;
            case PowerupType.EXP:
                LevelUpIcon.GameObject().SetActive(true);
                LevelUpText.GameObject().SetActive(true);
                break;
            case PowerupType.AttackSpeed:
                AttackSpeedText.GameObject().SetActive(true);
                AttackSpeedIcon.GameObject().SetActive(true);
                break;
            case PowerupType.Shield:
                ShieldIcon.GameObject().SetActive(true);
                ShieldText.GameObject().SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
        uiAnim.Show();
    }

    private void DisableAll()
    {
        HealthIcon.GameObject().SetActive(false);
        ShieldIcon.GameObject().SetActive(false);
        LevelUpIcon.GameObject().SetActive(false);
        AttackSpeedIcon.GameObject().SetActive(false);
        
        HealthText.GameObject().SetActive(false);
        ShieldText.GameObject().SetActive(false);
        LevelUpText.GameObject().SetActive(false);
        AttackSpeedText.GameObject().SetActive(false);
    }
}
