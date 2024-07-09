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
    private Image _border;
    private Image _background;
    private Image _icon;
    private TextMeshProUGUI _typeField;
    private TextMeshProUGUI _displayNameField;
    private TextMeshProUGUI _descriptionField;

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
        _descriptionField.text = _data.Description;
    }
    
    void Update()
    {
        
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        //_upgradesManager.Value.RegisterUpgrade();
        throw new NotImplementedException();
    }

    public void Reveal()
    {
        
    }

    public void Hide()
    {
        
    }
    
    
}
