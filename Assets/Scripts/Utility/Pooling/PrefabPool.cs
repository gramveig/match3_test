using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match3Test.Utility.Pooling
{
    [Serializable]
    public class PrefabPool<T> where T : MonoBehaviour
    {
        private ObjectPool<T> _prefabPool;
        private bool _detachInstances = true;
        private Transform _poolObjectsContainer;
        private T _prefab;

        public void InitPool(T prefab, int prefetchCount = 0, Transform container = null)
        {
            //we initialize only one pool per prefab
            if (IsInitialized) return;

            _prefab = prefab;

#if UNITY_EDITOR
            //keep instances in container in Unity editor for the tidy hierarchy window.
            //Detach them from container in build, to gain more performance
            _detachInstances = false;
#endif
            if (container == null)
            {
                //container will persist in the build even though no object instances are placed
                //in them: they serve as indicators of scene reloading
                GameObject containerObject = new GameObject();
                containerObject.name = "Container" + prefab.gameObject.name;
                _poolObjectsContainer = containerObject.transform;
            }
            else
            {
                //sometimes we need to use a container, for example when instantiating UI objects
                _poolObjectsContainer = container;
                _detachInstances = false;
            }

            _prefabPool = new ObjectPool<T>(InstantiatePoolObject, prefetchCount);
            if (prefetchCount > 0)
                _prefabPool.Prefetch(prefetchCount);
        }

        public void SetPoolReference(ObjectPool<T> pool, Transform poolContainer)
        {
            _prefabPool = pool;
            _poolObjectsContainer = poolContainer;
        }

        public T GetInstance(T prefab)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning($"Creating prefab pool for {prefab.gameObject} on the run. Please add prefab to the prefab initialization list");
                InitPool(prefab);
            }

            return _prefabPool.Draw();
        }
        
        public void ReturnToPool(T obj)
        {
            if (_prefabPool == null)
                Debug.LogError($"Prefab pool is null for object '{obj.name}'");

            _prefabPool.Return(obj);
        }

        public bool Contains(T obj)
        {
            return _prefabPool.Contains(obj);
        }
        
        public bool IsInitialized =>
            //we initialize pool only once per prefab
               _prefabPool != null
            //as prefabs are not reset when the scene is reloaded, we have to reinit pool when this happens
            && _poolObjectsContainer != null
        ;

        private T InstantiatePoolObject()
        {
            T objectInstance;
            if (_detachInstances)
                objectInstance = Object.Instantiate(_prefab);
            else
                objectInstance = Object.Instantiate(_prefab, _poolObjectsContainer);

            //prefab pool is set in prefab but not in prefab instance,
            //thus we have to copy the pool reference to instance
            var poolableInstance = (IPooledPrefab<T>) objectInstance;
            poolableInstance.SetPoolReference(_prefabPool, _poolObjectsContainer);

            return objectInstance;
        }
    }
}