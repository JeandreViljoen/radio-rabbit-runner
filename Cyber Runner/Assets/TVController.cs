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
}
