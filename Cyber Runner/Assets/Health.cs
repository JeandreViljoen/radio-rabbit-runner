using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public bool RandomHealth;
    public Vector2Int RandomHealthRange;
    public int CurrentHealth { get; private set; }

    public event Action OnHealthZero; 


    public void AddHealth(int value)
    {
        CurrentHealth = Math.Min(CurrentHealth + value, MaxHealth);
    }
    
    public void RemoveHealth(int value)
    {
        if (CurrentHealth - value <= 0)
        {
            OnHealthZero?.Invoke();
            return;
        }

        CurrentHealth = CurrentHealth - value;
    }

    void Start()
    {
        InitHealth();
    }

    public void InitHealth()
    {
        if (RandomHealth)
        {
            int randomHealthRoll = UnityEngine.Random.Range(RandomHealthRange.x, RandomHealthRange.y + 1);
            float scaleRelation = 1 + Help.Map01(randomHealthRoll, RandomHealthRange.x, RandomHealthRange.y + 1);
            gameObject.transform.localScale = new Vector3(scaleRelation, scaleRelation, scaleRelation);
            MaxHealth = randomHealthRoll;
        }
        
        CurrentHealth = MaxHealth;
    }
    
    void Update()
    {
        
    }
}
