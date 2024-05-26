using System;
using UnityEngine;



public class PrefabPoolObject : MonoBehaviour
{
    public PrefabPool Pool
    {
        get; set;
    }

    public int PrefabHash { get; set; }

    public void Return()
    {
        Pool.Return(gameObject);
    }

    public void OnDestroy()
    {
        Pool.NotifyDestroyed(gameObject);
    }
}

 
