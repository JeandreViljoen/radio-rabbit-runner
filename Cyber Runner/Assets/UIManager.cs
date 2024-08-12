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
    public MainMenu MainMenu;

    public UIAnimation SelectPrompt;
    
    public event Action OnUpgradeSubmitted;
    public event Action OnUpgradeCardsRevealed;

    void Start()
    {
        _stateManager.Value.OnStateChanged += DraftStarterWeapon;

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

    
    void Update()
    {
        if(Input.GetKeyDown(GlobalGameAssets.Instance.RestartKey))
        {
            LoadMainScene();
            AudioManager.PostEvent(AudioEvent.UI_SELECT);
        }
        
        if (!_player.Value.IsDead() && Input.GetKeyDown(GlobalGameAssets.Instance.ConfirmKey))
        {
            if (SelectedCard != null && SelectedCard.IsInit)
            {
                SelectedCard.Submit();
                SelectedCard = null;
                HideAllCards();
                OnUpgradeSubmitted?.Invoke();

                if (_stateManager.Value.ActiveState == GameState.StartDraft)
                {
                    _stateManager.Value.ActiveState = GameState.Playing;
                }
            }
        }
        else if (_player.Value.IsDead() && Input.GetKeyDown(GlobalGameAssets.Instance.ConfirmKey))
        {
            StatsScreen.HideStats();
            AudioManager.PostEvent(AudioEvent.UI_SELECT);
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
        s.AppendCallback(() =>
        {
            ThingsToDoBeforeSceneUnload();
            SceneManager.LoadScene("MainScene");
            
        });
        _blackoutFadeTween = s;

    }

    private void ThingsToDoBeforeSceneUnload()
    {
        AudioManager.PostEvent(AudioEvent.MX_STOP);
        AudioManager.PostEvent(AudioEvent.AMB_ROOFTOP_STOP);
        AudioManager.PostEvent(AudioEvent.AMB_FLYBY_STOP);
        DOTween.KillAll();
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
            List<UpgradeType> drafts = _upgradesManager.Value.GetStarterWeaponDraft(Cards.Count);
            
            for (var index = 0; index < Cards.Count; index++)
            {
                var card = Cards[index];
                
                if (drafts[index] != UpgradeType.None)
                {
                    card.Init(drafts[index]);
                }
                
                card.Show();
                yield return new WaitForSecondsRealtime(DraftRevealInterval);
            }
            
            SelectPrompt.Show();
            yield return new WaitForSeconds(0.3f);
            
            Cards[1].Select();
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
            int cardIndex = 0;

            GenericUpgrades draft = _upgradesManager.Value.GetFullDraft(Cards.Count);
            
            //Draft weapons
            for (var index = 0; index < draft.Weapons.Count; index++)
            {
                var wpn = draft.Weapons[index];

                Cards[cardIndex].Init(wpn);
                Cards[cardIndex].Show();
                cardIndex++;
                
                yield return new WaitForSecondsRealtime(DraftRevealInterval);
            }
            
            //Draft Perks
            for (var index = 0; index < draft.Perks.Count; index++)
            {
                var perk = draft.Perks[index];

                Cards[cardIndex].Init(perk);
                Cards[cardIndex].Show();
                cardIndex++;
                
                yield return new WaitForSecondsRealtime(DraftRevealInterval);
            }
            
            SelectPrompt.Show();
            OnUpgradeCardsRevealed?.Invoke();
            yield return new WaitForSeconds(0.3f);
            
            Cards[1].Select();
            
        }
    }

    public void HideAllCards()
    {
        foreach (var card in Cards)
        {
            card.Hide();
        }
        SelectPrompt.Hide();
    }
    

    private void OnDestroy()
    {
        foreach (var card in Cards)
        {
            card.OnSelected -= UpdateSelectedCard;
        }
    }

   
}
