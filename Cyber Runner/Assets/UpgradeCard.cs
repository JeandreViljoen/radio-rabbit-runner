using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [FoldoutGroup("References"), SerializeField] private UIAnimation _uiAnim;

    private LazyService<UpgradesManager> _upgradesManager;
    private UpgradeData _data;
    void Start()
    {
        _uiAnim.OnHideEnd += CheckIfMoreDrafts;
    }

    private void CheckIfMoreDrafts()
    {
        
    }

    public void Init(UpgradeType upgrade)
    {
        _data = _upgradesManager.Value.GetUpgradeData(upgrade);


        _levelField.text = "LEVEL " + (_upgradesManager.Value.GetWeaponInstance(_data.Type).Level + 1);
        _typeField.text = _data.Type.ToString();
        _displayNameField.text = _data.DisplayName;
        _descriptionField.text = TokenizeDescriptionValue(_data.Description, "{value}");
        _descriptionField.text = TokenizeDescriptionTarget(_descriptionField.text, "{targetType}");
        interactable = true;
    }

    public void Show()
    {
        _uiAnim.Show();
    }
    
    public void Hide()
    {
        _uiAnim.Hide();
    }

    private string TokenizeDescriptionValue(string input, string delim)
    {
        string[] parts = input.Split(delim);
        
        if (parts.Length <= 1)
        {
            return input;
        }
        string output = parts[0] + _data.Value + parts[1];

       
        return output;
    }
    
    private string TokenizeDescriptionTarget(string input, string delim)
    {
        string[] parts = input.Split(delim);
        
        if (parts.Length <= 1)
        {
            return input;
        }
        string output = parts[0] + _upgradesManager.Value.GetWeaponInstance(_data.Type).TargetType + parts[1];

       
        return output;
    }
    
    void Update()
    { 
        if(Input.GetKeyDown(KeyCode.U))
        {
            //_upgradesManager.Value.GetWeaponInstance(WeaponType.Railgun).LevelUp();
            //_upgradesManager.Value.GetWeaponInstance(WeaponType.Minigun).LevelUp();
            //_upgradesManager.Value.GetWeaponInstance(WeaponType.RocketLauncher).LevelUp();
            //Init(_upgradesManager.Value.GetNextUpgradeForWeapon(WeaponType.RocketLauncher));
        }
    }

    public event Action<UpgradeCard> OnSelected; 
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        _uiAnim.Highlight();
        OnSelected?.Invoke(this);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        _uiAnim.Show();
    }

    // public void OnSubmit(BaseEventData eventData)
    // {
    //     //OnSubmitBehavior();
    // }

    public void Submit()
    {
        _upgradesManager.Value.GetWeaponInstance(_data.Type).LevelUp();
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
