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
   
   public static float Map( float value, float leftMin, float leftMax, float rightMin, float rightMax , bool clamp = false)
   {
      
        float result = rightMin + ( value - leftMin ) * ( rightMax - rightMin ) / ( leftMax - leftMin );

        if (clamp)
        {
           if (result > rightMax)
           {
              return rightMax;
           }
           
           if(result < rightMin)
           {
              return rightMin;
           }
        }

        return result;
   }
   
   
   public static float Map01( float value, float min, float max )
   {
      return ( value - min ) * 1f / ( max - min );
   }
}
