using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class LevelManager : MonoService
{
    public Grid WorldGrid;
    private LazyService<LevelBlockManager> _blockManager;

    public int SafeLevelBlockCheckpoint = 3;

    private int _currentLevelBlockCount = 0;

    public int CurrentLevelBlockCount
    {
        get
        {
            return _currentLevelBlockCount;
        }
        set
        {
            _currentLevelBlockCount = value;

            if (_currentLevelBlockCount >= SafeLevelBlockCheckpoint)
            {
                _blockManager.Value.SafeZoneFlag = true;
            }
        }
    }
    public int CurrentLevel = 0;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
