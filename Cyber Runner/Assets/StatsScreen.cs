using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatsScreen : MonoBehaviour
{
    [SerializeField] private List<StatItemUI> Stats;
    [SerializeField] private float _statRevealInterval = 0.15f;
    [SerializeField] private float _statRevealSpeed = 0.3f;
    [SerializeField] private GameObject _background;
    public bool LockInput = true;

    public bool OverrideWithGlobalFlyInOffset = true;
    [SerializeField, ShowIf("OverrideWithGlobalFlyInOffset")] private Vector3 _flyInOffset;

    private LazyService<StatsTracker> _stats;
    void Start()
    {
        gameObject.SetActive(false);
    }

   
    void Update()
    {
        
    }

   

    private void SetFlyInOffsets()
    {
        foreach (var stat in Stats)
        {
            stat.SetFlyInOffset(_flyInOffset );
        }
    }

    public void ShowStats()
    {
        gameObject.SetActive(true);
        StartCoroutine(InputLockTimer(_statRevealInterval * Stats.Count + _statRevealSpeed));
    
        if (OverrideWithGlobalFlyInOffset)
        {
            SetFlyInOffsets();
        }

        for (int i = 0; i < Stats.Count; i++)
        {
            Stats[i].SetValueFieldAndShow(i*_statRevealInterval, _statRevealSpeed);
        }
        
    }
    
    IEnumerator InputLockTimer(float time)
    {
        LockInput = true;
        yield return new WaitForSeconds(time);
        LockInput = false;
    }
    
}

