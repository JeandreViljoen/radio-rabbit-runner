using System;
using UnityEngine;

public static class Help 
{
   public static Color SlightHighlightColor = new Color( 0.8f, 0.8f, 1f);

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

   public static Color GetColorBasedOnTargetType(TargetingType type)
   {
      Color c = Color.white;
      
      switch (type)
      {
         case TargetingType.None:
            break;
         case TargetingType.Closest:
            c = Color.red;
            break;
         case TargetingType.Furthest:
            c = new Color(0.4f, 0.4f, 1f);
            break;
         case TargetingType.HighestHealth:
            c = new Color(1, 0.5f, 0, 1);
            break;
         case TargetingType.LowestHealth:
            c = Color.green;
            break;
         case TargetingType.Random:
            c = new Color(1, 0, 1, 1);
            break;
         case TargetingType.Direction:
            c = new Color(0, 1, 1, 1);
            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }

      return c;
   }
   
   public static void DrawGizmoCircle(Vector3 circleCenter, float circleRadius, int segments = 16)
   {
      Vector3 circleNormal = new Vector3(0f, 0.001f, 1f);
        
      Vector3 radiusVector = Mathf.Abs(Vector3.Dot(circleNormal, Vector3.right)) - 1f <= Mathf.Epsilon 
         ? Vector3.Cross(circleNormal, Vector3.forward).normalized 
         : Vector3.Cross(circleNormal, Vector3.right).normalized;
      radiusVector *= circleRadius;
      float angleBetweenSegments = 360f / segments;
      Vector3 previousCircumferencePoint = circleCenter + radiusVector;
      for (int i = 0; i < segments; ++i)
      {
         radiusVector = Quaternion.AngleAxis(angleBetweenSegments, circleNormal) * radiusVector;
         Vector3 newCircumferencePoint = circleCenter + radiusVector;
         Gizmos.DrawLine(previousCircumferencePoint, newCircumferencePoint);
         previousCircumferencePoint = newCircumferencePoint;
      }
   }

}
