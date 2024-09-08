using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> PowerUpPrefabs;

    [SerializeField]private bool _overrideGlobalSpawnChance = false;
    [SerializeField][Range(0f,1f)][ShowIf("_overrideGlobalSpawnChance")] private float _spawnChance = 0f;
    public bool IsActive { get; private set; } = false;

    private LazyService<PrefabPool> _prefabPool;

    public void InitPowerUp()
    {
        gameObject.SetActive(true);
        float spawnChance = _overrideGlobalSpawnChance
            ? _spawnChance
            : GlobalGameAssets.Instance.GlobalPowerupSpawnChance;
        
        float spawnRNG = UnityEngine.Random.Range(0f, 1f);

        if (spawnRNG <= spawnChance)
        {
            IsActive = true;
            int randomIndex = UnityEngine.Random.Range(0, PowerUpPrefabs.Count);
            
            SpawnPowerup(randomIndex);
        }

    }

    private void SpawnPowerup(int index)
    {
        PowerUp p = _prefabPool.Value.Get(PowerUpPrefabs[index]).GetComponent<PowerUp>();
        p.transform.position = transform.position;
        p.transform.SetParent(transform);
        p.Hover();
    }
}
