using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class VFXManager : MonoService
{
    private LazyService<PrefabPool> _prefabPool;
    [SerializeField] private GameObject _dashDust;
    [SerializeField] private GameObject _dashVortex;
    [SerializeField] private GameObject _landingDust;
    [SerializeField] private GameObject _OnHitRailgun;
    [SerializeField] private GameObject _weaponSpawn;
    [SerializeField] private GameObject _ShieldHit;
    [SerializeField] private GameObject _lightningVFX;

    public void DashDust(Vector3 position)
    {
        GameObject effect = _prefabPool.Value.Get(_dashDust);
        effect.transform.position = position;
    }
    
    public void DashVortex(Vector3 position)
    {
        GameObject effect = _prefabPool.Value.Get(_dashVortex);
        effect.transform.position = position;
    }
    
    public void LandingDust(Vector3 position)
    {
        GameObject effect = _prefabPool.Value.Get(_landingDust);
        effect.transform.position = position;
    }
    
    public void WeaponSpawn(Transform t)
    {
        ParticleVFX effect = _prefabPool.Value.Get(_weaponSpawn).GetComponent<ParticleVFX>();
        effect.transform.position = t.position;
        effect.SetFollowTarget(t);
    }
    
    public void OnHitRailgun(Transform t, Vector3 direction)
    {
        ParticleVFX effect = _prefabPool.Value.Get(_OnHitRailgun).GetComponent<ParticleVFX>();
        effect.transform.position = t.position;
        effect.SetFollowTarget(t);
        effect.Play(direction);
    }
    
    public void OnHitShield(Vector3 position, Transform parent, Vector3 direction)
    {
        ParticleVFX effect = _prefabPool.Value.Get(_ShieldHit).GetComponent<ParticleVFX>();
        effect.transform.position = position;
        effect.transform.parent = parent;
        effect.Play(direction);
    }
    
    
}
