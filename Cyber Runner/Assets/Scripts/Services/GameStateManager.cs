using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class GameStateManager : MonoService
{
    public event Action<GameState, GameState> OnStateChanged;
    private GameState _oldStateTransitionStorage;//only used for OnStateChange event
    private GameState _activeState= GameState.None;
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
                PreLoadLevels();
                ServiceLocator.GetService<UIManager>().MainMenu.ShowPrompts();
                ServiceLocator.GetService<EnemiesManager>().ToggleSpawners(false);
                ServiceLocator.GetService<UIManager>().FadeOutBlackout();
                ServiceLocator.GetService<GameplayManager>().CameraFollowTarget.UpdateCameraPosition(GameState.None, GameState.Start);
                ServiceLocator.GetService<PlayerController>().PlayerVisuals.UpdateTvPosition(GameState.None, GameState.Start);
                AudioManager.SetSwitch("Music", "Combat");
                AudioManager.PostEvent(AudioEvent.MX_START);
                AudioManager.PostEvent(AudioEvent.AMB_ROOFTOP_START);
                AudioManager.PostEvent(AudioEvent.AMB_FLYBY_START);
                break;
            case GameState.StartDraft:
                ServiceLocator.GetService<StatsTracker>().ResetAllStats();
                ServiceLocator.GetService<HUDManager>().SetHealthDisplay(ServiceLocator.GetService<PlayerController>().Health.CurrentHealth,5f);
                ServiceLocator.GetService<HUDManager>().ShowBars();
                break;
            case GameState.Playing:
                AudioManager.SetSwitch("Music", "Combat");
                ServiceLocator.GetService<EnemiesManager>().ToggleSpawners(true);
                break;
            case GameState.Safe:
                AudioManager.SetSwitch("Music", "Safe");
                ServiceLocator.GetService<EnemiesManager>().ToggleSpawners(false);
                ServiceLocator.GetService<EnemiesManager>().KillAllEnemies(1);
                ServiceLocator.GetService<UIManager>().DraftCards(3);
                ServiceLocator.GetService<PlayerController>().Health.AddHealth(20);
                ServiceLocator.GetService<PlayerController>().PlayerVisuals.HealthUpVFX.Play();
                
                break;
            case GameState.Dead:
                ServiceLocator.GetService<EnemiesManager>().ToggleSpawners(false);
                ServiceLocator.GetService<EnemiesManager>().KillAllEnemies(1);
                ServiceLocator.GetService<HUDManager>().ShowDeathBanner(0.5f);
                AudioManager.SetSwitch("Music", "Dead");
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
            ServiceLocator.GetService<UIManager>().StatsScreen.gameObject.SetActive(true);
            yield return new WaitForSeconds(delay-0.5f);
            ServiceLocator.GetService<UIManager>().StatsScreen.ShowDeadCharacterSprite();
            yield return new WaitForSeconds(0.5f);
            ServiceLocator.GetService<UIManager>().StatsScreen.ShowStats();
        }
    }

    //Used to avoid spiking on Level Block spawns
    private void PreLoadLevels()
    {
        //all unique blocks used in the level data. Copies are omitted.
        List<GameObject> uniqueLevels = ServiceLocator.GetService<LevelManager>().GetAllUniqueLevelBlocks();

        
        foreach (var levelblock in uniqueLevels)
        {
            LevelBlockManager levelBlockManager = ServiceLocator.GetService<LevelBlockManager>();
            int amountToPreload = levelBlockManager.BackBlockBuffer + levelBlockManager.FrontBlockBuffer + 1;

            List<GameObject> preloadedBlocks = new List<GameObject>();

            for (int i = 0; i < amountToPreload; i++)
            {
                GameObject block = ServiceLocator.GetService<PrefabPool>().Get(levelblock);
                preloadedBlocks.Add(block);
            }
            
            foreach (var block in preloadedBlocks)
            {
                ServiceLocator.GetService<PrefabPool>().Return(block);
            }
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