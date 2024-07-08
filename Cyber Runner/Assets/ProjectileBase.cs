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

    public int PierceCount = 0;

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
                _targetLocation = value.transform.localPosition;
                direction = (_targetLocation - transform.localPosition).normalized;
            }
        }
    }

    public float Speed;
    public int Damage;

    private Vector3 _targetLocation;
    private Vector2 direction = Vector2.zero;
    private LazyService<PrefabPool> _prefabPool;
    private LazyService<ProjectileManager> _projectileManager;

    public SpriteRenderer Renderer;
    void Start()
    {
        
    }

    
    void Update()
    {
        if (TargetEntity == null)
        {
            return;
        }
        //UpdateTargets();
        DoFire();
    }

    private void UpdateTargets()
    {
        switch (Type)
        {
            case ProjectileType.Bullet:
                break;
            case ProjectileType.Rocket:
                _targetLocation = TargetEntity.transform.localPosition;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void DoImpact()
    {
        PierceCount--;
        if (PierceCount < 0)
        {
            _targetEntity = null;
            _prefabPool.Value.Return(gameObject);
            PierceCount = 0;
        }
    }

    public void DoFire()
    {
        if (Vector2.Distance(transform.localPosition, transform.parent.localPosition) >= _projectileManager.Value.CullDistance)
        {
            DoImpact();
            return;
        }
        //Vector2 direction = (_targetLocation - transform.localPosition).normalized;
        
        switch (Type)
        {
            case ProjectileType.Bullet:
                transform.localPosition += (Vector3) (direction * Speed * Time.deltaTime);
                break;
            case ProjectileType.Rocket:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<Enemy>().Health.RemoveHealth(Damage);
            DoImpact();
        }
    }
}

public enum ProjectileType
{
    Bullet,
    Rocket,
    
}
