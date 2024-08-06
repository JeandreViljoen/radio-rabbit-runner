using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class StatsTracker : MonoService
{
    
    public int ScoreMultiplier = 50;
    
    public int RunDistance = 0;
    public int EnemiesKilled = 0;
    public int UpgradesDrafted = 0;
    public int LevelReached = 0;
    public int EXPGained = 0;
    public int RoundReached = 0;
    public int DashAmount = 0;
    public int JumpAmount = 0;
    public int DamageTaken = 0;
    public int DamageDealt = 0;
    public int Score = 0;

    public void ResetAllStats()
    {
        RunDistance = 0;
        EnemiesKilled = 0;
        UpgradesDrafted = 0;
        LevelReached = 0;
        RoundReached = 0;
        DashAmount = 0;
        JumpAmount = 0;
        DamageTaken = 0;
        DamageDealt = 0;
        Score = 0;
    }

    public void SaveAllStats()
    {
        //TODO
    }

    private void CalculateScore()
    {
        Score = 0;
        //Score += RunDistance;
        Score += EnemiesKilled;
        //Score += UpgradesDrafted;
        Score += LevelReached * ScoreMultiplier;
        Score += RoundReached * ScoreMultiplier;
        Score += DashAmount;
        Score += JumpAmount;
        Score -= DamageTaken;
        Score += (int)(DamageDealt / Math.Max(1, ScoreMultiplier));
    }

    public int GetStatValue(StatType stat)
    {
        int temp = 0;
        
        switch (stat)
        {
            case StatType.None:
                temp = 0;
                break;
            case StatType.RunDistance:
                temp = RunDistance;
                break;
            case StatType.EnemiesKilled:
                temp = EnemiesKilled;
                break;
            case StatType.UpgradesDrafted:
                temp = UpgradesDrafted;
                break;
            case StatType.LevelReached:
                temp = LevelReached;
                break;
            case StatType.EXPGained:
                temp = EXPGained;
                break;
            case StatType.RoundReached:
                temp = RoundReached;
                break;
            case StatType.DashAmount:
                temp = DashAmount;
                break;
            case StatType.JumpAmount:
                temp = JumpAmount;
                break;
            case StatType.DamageTaken:
                temp = DamageTaken;
                break;
            case StatType.DamageDealt:
                temp = DamageDealt;
                break;
            case StatType.Score:
                CalculateScore();
                temp = Score;
                break;
            default:
                temp = 0;
                break;
        }

        return temp;
    }
    
}

public enum StatType
{
    None,
    RunDistance,
    EnemiesKilled,
    UpgradesDrafted,
    LevelReached,
    EXPGained,
    RoundReached,
    DashAmount,
    JumpAmount,
    DamageTaken,
    DamageDealt,
    Score
}
