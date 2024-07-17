using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnRate = 1f;
    public RectTransform rect;
    private LazyService<PrefabPool> _pool;

    private int index = 0;

    public bool IsActive = true;
    void Start()
    {
        StartCoroutine(SpawnTicker());
    }
    
    void Update()
    {
        
    }

    private IEnumerator SpawnTicker()
    {
        PlayerController _player = ServiceLocator.GetService<PlayerController>();
        
        while (!_player.IsDead())
        {
            yield return new WaitForSeconds(SpawnRate);
            if(!IsActive && !ServiceLocator.GetService<EnemiesManager>().IsBlocked()) continue;
            
            Enemy enemy = _pool.Value.Get(EnemyPrefab).GetComponent<Enemy>();
            enemy.name = index.ToString();
            index++;
            enemy.transform.parent = ServiceLocator.GetService<EnemiesManager>().transform;
            float xPos = Random.Range(transform.position.x - rect.rect.width / 2,
                transform.position.x + rect.rect.width / 2);
            float yPos = Random.Range(transform.position.y - rect.rect.height / 2,
                transform.position.y + rect.rect.height / 2);


            enemy.transform.position = new Vector2(xPos, yPos);
        }
        
    }
}
