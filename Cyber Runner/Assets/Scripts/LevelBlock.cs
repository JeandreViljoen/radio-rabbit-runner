using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Services;
using UnityEngine;

public class LevelBlock : MonoBehaviour
{
    private LazyService<LevelBlockManager> _levelBlockManager;

    public Transform StartConnection;
    public Transform EndConnection;

    [CanBeNull] public LevelBlock NextBlock;
    [CanBeNull] public LevelBlock PreviousBlock;

    private LazyService<LevelManager> _levelManager;
    private LazyService<UIManager> _uiManager;
    private LazyService<GameStateManager> _stateManager;

    public bool IsPlayerInBlock = false;

    public bool IsSafeBlock;

    private int _distanceFromPlayer;

    public int DistanceFromPlayer
    {
        get
        {
            return _distanceFromPlayer;
        }
        set
        {
            _distanceFromPlayer = value;

            if (value == 0)
            {
                gameObject.name = $"LevelBlock [{value}] - ACTIVE";
            }
            else
            {
                gameObject.name = $"LevelBlock [{value}]";
            }
            

            if (NextBlock && _distanceFromPlayer >= 0)
            {
                //Propagate forwards
                NextBlock.DistanceFromPlayer = this._distanceFromPlayer + 1;
            }

            if (PreviousBlock && _distanceFromPlayer <= 0)
            {
                //Propagate Backwards
                PreviousBlock.DistanceFromPlayer = this._distanceFromPlayer - 1;
            }

            ValidateCulling();
        }
    }

    private void ValidateCulling()
    {
        if (_distanceFromPlayer < -_levelBlockManager.Value.BackBlockBuffer)
        {
            _levelBlockManager.Value.DestroyBlock(this);
        }
    }

    public event Action OnLevelBlockEnter;
    public event Action OnLevelBlockExit;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void SetPlayerActiveInBlock()
    {
        IsPlayerInBlock = true;
        DistanceFromPlayer = 0;
        _levelBlockManager.Value.ActiveBlock = this;
        OnLevelBlockEnter?.Invoke();
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //If player enters block
        if (other.CompareTag("Player"))
        {

            //Update player position
            SetPlayerActiveInBlock();
            //Append a new block to end
            _levelBlockManager.Value.SpawnBlockAtEnd();

            if (IsSafeBlock)
            {
                //If tagged safe: Turn off enemies and kill them, then draft random upgrades
                _stateManager.Value.ActiveState = GameState.Safe;

                // if (!_uiManager.Value.IsDrafting)
                // {
                //     
                // }
            }
            else
            {
                if (_stateManager.Value.ActiveState == GameState.Safe)
                {
                    //If tagged not safe: Resume gameplay
                    _stateManager.Value.ActiveState = GameState.Playing;
                }

                if (_stateManager.Value.ActiveState == GameState.Playing)
                {
                    _levelManager.Value.AdvanceBlockCount();
                }

            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            IsPlayerInBlock = false;
            OnLevelBlockExit?.Invoke();
        }
    }
}
