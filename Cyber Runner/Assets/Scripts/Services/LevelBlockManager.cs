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
    [SerializeField] private LevelBlock SafeBlock;

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
            }
            //on Enable
            else 
            {
                //TODO: SHOw UI that safety is incoming
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            SafeZoneFlag = !SafeZoneFlag;
        }
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
        
        if (SafeZoneFlag)
        {
            //Safe blocks
            newBlock = Instantiate(GetRandomBlockPrefabFromPool(0), ServiceLocator.GetService<LevelManager>().WorldGrid.transform).GetComponent<LevelBlock>();
        }
        else
        {
            //Normal blocks
            newBlock = Instantiate(GetRandomBlockPrefabFromPool(), ServiceLocator.GetService<LevelManager>().WorldGrid.transform).GetComponent<LevelBlock>();
        }
        
        
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
        LoadedBlocks.Remove(blockToDestroy);
        Destroy(blockToDestroy.gameObject);
    }

    private GameObject GetRandomBlockPrefabFromPool(int forceLevel = -1)
    {
        int levelToGet = forceLevel < 0 ? _levelManager.Value.CurrentLevel : forceLevel;
        
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
