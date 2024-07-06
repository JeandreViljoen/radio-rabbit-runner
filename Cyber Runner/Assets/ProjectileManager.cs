using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class ProjectileManager : MonoService
{
    [SerializeField, GUIColor("grey")]private bool _showGizmos = false;
    [OdinSerialize, ShowInInspector, GUIColor("red")] public float CullDistance { get; private set; } 

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
    
    void OnDrawGizmos()
    {

        if (!_showGizmos)
        {
            return;
        }

        //if (Application.isPlaying)
        //{
            Gizmos.color = Color.white;
            Help.DrawGizmoCircle(transform.position, CullDistance);
        //}
        
    }

}
