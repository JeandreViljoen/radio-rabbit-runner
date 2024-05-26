using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;

public enum EnemyState
{
    Active,
    Dead
}

public class Enemy : MonoBehaviour
{

    public Vector2 Target;
    public float MoveSpeed = 1;
    private float additionalMoveSpeed = 0;
    public float MomentumMultiplier = 2;
    public Health Health;
    public SpriteRenderer Renderer;

    private LazyService<PlayerMovement> _player;

    public EnemyState State = EnemyState.Active;

    void Start()
    {
        Health.OnHealthZero += StartOnDeathBehavior;
    }

    private void OnEnable()
    {
        Renderer.color = Color.white;
        State = EnemyState.Active;
    }

    void FixedUpdate()
    {
        if (State == EnemyState.Dead )
        {
            transform.position =
                Vector2.MoveTowards(this.transform.position, _player.Value.transform.position, (MoveSpeed +additionalMoveSpeed) * Time.deltaTime);
        }
        else
        {
            additionalMoveSpeed = (_player.Value.SpeedDelta * -1 / _player.Value.TheoreticalMaxSpeed) * MomentumMultiplier;
        
            //Vector2 targetUnitVector = (_player.Value.transform.position - transform.position);
            transform.position =
                Vector2.MoveTowards(this.transform.position, _player.Value.transform.position, (MoveSpeed + Math.Abs(additionalMoveSpeed)) * Time.deltaTime);
        }
      
    }

    private void ReturnToPool()
    {
        ServiceLocator.GetService<PrefabPool>().Return(gameObject);
    }

    private void StartOnDeathBehavior()
    {
        State = EnemyState.Dead;
        StartCoroutine(OnDeathBehavior(0.3f));
    }

    IEnumerator OnDeathBehavior (float preDeathTime)
    {
        transform.parent = null;
        Tween t = Renderer.transform.DOShakePosition(preDeathTime, 0.3f, 15);
        Renderer.color = Color.red;
        
        yield return new WaitForSeconds(preDeathTime/2);
        Renderer.DOFade(0f, preDeathTime / 2);
        
        yield return new WaitForSeconds(preDeathTime/2);
       
        ReturnToPool();
    }
    

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Health.RemoveHealth(99999);
        }
    }

    private void OnDestroy()
    {
        Health.OnHealthZero -= StartOnDeathBehavior;
    }
}

