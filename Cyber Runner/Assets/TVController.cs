using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PowerTools;
using Services;
using UnityEngine;

public class TVController : MonoService
{
    public SpriteRenderer TVBaseRenderer;
    public SpriteAnim TVBaseAnim;
    public SpriteRenderer TVFaceRenderer;
    public SpriteAnim TVFaceAnim;
    public List<WeaponAttachmentPoint> AttachmentPoints;

    void Start()
    {
       DoRotate();
    }

    void DoRotate()
    {
        transform.DOLocalRotate(new Vector3(0,0,3), 1f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
    }
    
    void Update()
    {
        
    }

    public bool RegisterWeapon(Weapon weapon)
    {
        foreach (var point in AttachmentPoints)
        {
            if (!point.IsSlotted)
            {
                ServiceLocator.GetService<VFXManager>().WeaponSpawn(point.transform);
                point.InitWeapon(weapon);
                return true;
            }
        }
        
        Help.Debug(GetType(), "RegisterWeapon", "Tried to register a weapon but all slots have been filled");
        return false;
    }
    
    
}
