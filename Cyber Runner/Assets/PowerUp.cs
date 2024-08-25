using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Collider2D _collider;
    [SerializeField][EnumToggleButtons] private PowerupType _powerupType;
    [SerializeField] private float _value;

    private LazyService<PowerUpManager> _powerUpManager;
   
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _powerUpManager.Value.DoPowerupEffect(_powerupType, _value);
        }
    }

}

public enum PowerupType
{
    Health,
    EXP,
    AttackSpeed,
    Shield
}
