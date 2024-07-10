using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeCard : Selectable, ISubmitHandler
{
    [SerializeField] private Image _border;
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _typeField;
    [SerializeField]  private TextMeshProUGUI _displayNameField;
    [SerializeField] private TextMeshProUGUI _descriptionField;

    private LazyService<UpgradesManager> _upgradesManager;
    private UpgradeData _data;
    private WeaponType _type;
    void Start()
    {
        
    }

    public void Init(UpgradeType upgrade)
    {
        _type = _upgradesManager.Value.GetWeaponTypeFromUpgrade(upgrade);
        _data = _upgradesManager.Value.GetUpgradeData(upgrade);
        
        
        _typeField.text = _type.ToString();
        _displayNameField.text = _data.DisplayName;
        _descriptionField.text = TokenizeDescription(_data.Description, "{value}");
    }

    private string TokenizeDescription(string input, string delim)
    {
        string[] parts = input.Split(delim);
        
        if (parts.Length <= 1)
        {
            return input;
        }
        string output = parts[0] + _data.Value + parts[1];

       
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
    }

    public void OnSubmit(BaseEventData eventData)
    {
        //OnSubmitBehavior();
    }

    private void OnMouseDown()
    {
        OnSubmitBehavior();
    }

    private void OnSubmitBehavior()
    {
        _upgradesManager.Value.GetWeaponInstance(_type).LevelUp();
    }

    public void Reveal()
    {
        
    }

    public void Hide()
    {
        
    }
    
    
}
