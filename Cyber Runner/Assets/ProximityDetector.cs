using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class ProximityDetector : MonoBehaviour
{
    public int DetectionAmount = 1;
    [SerializeField] private Collider2D _detectionTrigger;
    private List<Enemy> DetectedTargets = new List<Enemy>();
    private GameObject _originalEntity;
    private LazyService<UpgradesManager> _upgradesManager;

    void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        DetectionAmount = 1;
        DetectedTargets.Clear();
    }

    public void EnableProximityDetector(GameObject originalEntity)
    {
        //DetectedTargets.Clear();
        _originalEntity = originalEntity;
        gameObject.SetActive(true);
    }

    public Enemy GetProximityTarget()
    {
        Enemy closest = null;
        float closestDistance = 0f;
        
        foreach (var target in DetectedTargets)
        {
            if (closest = null)
            {
                closest = target;
                continue;
            }

            float dist = Vector3.Distance(target.transform.position, closest.transform.position);

            if (dist < closestDistance)
            {
                closest = target;
                closestDistance = dist;
            }
        }

        return closest;
    }
    
    public List<Enemy> GetProximityTargets()
    {
        if (DetectedTargets.Count <= DetectionAmount)
        {
            return DetectedTargets;
        }

        List<Enemy> targets = new List<Enemy>();

        if (_upgradesManager.Value.HasUpgrade(UpgradeType.Lightning_ChanceToFork))
        {
            float rng = UnityEngine.Random.Range(0f, 1f);
            float chance = _upgradesManager.Value.GetUpgradeData(UpgradeType.Lightning_ChanceToFork).Value / 100;

            if (rng <= chance)
            {
                DetectionAmount = 2;
            }
        }
     
        for (int i = 0; i < DetectionAmount; i++)
        {
            if (DetectedTargets.Count == 0)
            {
                break;
            }
            
            int randomIndex = UnityEngine.Random.Range(0, DetectedTargets.Count);
            Enemy temp = DetectedTargets[randomIndex];

            if (temp != null && temp.State == EnemyState.Active && !temp.IsAlreadyElectrocuted())
            {
                targets.Add(temp);
            }
            else
            {
                i--;
            }
            
            DetectedTargets.Remove(temp);
        }

        ForceReturn();
        return targets;
    }

    public void ForceReturn()
    {
        gameObject.SetActive(false);
        _originalEntity = null;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Enemy e = col.GetComponent<Enemy>();
            
            if (col.gameObject != _originalEntity && !e.IsAlreadyElectrocuted() && e.State == EnemyState.Active)
            {
                DetectedTargets.Add(col.GetComponent<Enemy>());
            }
        }
    }
}
