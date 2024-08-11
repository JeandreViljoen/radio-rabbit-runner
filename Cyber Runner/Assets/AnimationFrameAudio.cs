using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFrameAudio : MonoBehaviour
{
    void Anim_Footstep()
    {
        AudioManager.PostEvent(AudioEvent.PL_FOOTSTEP);
    }
}
