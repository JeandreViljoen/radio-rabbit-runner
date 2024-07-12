using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeCard : Selectable, ISubmitHandler
{
    [FoldoutGroup("References"), SerializeField] private Image _border;
    [FoldoutGroup("References"), SerializeField] private Image _background;
    [FoldoutGroup("References"), SerializeField] private Image _icon;
    [FoldoutGroup("References"), SerializeField] private TextMeshProUGUI _typeField;
    [FoldoutGroup("References"), SerializeField]  private TextMeshProUGUI _displayNameField;
    [FoldoutGroup("References"), SerializeField] private TextMeshProUGUI _descriptionField;
    [FoldoutGroup("References"), SerializeField] private TextMeshProUGUI _levelField;

    private LazyService<UpgradesManager> _upgradesManager;
    private UpgradeData _data;
    void Start()
    {
        
    }

    public void Init(UpgradeType upgrade)
    {
        _data = _upgradesManager.Value.GetUpgradeData(upgrade);


        _levelField.text = "LEVEL " + (_upgradesManager.Value.GetWeaponInstance(_data.Type).Level + 1);
        _typeField.text = _data.Type.ToString();
        _displayNameField.text = _data.DisplayName;
        _descriptionField.text = TokenizeDescriptionValue(_data.Description, "{value}");
        _descriptionField.text = TokenizeDescriptionTarget(_data.Description, "{targetType}");
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
            Init(_upgradesManager.Value.GetNextUpgradeForWeapon(WeaponType.Minigun));
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        OnSubmitBehavior();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        //OnSubmitBehavior();
    }

    private void OnSubmitBehavior()
    {
        _upgradesManager.Value.GetWeaponInstance(_data.Type).LevelUp();
    }

    public void Reveal()
    {
        
    }

    public void Hide()
    {
        
    }
    
    
}
