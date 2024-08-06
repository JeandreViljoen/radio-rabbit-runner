using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class VFXManager : MonoService
{
    private LazyService<PrefabPool> _prefabPool;
    [SerializeField] private GameObject _dashDust;
    [SerializeField] private GameObject _landingDust;

    public void DashDust(Vector3 position)
    {
        GameObject effect = _prefabPool.Value.Get(_dashDust);
        effect.transform.position = position;
    }
    
    public void LandingDust(Vector3 position)
    {
        GameObject effect = _prefabPool.Value.Get(_landingDust);
        effect.transform.position = position;
    }
}
