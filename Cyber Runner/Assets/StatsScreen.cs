using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsScreen : MonoBehaviour
{
    [SerializeField] private List<StatItemUI> Stats;
    [SerializeField] private float _statRevealInterval = 0.15f;
    [SerializeField] private float _statRevealSpeed = 0.3f;
    [SerializeField] private Image _background;
    [SerializeField] private UIAnimation _titleBar;
    [SerializeField] private Image _character;
    [SerializeField] private UIAnimation _prompt;
    public bool LockInput = true;

    public bool OverrideWithGlobalFlyInOffset = true;
    [SerializeField, ShowIf("OverrideWithGlobalFlyInOffset")] private Vector3 _flyInOffset;

    private LazyService<StatsTracker> _stats;
    void Start()
    {
        gameObject.SetActive(false);
        Color c = _background.color;
        c.a = 0f;
        _background.color = c;
    }

    private void SetFlyInOffsets()
    {
        foreach (var stat in Stats)
        {
            stat.SetFlyInOffset(_flyInOffset );
        }
    }

    public void ShowDeadCharacterSprite()
    {
        Sequence s = DOTween.Sequence();
        s.AppendInterval(0f);
        s.Append(_character.DOFade(1f, 1f).SetEase(Ease.InOutSine));
    }
    

    public void ShowStats()
    {
        
        
        gameObject.SetActive(true);
        StartCoroutine(InputLockTimer(_statRevealInterval * Stats.Count + _statRevealSpeed));

        _background.DOFade(1f, 1f).SetEase(Ease.InOutSine);
        _titleBar.Show(1f);

        if (OverrideWithGlobalFlyInOffset)
        {
            SetFlyInOffsets();
        }

        for (int i = 0; i < Stats.Count; i++)
        {
            Stats[i].SetValueFieldAndShow(i*_statRevealInterval, _statRevealSpeed);
        }
        
    }

    public void HideStats()
    {
        for (int i = 0; i < Stats.Count; i++)
        {
            Stats[i].Hide(i*_statRevealInterval/2, _statRevealSpeed/2);
        }
    }
    
    IEnumerator InputLockTimer(float time)
    {
        LockInput = true;
        yield return new WaitForSeconds(time - 0.2f);
        _prompt.Show();
        yield return new WaitForSeconds(0.2f);
        LockInput = false;
    }
    
}

