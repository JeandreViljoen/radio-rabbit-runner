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
    PlayerController _player;
    private Color gizmoColor;
    void Start()
    {
        if (_player == null) _player = ServiceLocator.GetService<PlayerController>();
    }
    
    void Update()
    {
        transform.Rotate(Vector3.forward, 30*Time.deltaTime);

        if (_player == null) _player = ServiceLocator.GetService<PlayerController>();
        
        if (TargetType == TargetingType.Closest && _player.Targets.ClosestEnemy != null)
        {
            transform.position = _player.Targets.ClosestEnemy.transform.position;
            gizmoColor = Color.red;
        }
        
        if (TargetType == TargetingType.Furthest && _player.Targets.FurthestEnemy != null)
        {
            transform.position = _player.Targets.FurthestEnemy.transform.position;
            gizmoColor = Color.blue;
        }
        
        if (TargetType == TargetingType.HighestHealth && _player.Targets.HighestHealth != null)
        {
            transform.position = _player.Targets.HighestHealth.transform.position;
            gizmoColor = new Color(1,0.5f,0,1);
        }
        
        if (TargetType == TargetingType.LowestHealth && _player.Targets.LowestHealth != null)
        {
            transform.position = _player.Targets.LowestHealth.transform.position;
            gizmoColor = Color.green;
        }

    }
    

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(transform.position, _player.transform.position);
        }
    }

    
}
