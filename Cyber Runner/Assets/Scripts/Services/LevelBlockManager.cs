using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class LevelBlockManager : MonoService
{

    [Title("Level Block Manager", "Services", TitleAlignments.Centered)]
    
    [SerializeField] private LevelBlock StartBlock;
    [SerializeField] private LevelBlock SafeBlock;
    [SerializeField] public bool SafeZoneFlag = false;
    [SerializeField] private List<GameObject> _levelBlockPrefabs;
    public List<LevelBlock> LoadedBlocks = new List<LevelBlock>();
    [Range(0,5)] public int FrontBlockBuffer  = 1;
    [Range(0,5)] public int BackBlockBuffer  = 1;

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
            newBlock = Instantiate(SafeBlock, ServiceLocator.GetService<LevelManager>().WorldGrid.transform).GetComponent<LevelBlock>();
        }
        else
        {
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

    private GameObject GetRandomBlockPrefabFromPool()
    {
        int selection = Random.Range(0, _levelBlockPrefabs.Count);
        return _levelBlockPrefabs[selection];
    }
}
