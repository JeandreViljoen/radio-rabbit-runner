using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro.EditorUtilities;
using UnityEngine;

public class GameStateManager : MonoService
{
    public event Action<GameState, GameState> OnStateChanged;
    private GameState _oldStateTransitionStorage;//only used for OnStateChange event
    private GameState _activeState;
    public GameState ActiveState
    {
        get
        {
            return _activeState;
        }
        set
        {

            if (value == _activeState)
            {
                Help.Debug(GetType(), "ActiveState", $"Tried to change game state to {value}, which is already set.");
                return;
            }
            
            _oldStateTransitionStorage = _activeState;//only used for OnStateChange event
            OnStateExit(_activeState, value);
            _activeState = value;
            OnStateEnter(_activeState, _oldStateTransitionStorage);
            OnStateChanged?.Invoke(_oldStateTransitionStorage, _activeState);
        }
    }
    
    void Start()
    {
        ActiveState = GameState.Start;
    }
    
    void Update()
    {
        UpdateState(ActiveState);
    }

    private void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.None:
                break;
            case GameState.Start:
                break;
            case GameState.StartDraft:
                break;
            case GameState.Playing:
                break;
            case GameState.Safe:
                break;
            case GameState.Dead:
                break;
            default:
                Help.Debug(GetType(), "UpdateState", "tried to execute an enum that has not been implemented");
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void OnStateExit(GameState exitingState, GameState to)
    {
        switch (exitingState)
        {
            case GameState.None:
                break;
            case GameState.Start:
                break;
            case GameState.StartDraft:
                //ServiceLocator.GetService<EnemiesManager>().ToggleSpawners(true);
                break;
            case GameState.Playing:
                break;
            case GameState.Safe:
                break;
            case GameState.Dead:
                break;
            default:
                Help.Debug(GetType(), "OnStateExit", "tried to execute an enum that has not been implemented");
                throw new ArgumentOutOfRangeException(nameof(exitingState), exitingState, null);
        }
    }
    
    private void OnStateEnter(GameState enteringState, GameState from)
    {
        switch (enteringState)
        {
            case GameState.None:
                break;
            case GameState.Start:
                ServiceLocator.GetService<EnemiesManager>().ToggleSpawners(false);
                ServiceLocator.GetService<UIManager>().FadeOutBlackout();
                break;
            case GameState.StartDraft:
                break;
            case GameState.Playing:
                ServiceLocator.GetService<EnemiesManager>().ToggleSpawners(true);
                break;
            case GameState.Safe:
                ServiceLocator.GetService<EnemiesManager>().ToggleSpawners(false);
                ServiceLocator.GetService<EnemiesManager>().KillAllEnemies(1);
                ServiceLocator.GetService<UIManager>().DraftCards(3);
                break;
            case GameState.Dead:
                ServiceLocator.GetService<EnemiesManager>().ToggleSpawners(false);
                ServiceLocator.GetService<EnemiesManager>().KillAllEnemies(1);
                ShowStatsScreen(5);
                break;
            default:
                Help.Debug(GetType(), "OnStateEnter", "tried to execute an enum that has not been implemented");
                throw new ArgumentOutOfRangeException(nameof(enteringState), enteringState, null);
        }
    }

    private void ShowStatsScreen(float delay)
    {
        StartCoroutine(ShowStatsDelayed());
        
        IEnumerator ShowStatsDelayed()
        {
            yield return new WaitForSeconds(delay);
            ServiceLocator.GetService<UIManager>().StatsScreen.ShowStats();
        }
    }
}

public enum GameState
{
    None,
    Start,
    StartDraft,
    Playing,
    Safe,
    Dead
}