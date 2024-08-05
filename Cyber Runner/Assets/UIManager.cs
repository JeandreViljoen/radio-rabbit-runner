using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
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
    public bool IsDrafting = false;

    public List<WeaponType> TestList;

    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            DraftCards();
        }
        
        if (!_player.Value.IsDead() && (Input.GetKeyDown(GlobalGameAssets.Instance.JumpKey) || Input.GetKeyDown(GlobalGameAssets.Instance.DashKey)))
        {
            if (SelectedCard != null)
            {
                SelectedCard.Submit();
                SelectedCard = null;
                HideAllCards();
            }
        }
    }

    private void UpdateSelectedCard(UpgradeCard card)
    {
        SelectedCard = card;
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
