using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Vector2 Target;
    public float MoveSpeed = 1;
    private float additionalMoveSpeed = 0;
    public float MomentumMultiplier = 2;

    private LazyService<PlayerMovement> _player;

    void Start() 
    {
        
    }
    
    void FixedUpdate()
    {
        additionalMoveSpeed = (_player.Value.SpeedDelta * -1 / _player.Value.TheoreticalMaxSpeed) * MomentumMultiplier;
        
        //Vector2 targetUnitVector = (_player.Value.transform.position - transform.position);
        transform.position =
            Vector2.MoveTowards(this.transform.position, _player.Value.transform.position, (MoveSpeed +additionalMoveSpeed) * Time.deltaTime);
    }
}
