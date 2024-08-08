using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHUDItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelField;
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;

    [SerializeField] private Sprite _disabledSprite;
    [SerializeField] private Sprite _enabledSprite;

    

    public bool IsInit { get; private set; } = false;

    void Start()
    {
        _background.sprite = _disabledSprite;
        _background.color = new Color(1f, 1f, 1f, 0.3f);
        _icon.gameObject.SetActive(false);
        _levelField.gameObject.SetActive(false);

    }

    public void InitHUDItem(Sprite icon)
    {
        _background.sprite = _enabledSprite;
        _background.color = new Color(1f, 1f, 1f, 1f);
        _icon.sprite = icon;
        _icon.gameObject.SetActive(true);
        _levelField.gameObject.SetActive(true);
        IsInit = true;
    }

    public void SetLevelField(int level)
    {
        _levelField.text = level.ToString();
    }
}
