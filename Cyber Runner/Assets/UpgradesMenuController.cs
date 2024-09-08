using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class UpgradesMenuController : MonoBehaviour
{
    public GameObject UpgradesRemainingObject;
    public GameObject UpgradeIconPrefab;

    private LazyService<PrefabPool> _prefabPool;
    private LazyService<EXPManager> _expManager;

    private void AddUpgradeIcon()
    {
        GameObject icon = _prefabPool.Value.Get(UpgradeIconPrefab);
        icon.transform.parent = UpgradesRemainingObject.transform;
        icon.transform.SetAsLastSibling();
    }
    
    public void RemoveUpgradeIcon()
    {
        if (UpgradesRemainingObject.transform.childCount <= 0)
        {
            return;
        }
        
        GameObject icon = UpgradesRemainingObject.transform.GetChild(UpgradesRemainingObject.transform.childCount-1).gameObject;
        icon.transform.parent = null;
        _prefabPool.Value.Return(icon);
    }

    public void ShowUpgradeIcons(float delay)
    {
        StartCoroutine(DelayedShow());

        IEnumerator DelayedShow()
        {
            yield return new WaitForSeconds(delay);
            
            for (int i = 0; i <= _expManager.Value.UnclaimedLevels; i++)
            {
                AddUpgradeIcon();
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        
        
    }
}
