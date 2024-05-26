using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public enum TargetingType
{
    None,
    Closest,
    Furthest,
    HighestHealth,
    LowestHealth,
    Random,
    Direction
}

public class TargetFinder : MonoBehaviour
{
    public TargetingType TargetType;
    public SpriteRenderer TargetGraphic;
    private LazyService<PlayerMovement> _player;
    void Start()
    {
        
    }
    
    void Update()
    {
        if (TargetType == TargetingType.Closest && _player.Value.ClosestEnemy != null)
        {
            transform.position = _player.Value.ClosestEnemy.transform.position;
        }
        
        if (TargetType == TargetingType.Furthest && _player.Value.FurthestEnemy != null)
        {
            transform.position = _player.Value.FurthestEnemy.transform.position;
        }
      
    }

    
}
