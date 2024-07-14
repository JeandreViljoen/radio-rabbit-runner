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
    private bool _isInvulnerable = false;
    public int CurrentHealth { get; private set; }

    public event Action OnHealthZero; 
    public event Action OnHealthLost; 


    public void AddHealth(int value)
    {
        CurrentHealth = Math.Min(CurrentHealth + value, MaxHealth);
    }
    
    public bool RemoveHealth(int value)
    {
        if (_isInvulnerable)
        {
            return false;
        }
        
        if (CurrentHealth - value <= 0)
        {
            OnHealthZero?.Invoke();
            return true;
        }

        CurrentHealth = CurrentHealth - value;
        OnHealthLost?.Invoke();
        return false;
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

    public void SetInvulnerable(bool flag)
    {
        _isInvulnerable = flag;
    }
}
