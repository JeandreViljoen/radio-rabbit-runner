using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class EnemiesManager : MonoService
{
    
    public List<EnemySpawner> Spawners = new List<EnemySpawner>();
    public List<Enemy> AllEnemies = new List<Enemy>();

    private bool _forceBlockNewEnemySpawns = false;
    
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
}
