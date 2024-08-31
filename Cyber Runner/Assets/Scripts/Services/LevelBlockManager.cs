using System;
using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LevelBlockManager : MonoService
{

    [Title("Level Block Manager", "Services", TitleAlignments.Centered)]
    
    [SerializeField] private LevelBlock StartBlock;
    [SerializeField] private GameObject StartPrefab;
    [SerializeField] private LevelBlock SafeBlock;

    private LazyService<GameStateManager> _stateManager;
    private LazyService<PrefabPool> _prefabPool;

    public bool IsInSafeZone => _activeBlock.IsSafeBlock;
    
    
    private bool _safeZoneFlag = false;

    public bool SafeZoneFlag
    {
        get
        {
            return _safeZoneFlag;
        }
        set
        {
            if (value == _safeZoneFlag)
            {
                return;
            }

            //On disable
            if (value == false)
            {
                _levelManager.Value.AdvanceLevel();
                ServiceLocator.GetService<PlayerController>().PlayerVisuals.SetTvPosition(TVPosition.Player, 3);
               ServiceLocator.GetService<GameplayManager>().CameraFollowTarget.UpdateCameraPosition(GameState.None, GameState.Playing);
               AudioManager.SetSwitch("Music", "Combat");
            }
            //on Enable
            else 
            {
                
                SafezoneTracker tracker = ServiceLocator.GetService<HUDManager>().SafezoneTracker;
                if (!tracker.IsInit())
                {
                    tracker.InitBar(LoadedBlocks[^1]);
                }
            }

            _safeZoneFlag = value;
        }
    }
    
    [FormerlySerializedAs("_levelBlockPrefabs")] [SerializeField] private List<GameObject> _defaultLevelBlockPrefabs;
    public List<LevelBlock> LoadedBlocks = new List<LevelBlock>();
    [Range(0,5)] public int FrontBlockBuffer  = 1;
    [Range(0,5)] public int BackBlockBuffer  = 1;

    private LazyService<LevelManager> _levelManager;

    private LevelBlock _activeBlock;

    public LevelBlock ActiveBlock
    {
        get
        {
            return _activeBlock;
        }
        set
        {
            _activeBlock = value;
        }
    }
    
    void Start()
    {
        InitBlocks();
    }
    
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     SafeZoneFlag = !SafeZoneFlag;
        // }
    }
    
    private void InitBlocks()
    {
        SafeBlock = GlobalGameAssets.Instance.LevelData.GetLevelInfo(0).Blocks[0].GetComponent<LevelBlock>();
        LevelBlock workingBlock = StartBlock;
        LoadedBlocks.Add(StartBlock);
        
        for (int i = 0; i < FrontBlockBuffer-1; i++)
        {
            LevelBlock newBlock = SpawnBlock(workingBlock);

            workingBlock = newBlock;
        }
        
        StartBlock.SetPlayerActiveInBlock();
    }
    
    private LevelBlock SpawnBlock(LevelBlock previousBlock)
    {
        LevelBlock newBlock;

        if (_stateManager.Value.ActiveState == GameState.Start  || _stateManager.Value.ActiveState == GameState.StartDraft)
        {
            //newBlock = Instantiate(StartPrefab, ServiceLocator.GetService<LevelManager>().WorldGrid.transform).GetComponent<LevelBlock>();
            newBlock = _prefabPool.Value.Get(StartPrefab).GetComponent<LevelBlock>();
        }
        else
        {
            if (SafeZoneFlag)
            {
                //Safe blocks
                newBlock = _prefabPool.Value.Get(GetRandomBlockPrefabFromPool(0)).GetComponent<LevelBlock>();
                //newBlock = Instantiate(GetRandomBlockPrefabFromPool(0), ServiceLocator.GetService<LevelManager>().WorldGrid.transform).GetComponent<LevelBlock>();
            }
            else
            {
                //Normal blocks
                newBlock = _prefabPool.Value.Get(GetRandomBlockPrefabFromPool()).GetComponent<LevelBlock>();
                //newBlock = Instantiate(GetRandomBlockPrefabFromPool(), ServiceLocator.GetService<LevelManager>().WorldGrid.transform).GetComponent<LevelBlock>();
            }
        }
        
        newBlock.transform.parent = ServiceLocator.GetService<LevelManager>().WorldGrid.transform;
        newBlock.transform.position = previousBlock.EndConnection.position;
        newBlock.PreviousBlock = previousBlock;
        previousBlock.NextBlock = newBlock;
        newBlock.DistanceFromPlayer = previousBlock.DistanceFromPlayer + 1;
        LoadedBlocks.Add(newBlock);

        return newBlock;
    }

    public void SpawnBlockAtEnd()
    {
        SpawnBlock(LoadedBlocks[^1]);
    }

    public void DestroyBlock(LevelBlock blockToDestroy)
    {
        blockToDestroy.NextBlock.PreviousBlock = null;
        blockToDestroy.PreviousBlock = null;
        blockToDestroy.NextBlock = null;
        LoadedBlocks.Remove(blockToDestroy);
        _prefabPool.Value.Return(blockToDestroy.gameObject);
        //Destroy(blockToDestroy.gameObject);
    }

    private GameObject GetRandomBlockPrefabFromPool(int forceLevel = -1)
    {
        int levelToGet = forceLevel < 0 ? _levelManager.Value.CurrentRound : forceLevel;
        
        LevelInfo currentLevelData = GlobalGameAssets.Instance.LevelData.GetLevelInfo(levelToGet);
        List<GameObject> pool;
        
        if (currentLevelData.Blocks.Count > 0)
        {
            pool = currentLevelData.Blocks;
        }
        else
        {
            pool = _defaultLevelBlockPrefabs;
        }
       
        int selection = Random.Range(0, pool.Count);
        return pool[selection];
    }
}
