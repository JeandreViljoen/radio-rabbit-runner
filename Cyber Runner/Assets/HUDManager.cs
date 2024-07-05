using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class HUDManager : MonoService
{
    [SerializeField] private TextMeshProUGUI _speedField;
    


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
}
