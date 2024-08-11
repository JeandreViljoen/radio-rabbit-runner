using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private LazyService<GameStateManager> _stateManager;
    public float RevealInterval = 0.1f;
    public float WaitBeforePromptShow = 2f;
    
    public UIAnimation StartPrompt;
    public UIAnimation ExitPrompt;
    public UIAnimation RestartPrompt;
    public UIAnimation JumpPrompt;
    public UIAnimation DashPrompt;

    public UIAnimation Logo;

    public List<UIAnimation> Prompts = new ();
    void Start()
    {
        Prompts.Clear();
        Prompts.Add(ExitPrompt);
        Prompts.Add(RestartPrompt);
        Prompts.Add(JumpPrompt);
        Prompts.Add(DashPrompt);
        Prompts.Add(StartPrompt);

        StartPrompt.OnHideEnd += MainMenuPromptsHidden;

        Logo.transform.DOScale(transform.localScale * 1.1f, 3f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        Logo.Show();
    }

    private void MainMenuPromptsHidden()
    {
        gameObject.SetActive(false);
    }

    public void ShowPrompts()
    {
        
        Prompts.Clear();
        Prompts.Add(ExitPrompt);
        Prompts.Add(RestartPrompt);
        Prompts.Add(JumpPrompt);
        Prompts.Add(DashPrompt);
        Prompts.Add(StartPrompt);
       
        for (var index = 0; index < Prompts.Count; index++)
        {
            var p = Prompts[index];
            p.Show(index*RevealInterval + WaitBeforePromptShow);
        }
    }
    
    public void HidePrompts()
    {
        for (var index = 0; index < Prompts.Count; index++)
        {
            var p = Prompts[index];
            p.Hide(index*RevealInterval + 0.2f);
        }
        
        Logo.Hide(Prompts.Count * RevealInterval + 0.2f);
    }
    void Update()
    {
        if (Input.GetKeyDown(GlobalGameAssets.Instance.StartKey))
        {
            AudioManager.PostEvent(AudioEvent.UI_SELECT);
            StartPrompt.InteractionFeedback();
            PlayButtonClicked();
            HidePrompts();
        }
        
        if (Input.GetKeyDown(GlobalGameAssets.Instance.ExitKey))
        {
            AudioManager.PostEvent(AudioEvent.UI_SELECT);
            ExitButtonClicked();
        }

        if (Input.GetKeyDown(GlobalGameAssets.Instance.JumpKey))
        {
            JumpPrompt.InteractionFeedback();
        }
        
        if (Input.GetKeyDown(GlobalGameAssets.Instance.DashKey))
        {
            DashPrompt.InteractionFeedback();
        }
    }
    
    
    private void PlayButtonClicked()
    {
        _stateManager.Value.ActiveState = GameState.StartDraft;
    }

    private void ExitButtonClicked()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void OnDestroy()
    {
        StartPrompt.OnHideEnd -= MainMenuPromptsHidden;
    }
    
}
