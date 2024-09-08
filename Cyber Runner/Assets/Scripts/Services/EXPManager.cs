using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class EXPManager : MonoService
{

    [Title("EXP Manager", "Services", TitleAlignments.Centered)]
    [SerializeField] private int _startingEXPNeeded = 100;
    [ShowInInspector, ReadOnly] private int _currentEXPNeeded = 100;

    private LazyService<UpgradesManager> _upgradesManager;

    private int _unclaimedLevels = 0;
    private int _currentEXP = 0;
    public int UnclaimedLevels => _unclaimedLevels;

    [ShowInInspector, ReadOnly] public int CurrentEXP
    {
        get
        {
            return _currentEXP;
        }
        private set
        {
            ServiceLocator.GetService<StatsTracker>().EXPGained += value;
            _currentEXP = value;
            if (_currentEXP >= _currentEXPNeeded)
            {
                int leftover = _currentEXP - _currentEXPNeeded;

                _unclaimedLevels++;
                CurrentLevel++;
                CurrentEXP += leftover;
            }
            UpdateEXPBar();
        }
    }

    public bool TryClaimLevel()
    {
        if (_unclaimedLevels > 0)
        {
            _unclaimedLevels--;
            return true;
        }

        _unclaimedLevels = 0;
        return false;
    }

    public event Action OnLevelUp;
    
    [SerializeField] private float _growthRate = 1.2f;

    private int _currentLevel = 0;
    [ShowInInspector, ReadOnly] public int CurrentLevel
    {
        get
        {
            return _currentLevel;
        }
        private set
        {
            _currentLevel = value;
            _currentEXP = 0;
            _currentEXPNeeded = (int)(_currentEXPNeeded * _growthRate);
            OnLevelUp?.Invoke();
            ServiceLocator.GetService<StatsTracker>().LevelReached = _currentLevel;
        }
    }

    [GUIColor("grey")][FoldoutGroup("References")] public Image Image;
    [GUIColor("grey")][FoldoutGroup("References")] public TextMeshProUGUI TextField;

    private Tween _barFillTween;
    
    void Start()
    {
        _currentEXPNeeded = _startingEXPNeeded;
        CurrentEXP = 0;
    }

    private void UpdateEXPBar()
    {
        float cur = CurrentEXP;
        float max = _currentEXPNeeded;
        float amt = cur / max;
        
        _barFillTween?.Kill();
        _barFillTween = Image.DOFillAmount(amt, 0.3f).SetEase(Ease.InOutCubic);

        TextField.text = $"LEVEL {CurrentLevel}";

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddEXP(2000);
        }
    }

    public void AddEXP(int amount)
    {
        if (amount <= 0)
        {
            Help.Debug(GetType(), "AddEXP", "Tried to add 0 or negative experience. This should ideally not happen. Returning early for now");
            return;
        }

        float modifiedAmount = amount;
        
        if (_upgradesManager.Value.HasPerkGroup(PerkGroup.IncreaseEXP, out float val))
        {
            modifiedAmount *= 1 + val/100;
        }

        CurrentEXP += (int)modifiedAmount;
    }

    public void AddLevel()
    {
        AddEXP(Math.Abs( _currentEXPNeeded - _currentEXP));
    }
}
