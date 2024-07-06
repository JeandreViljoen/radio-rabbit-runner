using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Targets : MonoBehaviour
{
    [SerializeField, GUIColor("green")]private bool _showGizmos = false;
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
        if (!_showGizmos)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Help.DrawGizmoCircle(transform.position, FurthestTargetInnerRange);
            Help.DrawGizmoCircle(transform.position,FurthestTargetOuterRange);
            
            Gizmos.color = Color.green;
            Help.DrawGizmoCircle(transform.position,HighestHealthTargetOuterRange);
            
        }
        
    }

   
    
    
}
