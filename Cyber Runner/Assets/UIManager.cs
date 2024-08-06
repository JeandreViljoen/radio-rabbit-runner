using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class UIManager : MonoService
{
    public UpgradeCard SelectedCard = null;
    public List<UpgradeCard> Cards;
    public float DraftRevealInterval = 0.15f;
    private LazyService<PlayerController> _player;
    private LazyService<UpgradesManager> _upgradesManager;
    private LazyService<EXPManager> _expManager;
    private LazyService<LevelBlockManager> _levelBlockManager;
    private LazyService<GameStateManager> _stateManager;
    public Image SceneLoadBlackout;
    public StatsScreen StatsScreen;
    public bool IsDrafting = false;

    public Button StartButton;
    public Button QuitButton;

    void Start()
    {
        _stateManager.Value.OnStateChanged += DraftStarterWeapon;
        StartButton.onClick.AddListener(PlayButtonClicked);
        QuitButton.onClick.AddListener(ExitButtonClicked);
        
        foreach (var card in Cards)
        {
            card.OnSelected += UpdateSelectedCard;
        }

        Cards[^1].UIAnim.OnHideEnd += CheckDraft;
    }

    void CheckDraft()
    {
        DraftCards(0.5f);
    }

    private void PlayButtonClicked()
    {
        _stateManager.Value.ActiveState = GameState.StartDraft;

        StartButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
    }

    private void ExitButtonClicked()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            LoadMainScene();
        }
        
        
        if (!_player.Value.IsDead() && (Input.GetKeyDown(GlobalGameAssets.Instance.JumpKey) || Input.GetKeyDown(GlobalGameAssets.Instance.DashKey)))
        {
            if (SelectedCard != null)
            {
                SelectedCard.Submit();
                SelectedCard = null;
                HideAllCards();

                if (_stateManager.Value.ActiveState == GameState.StartDraft)
                {
                    _stateManager.Value.ActiveState = GameState.Playing;
                }
            }
        }
        else if (_player.Value.IsDead() && (Input.GetKeyDown(GlobalGameAssets.Instance.JumpKey) || Input.GetKeyDown(GlobalGameAssets.Instance.DashKey)))
        {
            if (!StatsScreen.LockInput)
            {
                LoadMainScene();
            }
            
        }
    }
    
    

    private Tween _blackoutFadeTween;
    private void LoadMainScene()
    {
        _blackoutFadeTween?.Kill();
        Sequence s = DOTween.Sequence();
        s.Append(SceneLoadBlackout.DOFade(1f, 1f));
        s.AppendCallback(() => {  SceneManager.LoadScene("MainScene"); });
        _blackoutFadeTween = s;

    }

    public void FadeOutBlackout()
    {
        _blackoutFadeTween?.Kill();
        Sequence s = DOTween.Sequence();
        s.AppendInterval(01f);
        s.Append(SceneLoadBlackout.DOFade(0f, 1f));
        _blackoutFadeTween = s;
    }

    private void UpdateSelectedCard(UpgradeCard card)
    {
        SelectedCard = card;
    }

    public void DraftStarterWeapon(GameState from, GameState to)
    {
        if (from != GameState.Start || to != GameState.StartDraft)
        {
            return;
        }

        StartCoroutine(DelayedDraft());
        
        IEnumerator DelayedDraft()
        {

            yield return new WaitForSeconds(1f);
            
            for (var index = 0; index < Cards.Count; index++)
            {
                var card = Cards[index];
                
                UpgradeType upgrade = _upgradesManager.Value.GetRandomUnownedWeaponUpgrade();
                if (upgrade != UpgradeType.None)
                {
                    card.Init(upgrade);
                }
                
                card.Show();
                yield return new WaitForSecondsRealtime(DraftRevealInterval);
            }
        }
    }

    public void DraftCards(float preDelay = 0f)
    {
        if (_expManager.Value.TryClaimLevel())
        {
            IsDrafting = true;
            StartCoroutine(DelayedDraft());
        }
        else
        {
            IsDrafting = false;
            _levelBlockManager.Value.SafeZoneFlag = false;
        }

        IEnumerator DelayedDraft()
        {
            yield return new WaitForSeconds(preDelay);
            
            for (var index = 0; index < Cards.Count; index++)
            {
                var card = Cards[index];

                float rng = UnityEngine.Random.Range(0f,1f);
                if (rng<= 0.5)
                {
                    UpgradeType upgrade = _upgradesManager.Value.GetRandomUnownedWeaponUpgrade();
                    if (upgrade != UpgradeType.None)
                    {
                        card.Init(upgrade);
                    }
                    else
                    {
                        PerkType perk = _upgradesManager.Value.GetRandomUnOwnedPerk();
                        card.Init(perk);
                    }
                    
                }
                else
                {
                    PerkType perk = _upgradesManager.Value.GetRandomUnOwnedPerk();
                    if (perk != PerkType.None)
                    {
                        card.Init(perk);
                    }
                    else
                    {
                        UpgradeType upgrade = _upgradesManager.Value.GetRandomUnownedWeaponUpgrade();
                        card.Init(upgrade);
                    }
                    
                }
                
                card.Show();
                yield return new WaitForSecondsRealtime(DraftRevealInterval);
            }
        }
    }

    public void HideAllCards()
    {
        foreach (var card in Cards)
        {
            card.Hide();
        }
    }
    

    private void OnDestroy()
    {
        foreach (var card in Cards)
        {
            card.OnSelected -= UpdateSelectedCard;
        }
    }

   
}
