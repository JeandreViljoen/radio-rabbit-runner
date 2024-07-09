using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

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
                direction = Quaternion.AngleAxis(UnityEngine.Random.Range(-Spread/2, Spread/2), Vector3.forward) * direction;
            }
        }
    }

    private Vector3 _targetStatic;

    public Vector3 TargetStatic
    {
        get
        {
            return _targetStatic;
        }
        set
        {
            _targetStatic = value;

            _targetLocation = value;
            direction = (_targetLocation - transform.localPosition).normalized;
        }
    }

    public float Speed;
    public int Damage;
    public int Spread = 0;

    protected Vector3 _targetLocation;
    protected Vector2 direction = Vector2.zero;
    protected LazyService<PrefabPool> _prefabPool;
    protected LazyService<ProjectileManager> _projectileManager;

    public SpriteRenderer Renderer;
    
    protected void Update()
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

    protected virtual void DoImpact()
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
            if ( col.gameObject.GetComponent<Enemy>().Health.RemoveHealth(Damage))
            {
                DoOnKillEffects();
            }

            DoOnHitEffects();
            DoImpact();
        }
    }

    public virtual void DoOnKillEffects()
    {
        
    }
    
    public virtual void DoOnHitEffects()
    {
        
    }
}

public enum ProjectileType
{
    Bullet,
    Rocket,
    
}
