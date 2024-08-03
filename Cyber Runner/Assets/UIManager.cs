using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Random = System.Random;

public class UIManager : MonoBehaviour
{
    public UpgradeCard SelectedCard = null;
    public List<UpgradeCard> Cards;
    public float DraftRevealInterval = 0.15f;
    private LazyService<PlayerController> _player;
    private LazyService<UpgradesManager> _upgradesManager;

    public List<WeaponType> TestList;

    void Start()
    {
        foreach (var card in Cards)
        {
            card.OnSelected += UpdateSelectedCard;
        }
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

    public void DraftCards()
    {
        StartCoroutine(DelayedDraft());
        
        IEnumerator DelayedDraft()
        {
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
