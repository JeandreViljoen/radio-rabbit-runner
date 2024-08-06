using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UpgradeCard : Selectable
{
    [FoldoutGroup("References"), SerializeField] private Image _border;
    [FoldoutGroup("References"), SerializeField] private Image _background;
    [FoldoutGroup("References"), SerializeField] private Image _icon;
    [FoldoutGroup("References"), SerializeField] private TextMeshProUGUI _typeField;
    [FoldoutGroup("References"), SerializeField]  private TextMeshProUGUI _displayNameField;
    [FoldoutGroup("References"), SerializeField] private TextMeshProUGUI _descriptionField;
    [FoldoutGroup("References"), SerializeField] private TextMeshProUGUI _levelField;
    [FormerlySerializedAs("_uiAnim")] [FoldoutGroup("References"), SerializeField] public UIAnimation UIAnim;

    private LazyService<UpgradesManager> _upgradesManager;
    private UpgradeData _weaponData;
    private PerkUpgradeInfo _perkData;
    private InfoPanelType _panelType;
    void Start()
    {
        UIAnim.OnHideEnd += CheckIfMoreDrafts;
    }

    private void CheckIfMoreDrafts()
    {
        
    }

    public void Init(UpgradeType upgrade)
    {
        _panelType = InfoPanelType.WEAPON;
        _weaponData = _upgradesManager.Value.GetUpgradeData(upgrade);


        _levelField.text = "LEVEL " + (_upgradesManager.Value.GetWeaponInstance(_weaponData.Type).Level + 1);
        _typeField.text = _weaponData.Type.ToString();
        _displayNameField.text = _weaponData.DisplayName;
        _descriptionField.text = TokenizeDescriptionValue(_weaponData.Description, "{value}");
        _descriptionField.text = TokenizeDescriptionTarget(_descriptionField.text, "{targetType}");
        interactable = true;
    }
    
    public void Init(PerkType perkUpgrade)
    {
        _panelType = InfoPanelType.PERK;
        _perkData = _upgradesManager.Value.GetPerkInfo(perkUpgrade);


        _levelField.text = "LEVEL " + (_upgradesManager.Value.GetPerkInstance(_perkData.GroupType).Level + 1);
        _typeField.text = "PERK";
        _displayNameField.text = _perkData.DisplayName;
        _descriptionField.text = TokenizeDescriptionValue(_perkData.Description, "{value}");
        interactable = true;
    }

    public void Show()
    {
        UIAnim.Show();
    }
    
    public void Hide()
    {
        UIAnim.Hide();
    }

    private string TokenizeDescriptionValue(string input, string delim)
    {
        string[] parts = input.Split(delim);
        
        if (parts.Length <= 1)
        {
            return input;
        }
        
        string output = "";

        switch (_panelType)
        {
            case InfoPanelType.WEAPON:
                output = parts[0] + _weaponData.Value + parts[1];
                break;
            case InfoPanelType.PERK:
                output = parts[0] + _perkData.Value + parts[1];
                break;
        }
        
        return output;
    }

    private string TokenizeDescriptionTarget(string input, string delim)
    {
        string[] parts = input.Split(delim);
        
        if (parts.Length <= 1)
        {
            return input;
        }
        string output = parts[0] + _upgradesManager.Value.GetWeaponInstance(_weaponData.Type).TargetType + parts[1];

       
        return output;
    }
    
    void Update()
    {
        
    }

    public event Action<UpgradeCard> OnSelected; 
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        UIAnim.Highlight();
        OnSelected?.Invoke(this);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        UIAnim.Show();
    }

    // public void OnSubmit(BaseEventData eventData)
    // {
    //     //OnSubmitBehavior();
    // }

    public void Submit()
    {
        switch (_panelType)
        {
            case InfoPanelType.WEAPON:
                _upgradesManager.Value.GetWeaponInstance(_weaponData.Type).LevelUp();
                break;
            case InfoPanelType.PERK:
                _upgradesManager.Value.GetPerkInstance(_perkData.GroupType).LevelUp();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ServiceLocator.GetService<StatsTracker>().UpgradesDrafted++;
        interactable = false;
    }

    // private void OnSubmitBehavior()
    // {
    //     _upgradesManager.Value.GetWeaponInstance(_data.Type).LevelUp();
    // }
    
    
    
    public void SetPanelState(InfoPanelState state)
    {
        switch (state)
        {
            case InfoPanelState.Hidden:
                break;
            case InfoPanelState.Shown:
                break;
            case InfoPanelState.Highlighted:
                break;
            case InfoPanelState.Submitted:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
}

public enum InfoPanelState
{
    Hidden,
    Shown,
    Highlighted,
    Submitted
}

public enum InfoPanelType
{
    WEAPON,
    PERK
}
