using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    
    [EnumToggleButtons]
    public ProjectileType Type;

    private GameObject _targetEntity;
    public GameObject TargetEntity
    {
        get
        {
            return _targetEntity;
        }

        set
        {
            _targetEntity = value;

            if (_targetEntity == null)
            {
                _targetLocation = Vector2.zero;
            }
            else
            {
                _targetLocation = value.transform.position;
            }
            
        }
    }

    public float Speed;
    public int Damage;

    private Vector3 _targetLocation;
    private LazyService<PrefabPool> _prefabPool;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (TargetEntity == null)
        {
            return;
        }
        UpdateTargets();
        DoFire();
    }

    private void UpdateTargets()
    {
        switch (Type)
        {
            case ProjectileType.Bullet:
                break;
            case ProjectileType.Rocket:
                _targetLocation = TargetEntity.transform.position;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void DoImpact()
    {
        _targetEntity = null;
        _prefabPool.Value.Return(gameObject);
    }

    public void DoFire()
    {
        Vector2 direction = _targetLocation - transform.position;
        
        switch (Type)
        {
            case ProjectileType.Bullet:
                transform.position += (Vector3) (direction * Speed * Time.deltaTime);
                break;
            case ProjectileType.Rocket:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
}

public enum ProjectileType
{
    Bullet,
    Rocket,
    
}
