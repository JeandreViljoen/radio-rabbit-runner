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
    void Start()
    {
        StartCoroutine(SpawnTicker());
    }
    
    void Update()
    {
        
    }

    private IEnumerator SpawnTicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnRate);
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
