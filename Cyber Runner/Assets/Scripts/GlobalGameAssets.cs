using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Game Asset", menuName = "Custom Assets/New Global Game Asset")]
public class GlobalGameAssets : ScriptableObjectSingleton<GlobalGameAssets>
{
    public KeyCode JumpKey;
    public KeyCode DashKey;
    public KeyCode ConfirmKey;
    public KeyCode ExitKey;
    public KeyCode StartKey;
    public KeyCode RestartKey;

    public LevelData LevelData;
    
    
    [Title("Game Settings")]
    [Range(0f,1f)] public float GlobalPowerupSpawnChance = 0f;
}
