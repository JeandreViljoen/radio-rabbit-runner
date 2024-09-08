using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileReceptor : MonoBehaviour
{
    public event Action<ProjectileBase> OnHit;
}
