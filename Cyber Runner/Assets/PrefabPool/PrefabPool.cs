
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.Pool;

    [DefaultExecutionOrder(-100)]
    public class PrefabPool : MonoService
    {
        private readonly List<(ObjectPool<GameObject> objPool, int hash)> _prefabPools = new();

        private bool _sceneDestroyed;

        public void OnDestroy()
        {
            //leveraging DefaultExecutionOrder for this to be run first, thereby flagging that any other pool objects being destroyed is ok :)
            _sceneDestroyed = true;
        }

        /// <summary>
        ///     Get an instance of the prefab type from the pool
        /// </summary>
        /// <param name="prefab">MUST be a prefab not instance as prefab GUID will be used to find pool.</param>
        public GameObject Get(GameObject prefab)
        {
            int hashCode = prefab.GetHashCode();
            (ObjectPool<GameObject> objPool, int hash)? pool = null;

            for (int i = 0; i < _prefabPools.Count; i++)
            {
                (ObjectPool<GameObject> objPool, int hash) candidate = _prefabPools[i];

                if (candidate.hash == hashCode)
                {
                    pool = candidate;

                    break;
                }
            }

            if (pool == null)
            {
                ObjectPool<GameObject> newPool = new(() =>
                    {
                        GameObject g = Instantiate(prefab);
                        PrefabPoolObject o = g.AddComponent<PrefabPoolObject>();
                        o.Pool = this;
                        o.PrefabHash = hashCode;
                        g.name = $"{prefab.name}(Pooled)";

                        return g;
                    },
                    instance =>
                    {
                        instance.SetActive(true);
                    },
                    instance =>
                    {
                        instance.SetActive(false);
                    },
                    instance =>
                    {
                        if (instance != null)
                        {
                            Destroy(instance);
                        }
                    });
                pool = (newPool, hashCode);
                _prefabPools.Add(pool.Value);
            }

            return pool.Value.objPool.Get();
        }

        public void Return(GameObject instance)
        {
            PrefabPoolObject poolObject = instance.GetComponent<PrefabPoolObject>();

            if (poolObject == null)
            {
                return;
            }

            for (int i = 0; i < _prefabPools.Count; i++)
            {
                (ObjectPool<GameObject> objPool, int hash) candidate = _prefabPools[i];

                if (candidate.hash == poolObject.PrefabHash)
                {
                    candidate.objPool.Release(instance);

                    return;
                }
            }
        }

        public void NotifyDestroyed(GameObject instance)
        {
            if (_sceneDestroyed)
            {
                return;
            }

            Debug.LogWarning(
                $"DESTROYING OBJECT THAT'S PART OF A PREFAB POOL! Call Return(obj) rather. (Or maybe this is scene unload) {instance.name}");
        }
    }
