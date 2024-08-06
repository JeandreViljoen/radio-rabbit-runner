using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class Health : MonoBehaviour
{
    [EnumToggleButtons]
    public HealthType Type;
    public int MaxHealth;
    public bool RandomHealth;
    public Vector2Int RandomHealthRange;
    private bool _isInvulnerable = false;
    public int CurrentHealth { get; private set; }

    public event Action OnHealthZero; 
    public event Action OnHealthLost; 
    public event Action OnHealthGained; 


    public void AddHealth(int value)
    {
        CurrentHealth = Math.Min(CurrentHealth + value, MaxHealth);
        if(Type == HealthType.Player) ServiceLocator.GetService<HUDManager>().SetHealthDisplay(CurrentHealth);
        OnHealthGained?.Invoke();
    }
    
    public bool RemoveHealth(int value)
    {
        if (_isInvulnerable)
        {
            return false;
        }
        
        if (CurrentHealth - value <= 0)
        {
            if(Type == HealthType.Player) ServiceLocator.GetService<HUDManager>().SetHealthDisplay(0);
            OnHealthLost?.Invoke();
            OnHealthZero?.Invoke();
            return true;
        }

        CurrentHealth = CurrentHealth - value;
        OnHealthLost?.Invoke();
        if(Type == HealthType.Player) ServiceLocator.GetService<HUDManager>().SetHealthDisplay(CurrentHealth);
        return false;
    }

    public void DelayedRemoveHealth(int value, float delay)
    {
        StartCoroutine(DelayDamage(delay));
        
        IEnumerator DelayDamage(float delay)
        {
            yield return new WaitForSeconds(delay);
            RemoveHealth(value);
        }
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
    
    [Button, GUIColor("$GetInvulnerableToggleColor")]
    public void ToggleInvulnerable()
    {
        _isInvulnerable = !_isInvulnerable;
    }

    private Color GetInvulnerableToggleColor
    {
        get
        {
            if (_isInvulnerable)
            {
                return Color.green;
            }
            else
            {
                return Color.white;
            }
        }
    }
}

public enum HealthType
{
    Player,
    Enemy,
    Destructible
}
