using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class HUDManager : MonoService
{
    [SerializeField] private TextMeshProUGUI _speedField;
    [SerializeField] private TextMeshProUGUI _healthTextField;
    


    void Start()
    {
       
    }

    void Update()
    {
        
    }

    public void SetSpeedValue(float speed)
    {
        _speedField.text = "Speed: " + speed;
    }

    public void SetHealthDisplay(int amount)
    {
        _healthTextField.text = "HEALTH: " + amount;
    }
}
