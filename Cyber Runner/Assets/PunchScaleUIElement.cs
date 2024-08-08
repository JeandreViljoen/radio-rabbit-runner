using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PunchScaleUIElement : MonoBehaviour
{
    private Tween _punchTween;
    
    public void Punch(float amount, float duration, int vib)
    {
        if (_punchTween == null || !_punchTween.IsPlaying())
        {
            _punchTween = transform.DOPunchScale(Vector3.one * amount, duration, vib);
        }
    }
}
