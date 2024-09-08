using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Custom Assets/Audio/Audio Library")]
public class AudioLibrary : SerializedScriptableObject
{
    
    [SerializeField] public Dictionary<string, string> Events;

    public string GetEvent(AudioEvent e)
    {
        return Events[e.ToString()];
    }
    

#if UNITY_EDITOR
    [Button(ButtonSizes.Gigantic), GUIColor("blue")]
    public void RegenerateEnums()
    {
        string path = "Assets/Audio/AudioEvent.cs";

        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.WriteLine("public enum AudioEvent");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\t" + "None,");

            foreach (var e in Events)
            {
                streamWriter.WriteLine("\t" + e.Key + ",");
            }
            
            streamWriter.WriteLine("}");
        }

        AssetDatabase.Refresh(); 
    }
#endif

}
