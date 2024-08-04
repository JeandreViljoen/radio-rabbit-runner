using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class LevelManager : MonoService
{
    public Grid WorldGrid;
    private LazyService<LevelBlockManager> _blockManager;
    private LevelData _data;
    private LazyService<PlayerController> _player;
    private LazyService<HUDManager> _hudManager;
    private LazyService<EnemiesManager> _enemiesManager;

    public int SafeLevelBlockCheckpoint = 3;

    private int _blockCounter = 0;
    private int _targetBlockCount = 0;

    private int _currentLevelBlockCount = 0;
    

    public void AdvanceBlockCount()
    {
        _blockCounter++;

        if (_blockCounter == _targetBlockCount)
        {
            _blockCounter = 0;
            _blockManager.Value.SafeZoneFlag = true;
        }
    }

    private int _currentLevel = 0;

    public int CurrentLevel
    {
        get
        {
            return _currentLevel;
        }
        set
        {
            _currentLevel = value;
            SetLevelProperties(_currentLevel);
        }
    }

    public void AdvanceLevel()
    {
        CurrentLevel++;
    }

    void Awake ()
    {
        _data = GlobalGameAssets.Instance.LevelData;
    }
    void Start()
    {
       
    }
    
    void Update()
    {
        
    }

    private void SetLevelProperties(int level)
    {
        if (level == 0) return;
        
        
        LevelInfo data;
        if (level > _data.NumberOfLevels)
        {
            data = _data.GetLevelInfo(_data.NumberOfLevels);
        }
        else
        {
            data = _data.GetLevelInfo(level);
        }
        
        
        //Speed
        _player.Value.ConstantForce.force = new Vector2( data.Speed * 2, 0f);
        //Enemies
        _enemiesManager.Value.SetEnemyTypesToSpawn(data.EnemyPrefabs);
        //Enemy spawn speed;
        _enemiesManager.Value.SetSpawnRates(data.SpawnInterval);
        //Refresh Block count + set target
        _blockCounter = 0;
        _targetBlockCount = data.BlockCount;
        //Update HUD
        _hudManager.Value.SetLevelDisplay(level);
    }
}
