using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Custom Assets/Level Data")]
public class LevelData : SerializedScriptableObject
{
    private void SetIDs()
    {
        for (int i = 0; i < LevelInformation.Count; i++)
        {
            LevelInformation[i].Level = i;
        }
    }

    [OnValueChanged("SetIDs")]
    [ListDrawerSettings(ShowIndexLabels = true) ]
    public List<LevelInfo> LevelInformation;

    public LevelInfo GetLevelInfo(int level)
    {
        if (level > NumberOfLevels)
        {
            return LevelInformation[^1];
        }
        return LevelInformation[level];
    }

    public int NumberOfLevels => LevelInformation.Count -1;


}

[Serializable]
public class LevelInfo
{
    [ReadOnly] public int Level;
    
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 30)]
    public float Speed;
    
    [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
    [BoxGroup("Enemies", centerLabel:true)] public float SpawnInterval;
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
    [BoxGroup("Enemies")] public List<GameObject> EnemyPrefabs;
    
    [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
    [BoxGroup("Blocks", centerLabel:true)] public int BlockCount = 1;
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
    [BoxGroup("Blocks")] public List<GameObject> Blocks;

}
