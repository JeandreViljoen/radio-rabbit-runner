using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth;
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
        CurrentHealth = MaxHealth;
    }
    
    void Update()
    {
        
    }
}
