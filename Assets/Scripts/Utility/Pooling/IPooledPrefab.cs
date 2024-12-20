using UnityEngine;

namespace Match3Test.Utility.Pooling
{
    //interface for classes using PrefabPool<T>.
    //Used to pass reference to object pool from prefab to prefab instances
    public interface IPooledPrefab<T> : IPooledPrefab where T : MonoBehaviour
    {
        public T GetInstance();
        public void ReturnToPool();
        public void SetPoolReference(ObjectPool<T> pool, Transform poolContainer);
    }

    public interface IPooledPrefab
    {
        public void InitPool(int prefetchCount = 0, Transform container = null);
    }
}