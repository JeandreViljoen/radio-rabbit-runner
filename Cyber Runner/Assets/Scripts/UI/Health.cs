using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
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

    private LazyService<UpgradesManager> _upgradesManager;
    private LazyService<PlayerController> _player;

    public event Action OnHealthZero; 
    public event Action<int> OnHealthLost; 
    public event Action OnHealthGained; 


    public void AddHealth(int value)
    {
        CurrentHealth = Math.Min(CurrentHealth + value, MaxHealth);
        if(Type == HealthType.Player) ServiceLocator.GetService<HUDManager>().SetHealthDisplay(CurrentHealth, 5f);
        OnHealthGained?.Invoke();
    }
    
    public bool RemoveHealth(int value)
    {
        if (_isInvulnerable)
        {
            return false;
        }

        int modifiedValue = value;

        if (Type == HealthType.Enemy)
        {
            if (_upgradesManager.Value.HasPerkGroup(PerkGroup.AirDamage, out float damageIncreasePercent))
            {
                if (!_player.Value.IsGrounded)
                {
                    float temp = 1f +(damageIncreasePercent / 100f);
                    temp *= modifiedValue;
                    modifiedValue = (int)temp;
                    //Debug.LogError($"HEALTH.CS   :     Damage Increase from being airborne:  original:{value}|AB:{modifiedValue}");
                }
            }
        }
        
        //Death blow
        if (CurrentHealth - modifiedValue <= 0)
        {
            if (Type == HealthType.Player)
            {
                ServiceLocator.GetService<HUDManager>().SetHealthDisplay(0);
                ServiceLocator.GetService<StatsTracker>().DamageTaken += CurrentHealth;
            }
            else if (Type == HealthType.Enemy)
            {
                if (modifiedValue < 999)
                {
                    ServiceLocator.GetService<StatsTracker>().DamageDealt += modifiedValue;
                    ServiceLocator.GetService<StatsTracker>().EnemiesKilled ++;
                }
            }
            OnHealthLost?.Invoke(CurrentHealth);
            OnHealthZero?.Invoke();
            return true;
        }

        //Normal damage
        CurrentHealth = CurrentHealth - modifiedValue;
        OnHealthLost?.Invoke(modifiedValue);
        if (Type == HealthType.Player)
        {
            ServiceLocator.GetService<HUDManager>().SetHealthDisplay(CurrentHealth);
            ServiceLocator.GetService<HUDManager>().FlashRed();
            AudioManager.PostEvent(AudioEvent.ENEMY_HITMARK);
            ServiceLocator.GetService<StatsTracker>().DamageTaken += modifiedValue;
        }
        else if (Type == HealthType.Enemy)
        {
            if (modifiedValue < 999)
            {
                ServiceLocator.GetService<StatsTracker>().DamageDealt += modifiedValue;
            }
           
        }
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
