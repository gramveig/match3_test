using System;
using UnityEngine;

namespace Match3Test.Utility.Pooling
{
    /// <summary>
    /// Allow to create pool for selected prefabs at start
    /// if creating pool for them in code is inconvenient
    /// </summary>
    public class PrefabPoolInitializer : MonoBehaviour
    {
        [Serializable]
        private class PooledPrefabData
        {
            public GameObject Prefab;
            public int prefetchCount;
        }

        [SerializeField] private PooledPrefabData[] prefabs;

        public void Start()
        {
            foreach (PooledPrefabData prefabData in prefabs)
            {
                GameObject prefab = prefabData.Prefab;
                IPooledPrefab pooledPrefab = prefab.GetComponent<IPooledPrefab>();
                if (pooledPrefab == null)
                {
                    Debug.LogError($"Prefab {prefab} contains no components implementing IPooledPrefab interface");
                    continue;
                }

                pooledPrefab.InitPool(prefabData.prefetchCount);
            }
        }
    }
}