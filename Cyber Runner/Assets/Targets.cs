using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targets : MonoBehaviour
{
    public float FurthestTargetInnerRange = 5f;
    public float FurthestTargetOuterRange = 6f;
    
    public float HighestHealthTargetInnerRange = 5f;
    public float HighestHealthTargetOuterRange = 6f;
    
    public Enemy ClosestEnemy { get; private set; }
    public Enemy FurthestEnemy{ get; private set; }
    
    public Enemy HighestHealth { get; private set; }
    
    public Enemy LowestHealth{ get; private set; }
    
    
    public void SetClosestEnemy(Enemy e)
    {
        ClosestEnemy = e;
    }
    
    public void SetFurthestEnemy(Enemy e)
    {
        FurthestEnemy = e;
    }
    
    public void SetHighestHealthEnemy(Enemy e)
    {
        HighestHealth = e;
    }
    
    public void SetLowestHealthEnemy(Enemy e)
    {
        LowestHealth = e;
    }
    
    void OnDrawGizmos()
    {

        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            DrawGizmoCircle(transform.position, CircleNormal, FurthestTargetInnerRange,16);
            DrawGizmoCircle(transform.position, CircleNormal,FurthestTargetOuterRange,16);
            
            Gizmos.color = Color.green;
            DrawGizmoCircle(transform.position, CircleNormal,HighestHealthTargetOuterRange,16);
            
        }
        
    }

    public Vector3 CircleNormal = Vector3.forward;
    private void DrawGizmoCircle(Vector3 circleCenter, Vector3 circleNormal, float circleRadius, int segments = 32)
    {
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
