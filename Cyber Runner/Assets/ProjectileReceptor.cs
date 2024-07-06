using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileReceptor : MonoBehaviour
{
    public event Action<ProjectileBase> OnHit;

    // private void OnTriggerEnter2D(Collider2D col)
    // {
    //     if (col.CompareTag("Projectile"))
    //     {
    //         ProjectileBase projectile = col.gameObject.GetComponent<ProjectileBase>();
    //         
    //         if (projectile == null)
    //             Help.Debug(GetType(), "OnTriggerEnter2D", "A collision with a projectile has been detected but could not find ProjectileBase component, this is very bad. fix ASAP");
    //         else
    //             OnHit?.Invoke(projectile);
    //     }
    // }
}
