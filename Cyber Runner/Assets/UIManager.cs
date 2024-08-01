using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

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
                card.Init(_upgradesManager.Value.GetNextUpgradeForWeapon(TestList[index]));
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
