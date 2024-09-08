using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SafezoneTracker : MonoBehaviour
{

    [SerializeField] private Image _bar;
    [SerializeField] private Image _face;
    [SerializeField] private Transform _startPos;
    [SerializeField] private Transform _endPos;

    public LevelBlock PreFirstSafeBlock = null;

    private float _referenceDistance = 1f;

    private LazyService<PlayerController> _player;

    private Tween _moveTween;
    [SerializeField] private UIAnimation uiAnim;
    
    void Start()
    {
        uiAnim.InstantHide();
    }
    
    void Update()
    {
        if (IsInit())
        {
            SetBar();
        }
    }

    public bool IsInit()
    {
        return PreFirstSafeBlock != null;
    }

    public void InitBar(LevelBlock preFirstSafeBlock)
    {
        PreFirstSafeBlock = preFirstSafeBlock;
        _referenceDistance = Vector3.Distance(preFirstSafeBlock.EndConnection.position,
            _player.Value.transform.position);
        uiAnim.Show();
    }

    private void SetBar()
    {
        float distance = Vector3.Distance(PreFirstSafeBlock.EndConnection.position,
            _player.Value.transform.position);
        float coeff = distance/_referenceDistance;
        
        _bar.fillAmount = coeff;

        float faceProgress = 1 - coeff;
        float faceDistance =  Vector3.Distance(_startPos.position, _endPos.position);

        _face.transform.position = (_startPos.position + Vector3.right *(faceDistance * faceProgress));

        if (PreFirstSafeBlock.NextBlock != null && PreFirstSafeBlock.NextBlock.IsPlayerInBlock) 
        {
            PreFirstSafeBlock = null;
            uiAnim.Hide();
        }

    }
    
}
