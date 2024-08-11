using System.Collections;
using System.Collections.Generic;
using System.IO;
using Services;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class AudioManager : MonoService
{
	public AudioLibrary AudioLibrary;
	public static AudioManager Instance;
	void Awake()
	{
		if (Instance == null)
		{
			//DontDestroyOnLoad only works for root GameObjects or components on root GameObjects.
			//DontDestroyOnLoad(gameObject);
			Instance = this;
		}
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
   
    
    public static void PostEvent(string eventName, GameObject audioObject = null)
    {

	    if (audioObject != null && !IsRegistered(audioObject))
	    {
		    RegisterGameObj(audioObject);
	    }
	    
    	if (Instance == null)
    	{
            Help.Debug("AudioManager.cs", "PostEvent", "Tried to post an event but AudioManager Instance is null");
    		return;
    	}

        AkSoundEngine.PostEvent(eventName, audioObject == null ? Instance.gameObject : audioObject);
    }
    
    public static void PostEvent(AudioEvent eventEnum, GameObject audioObject = null)
    {
	    if (eventEnum == AudioEvent.None)
	    {
		    Help.Debug("AudioManager.cs", "PostEvent", "Tried to post a NONE audio event. This ideally shouldnt happen. returning early.");
		    return;
	    }

	    if (audioObject != null && !IsRegistered(audioObject))
	    {
		    RegisterGameObj(audioObject);
	    }
	    
	    if (Instance == null)
	    {
		    Help.Debug("AudioManager.cs", "PostEvent", "Tried to post an event but AudioManager Instance is null");
		    return;
	    }

	    AudioLibrary library =  ServiceLocator.GetService<AudioManager>().AudioLibrary;
	    AkSoundEngine.PostEvent(library.GetEvent(eventEnum), audioObject == null ? Instance.gameObject : audioObject);
    }
    
    public static void SetRTPCValue(string rtpcName, float rtpcValue, GameObject audioObject = null)
    {
	    if (Instance == null)
	    {
		    return;
	    }
	    AkSoundEngine.SetRTPCValue(rtpcName, rtpcValue, audioObject == null ? Instance.gameObject : audioObject);
    }

    public static void RegisterGameObj(GameObject audioObject)
    {
	    AkSoundEngine.RegisterGameObj(audioObject);
    }

    public static bool IsRegistered(GameObject audioObject)
    {
	    return AkSoundEngine.IsGameObjectRegistered(audioObject);
    }
    
    
    public static void SetObjectPosition(GameObject audioObject, Transform transform)
    {
	    AkSoundEngine.SetObjectPosition(audioObject, transform);
    }
    
    public static void SetSwitch(string switchGroup, string switchName, GameObject audioObject = null)
    {
	    if (Instance == null)
	    {
		    return;
	    }
	    AkSoundEngine.SetSwitch(switchGroup, switchName, audioObject == null ? Instance.gameObject : audioObject);
    }
    
    
}
