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
    private LazyService<GameStateManager> _stateManager;
    
    public float PercentageSpeedIncreasePerEndlessLevel = 0.1f;
    public float PercentageSpawnRateIncreasePerEndlessLevel = 0.02f;
        

    public int SafeLevelBlockCheckpoint = 3;

    private int _blockCounter = 0;
    private int _targetBlockCount = 0;

    private int _currentLevelBlockCount = 0;

    public bool IsEndless = false;
    public int EndlessLevelCount = 0;
    

    public void AdvanceBlockCount()
    {
        _blockCounter++;

        if (_blockCounter == _targetBlockCount)
        {
            _blockCounter = 0;
            _blockManager.Value.SafeZoneFlag = true;
        }
    }

    private int _currentRound = 0;

    public int CurrentRound
    {
        get
        {
            return _currentRound;
        }
        set
        {
            _currentRound = value;
            ServiceLocator.GetService<StatsTracker>().RoundReached = _currentRound;
            SetRoundProperties(_currentRound);
        }
    }

    public void AdvanceLevel()
    {
        CurrentRound++;
        ServiceLocator.GetService<HUDManager>().ShowGetReadyBanner(CurrentRound, 1f);
    }

    void Awake ()
    {
        _data = GlobalGameAssets.Instance.LevelData;
    }
    void Start()
    {
        _stateManager.Value.OnStateChanged += InitFirstLevel;
    }

    private void InitFirstLevel(GameState from, GameState to)
    {
        if (from == GameState.StartDraft && to == GameState.Playing)
        {
            AdvanceLevel();
        }
    }
    
    void Update()
    {
        
    }

    private void SetRoundProperties(int level)
    {
        if (level == 0) return;
        
        
        LevelInfo data;
        if (level > _data.NumberOfLevels)
        {
            data = _data.GetLevelInfo(_data.NumberOfLevels);
            IsEndless = true;
            EndlessLevelCount++;
        }
        else
        {
            data = _data.GetLevelInfo(level);
        }

        float endlessSpeedMult = 1 + PercentageSpeedIncreasePerEndlessLevel * EndlessLevelCount;
        float endlessSpawnRates = 1 - EndlessLevelCount * PercentageSpawnRateIncreasePerEndlessLevel;
                
        
        
        //Speed
        SetSpeed(data.Speed * endlessSpeedMult);
        //Enemies
        _enemiesManager.Value.SetEnemyTypesToSpawn(data.EnemyPrefabs);
        //Enemy spawn speed;
        _enemiesManager.Value.SetSpawnRates(data.SpawnInterval * endlessSpawnRates);
        //Refresh Block count + set target
        _blockCounter = 0;
        _targetBlockCount = data.BlockCount;
        //Update HUD
        _hudManager.Value.SetLevelDisplay(level);
    }

    public void SetSpeed(float speed)
    {
        _player.Value.ConstantForce.force = new Vector2( speed * 2, 0f);
    }
    
    public void SetSafeSpeed()
    {
        _player.Value.ConstantForce.force = new Vector2( _data.GetLevelInfo(0).Speed * 2, 0f);
    }

    public List<GameObject> GetAllUniqueLevelBlocks()
    {
        return _data.GetAllUniqueLevelBlocks();
    }
}


