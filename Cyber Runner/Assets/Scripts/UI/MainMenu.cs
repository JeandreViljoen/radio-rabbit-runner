using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
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
    public UIAnimation TutorialPrompt;
    public UIAnimation SurveyPrompt;

    public UIAnimation Logo;

    private bool _isStarted = false;

    public List<UIAnimation> Prompts = new ();
    void Start()
    {
        Prompts.Clear();
        Prompts.Add(ExitPrompt);
        Prompts.Add(RestartPrompt);
        Prompts.Add(TutorialPrompt);
        Prompts.Add(JumpPrompt);
        Prompts.Add(DashPrompt);
        Prompts.Add(StartPrompt);
        Prompts.Add(SurveyPrompt);

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
        Prompts.Add(TutorialPrompt);
        Prompts.Add(JumpPrompt);
        Prompts.Add(DashPrompt);
        Prompts.Add(StartPrompt);
        Prompts.Add(SurveyPrompt);
       
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
            p.Hide( 0.2f);
        }
        
        Logo.Hide(  0.2f);
    }
    void Update()
    {
        if (Input.GetKeyDown(GlobalGameAssets.Instance.StartKey))
        {
            if (_isStarted)
            {
                return;
            }
            
            AudioManager.PostEvent(AudioEvent.UI_SELECT);
            StartPrompt.InteractionFeedback();
            PlayButtonClicked();
            HidePrompts();
            _isStarted = true;
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
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            AudioManager.PostEvent(AudioEvent.UI_SELECT);
            Application.OpenURL("https://forms.gle/EkSRna5Uyx9MwDpZ7");
        }
    }
    
    
    private void PlayButtonClicked()
    {
        if (_stateManager.Value.ActiveState != GameState.StartDraft)
        {
            _stateManager.Value.ActiveState = GameState.StartDraft;
        }
       
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

    [SerializeField] private TextMeshProUGUI _shieldedText;
    public void HighlightShieldedText()
    {
        _shieldedText.color = new Color(0.6f, 0.6f, 1f);
    }
    
    public void UnhighlightShieldedText()
    {
        _shieldedText.color = new Color(1f, 1f, 1f);
    }
    
}
