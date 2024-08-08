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
    [SerializeField] private int _siblingIndex;
    
    [FoldoutGroup("References"), SerializeField] private Image _border;
    [FoldoutGroup("References"), SerializeField] private Image _background;
    [FoldoutGroup("References"), SerializeField] private Image _icon;
    [FoldoutGroup("References"), SerializeField] private PunchScaleUIElement _iconPunch;
    [FoldoutGroup("References"), SerializeField] private TextMeshProUGUI _typeField;
    [FoldoutGroup("References"), SerializeField]  private TextMeshProUGUI _displayNameField;
    [FoldoutGroup("References"), SerializeField] private TextMeshProUGUI _descriptionField;
    [FoldoutGroup("References"), SerializeField] private TextMeshProUGUI _levelField;
    [FormerlySerializedAs("_uiAnim")] [FoldoutGroup("References"), SerializeField] public UIAnimation UIAnim;

    [SerializeField] private List<Sprite> _borderSprites;
    [SerializeField] private Sprite _weaponBGSprite;
    [SerializeField] private Sprite _perkBGSprite;
    [SerializeField] private Image _levelBG;
    [SerializeField] private Image _star;

    private LazyService<UpgradesManager> _upgradesManager;
    private UpgradeData _weaponData;
    private PerkUpgradeInfo _perkData;
    private InfoPanelType _panelType;
    private Vector3 _startScale;
    private float _initRotation;

    public bool IsInit { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();
        _startScale = transform.localScale;
    }

    void Start()
    {

        UIAnim.OnShowEnd += () => { IsInit = true; };
        _border.gameObject.SetActive(false);
        _star.gameObject.SetActive(false);
        DimGraphics(true);
    }

    public void Init(UpgradeType upgrade)
    {
        _star.gameObject.SetActive(false);
        _panelType = InfoPanelType.WEAPON;
        _weaponData = _upgradesManager.Value.GetUpgradeData(upgrade);

        int lvl = _upgradesManager.Value.GetWeaponInstance(_weaponData.Type).Level;
        if(lvl == 0) _star.gameObject.SetActive(true);
        
        
        _levelField.text = "LEVEL " + (lvl + 1);
        _typeField.text = _weaponData.Type.ToString();
        _displayNameField.text = _weaponData.DisplayName;
        _icon.sprite = _weaponData.Icon;
        _background.sprite = _weaponBGSprite;
        _descriptionField.text = TokenizeDescriptionValue(_weaponData.Description, "{value}");
        _descriptionField.text = TokenizeDescriptionTarget(_descriptionField.text, "{targetType}");
        interactable = true;
        _initRotation = UnityEngine.Random.Range(-5f, 5f);
        SelectRotate(0.0001f, 0f);

    }
    
    public void Init(PerkType perkUpgrade)
    {
        _star.gameObject.SetActive(false);
        _panelType = InfoPanelType.PERK;
        _perkData = _upgradesManager.Value.GetPerkInfo(perkUpgrade);

        int lvl = _upgradesManager.Value.GetPerkInstance(_perkData.GroupType).Level;
        if(lvl == 0) _star.gameObject.SetActive(true);

        _levelField.text = "LEVEL " + (lvl + 1);
        _typeField.text = "PERK";
        _displayNameField.text = _perkData.DisplayName;
        _descriptionField.text = TokenizeDescriptionValue(_perkData.Description, "{value}");
        _icon.sprite = _perkData.Icon;
        _background.sprite = _perkBGSprite;
        interactable = true;
        _initRotation = UnityEngine.Random.Range(-5f, 5f);
        SelectRotate(0.0001f, 0f);
        
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

    private Coroutine HighlightAnim;

    public event Action<UpgradeCard> OnSelected; 
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        EnableSelectState();
        OnSelected?.Invoke(this);
    }

    private void DimGraphics(bool flag)
    {
        Color dimColor = Color.gray;
        
        if (!flag)
        {
            dimColor = Color.white;
        }
        
        _background.color = dimColor;
        _icon.color = dimColor;
        _levelBG.color = dimColor;
        _star.color = dimColor;

    }

    private void EnableSelectState()
    {
        DimGraphics(false);
        
        transform.SetAsLastSibling();
        _border.gameObject.SetActive(true);
        
        SelectRotate(0.05f, -4f);
        SelectScale(1.2f, 0.05f);
        SelectRotateIcon(0.2f,6);
        
        HighlightAnim = StartCoroutine(BorderAnim(0.2f));

        IEnumerator BorderAnim(float interval)
        {
            int counter = 0;
            int countLimit = _borderSprites.Count-1;
            while (true)
            {
                counter++;

                if (counter > countLimit) counter = 0;
                _border.sprite = _borderSprites[counter];
                
                yield return new WaitForSeconds(interval);
            }
        }
    }

    private void SelectRotate(float speed, float rotation)
    {
        _selectRotateTween?.Kill();
        _selectRotateTween = transform.DORotate(new Vector3(0f, 0f, _initRotation+rotation), speed).SetEase(Ease.InOutSine);
    }
    
    private void SelectRotateIcon(float speed, float rotationRange)
    {
        
        float rotation = UnityEngine.Random.Range(-rotationRange/2, rotationRange/2);
        
        _selectRotateIconTween?.Kill();
        _selectRotateIconTween = _icon.transform.DORotate(new Vector3(0f, 0f, rotation), speed).SetEase(Ease.InOutSine);

        _iconPunch.Punch(0.4f, 0.2f, 5);
    }

    private void SelectScale(float amount, float speed)
    {
        _selectScaleTween?.Kill();
        _selectScaleTween = transform.DOScale(_startScale*amount, speed).SetEase(Ease.InOutSine);
    }

    private Tween _selectRotateTween;
    private Tween _selectRotateIconTween;
    private Tween _selectScaleTween;

    private void DisableSelectState()
    {
        DimGraphics(true);
        SelectRotate(0.15f, 0f);
        SelectScale(1f, 0.15f);
        
        transform.SetSiblingIndex(_siblingIndex);
        StopCoroutine(HighlightAnim);
        _border.gameObject.SetActive(false);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        DisableSelectState();
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
        IsInit = false;
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
