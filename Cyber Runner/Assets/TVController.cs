using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PowerTools;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

public class TVController : MonoService
{
    public SpriteRenderer TVBaseRenderer;
    public SpriteAnim TVBaseAnim;
    public SpriteRenderer TVFaceRenderer;
    public SpriteAnim TVFaceAnim;
    public List<WeaponAttachmentPoint> AttachmentPoints;
    public LazyService<PlayerController> _player;

    
    [FoldoutGroup("Face Animations")] public AnimationClip DeadFace;
    [FoldoutGroup("Face Animations")] public AnimationClip NormalFace;
    [FoldoutGroup("Face Animations")] public AnimationClip HappyFace;
    [FoldoutGroup("Face Animations")] public AnimationClip HurtFace;
    [FoldoutGroup("Face Animations")] public AnimationClip AngryFace;

    [FoldoutGroup("Face Animations")] public List<AnimationClip> NumberFaces;

    private LazyService<UIManager> _uiManager;

    private bool _lockTVFace = false;

    void Start()
    {
       DoRotate();
       _player.Value.PlayerVisuals.FlipLeftFlag += FlipTV;
       ServiceLocator.GetService<GameStateManager>().OnStateChanged += ChangeFaceOnStateChange;
       ServiceLocator.GetService<PlayerController>().Health.OnHealthLost += DoHurtFace;
       _uiManager.Value.OnUpgradeSubmitted += DoHappyFace;
       _uiManager.Value.OnUpgradeCardsRevealed += SetNumberFaceAuto;
       _player.Value.OnDashEnter += DoAngryFace;
    }

    private void FlipTV(bool flipflag)
    {
        if (flipflag)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void DoRotate()
    {
        transform.DOLocalRotate(new Vector3(0,0,3), 1f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
    }
    
    void Update()
    {
        
    }

    private void SetNumberFaceAuto()
    {
        if (_lockTVFace) return;
            
            
        SetNumberFace(ServiceLocator.GetService<EXPManager>().UnclaimedLevels+1);
    }
    
    public void SetNumberFace(int number)
    {
        if (number < 1)
        {
            TVFaceAnim.Play(NormalFace);
            return;
        }
        else if (number > NumberFaces.Count)
        {
            TVFaceAnim.Play(NumberFaces[^1]);
            return;
        }
        
        TVFaceAnim.Play(NumberFaces[number-1]);
    }

    public void DoHappyFace()
    {
        if (_lockTVFace) return;
        TVFaceAnim.Play(HappyFace);
        AudioManager.PostEvent(AudioEvent.PL_TV_VOCALISE, gameObject);
    }
    
    public void DoAngryFace()
    {
        if (_lockTVFace) return;
        TVFaceAnim.Play(AngryFace);

        StartCoroutine(AngryFaceDelay());
        
        IEnumerator AngryFaceDelay()
        {
            yield return new WaitUntil(IsTVFaceStopped);
            TVFaceAnim.Play(NormalFace);
        }
    }

    public void DoHurtFace()
    {
        if (_lockTVFace) return;
        TVFaceAnim.Play(HurtFace);

        StartCoroutine(HurtFaceDelay());
        
        IEnumerator HurtFaceDelay()
        {
            yield return new WaitUntil(IsTVFaceStopped);
            TVFaceAnim.Play(NormalFace);
        }
    }

    private bool IsTVFaceStopped()
    {
        return !TVFaceAnim.IsPlaying();
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

    public void ChangeFaceOnStateChange(GameState from, GameState to)
    {
        switch (to)
        {
            case GameState.None:
                break;
            case GameState.Start:
                TVFaceAnim.Play(NormalFace);
                break;
            case GameState.StartDraft:
                TVFaceAnim.Play(NormalFace);
                break;
            case GameState.Playing:
                TVFaceAnim.Play(NormalFace);
                break;
            case GameState.Safe:
                TVFaceAnim.Play(NormalFace);
                break;
            case GameState.Dead:
                TVFaceAnim.Play(DeadFace);
                _lockTVFace = true;
                break;
            default:
                break;
        }
    }
    
    
}
