using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponAttachmentPoint : MonoBehaviour
{
    [EnumToggleButtons] public AttachmentPosition Slot;
    private Weapon _weapon;
    [ReadOnly] public bool IsSlotted = false;
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void InitWeapon(Weapon w)
    {
        _weapon = w;
        _weapon.transform.position = transform.position;
        _weapon.transform.parent = transform.parent;
        IsSlotted = true;

        if (Slot == AttachmentPosition.RIGHT)
        {
            _weapon.SetRenderOrder(0);
        }
        else
        {
            _weapon.SetRenderOrder(3);
        }
    }
}

public enum AttachmentPosition
{
    LEFT,
    RIGHT,
    TOP,
    BOTTOM
}
