using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class EnemiesManager : MonoService
{
    
    public List<EnemySpawner> Spawners = new List<EnemySpawner>();
    public List<Enemy> AllEnemies = new List<Enemy>();
    

    private bool _forceBlockNewEnemySpawns = false;
    public event Action<Enemy> OnEnemyKilled;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    public void ToggleSpawners(bool flag)
    {
        foreach (var spawner in Spawners)
        {
            spawner.IsActive = flag;
        }
    }

    public void InvokeEnemyKilled(Enemy enemy)
    {
        OnEnemyKilled?.Invoke(enemy);
    }

    public void KillAllEnemies(float time = 0)
    {
        float interval = time/AllEnemies.Count;

        for (var i = AllEnemies.Count-1; i >= 0; i--)
        {
            var enemy = AllEnemies[i];
            enemy.Health.DelayedRemoveHealth(9999999, i*interval);
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!AllEnemies.Contains(enemy))
        {
            AllEnemies.Add(enemy);
        }
    }
    
    public void DeRegisterEnemy(Enemy enemy)
    {
        if (AllEnemies.Contains(enemy))
        {
            AllEnemies.Remove(enemy);
        }
    }

    public bool IsBlocked()
    {
        return _forceBlockNewEnemySpawns;
    }

    public void SetSpawnRates(float rate)
    {
        foreach (var spawner in Spawners)
        {

            float safetyRate = Math.Max(rate, 0.1f);
            
            spawner.SpawnRate = safetyRate;
        }
    }

    public void SetEnemyTypesToSpawn(List<GameObject> enemies)
    {
        foreach (var spawner in Spawners)
        {
            spawner.EnemyPrefabs = enemies;
        }
    }
}
