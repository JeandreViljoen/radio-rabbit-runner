using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targets : MonoBehaviour
{
    public float FurthestTargetInnerRange = 5f;
    public float FurthestTargetOuterRange = 6f;
    
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
}
