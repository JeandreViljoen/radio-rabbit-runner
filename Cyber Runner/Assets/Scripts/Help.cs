using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Help 
{
   public static void Log(string className, string function, string message)
   {
      UnityEngine.Debug.Log($"[{className}.cs] : {function}() - {message}");
   }
   
   public static void Warning(string className, string function, string message)
   {
      UnityEngine.Debug.LogWarning($"[{className}.cs] : {function}() - {message}");
   }
   
   public static void Debug(string className, string function, string message)
   {
      UnityEngine.Debug.LogError($"[{className}.cs] : {function}() - {message}");
   }
   
   public static void Log(System.Type className, string function, string message)
   {
      UnityEngine.Debug.Log($"[{className.FullName}.cs] : {function}() - {message}");
   }
   
   public static void Warning(System.Type className, string function, string message)
   {
      UnityEngine.Debug.LogWarning($"[{className.FullName}.cs] : {function}() - {message}");
   }
   
   public static void Debug(System.Type className, string function, string message)
   {
      UnityEngine.Debug.LogError($"[{className.FullName}.cs] : {function}() - {message}");
   }
}
