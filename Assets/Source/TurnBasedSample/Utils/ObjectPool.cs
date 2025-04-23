using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedSample.Utils
{
    public class ObjectPool<T> where T : Component
    {
        private T prefab;
        private int initialPoolSize;
        private bool expandable;
        
        private Stack<T> availableObjects;
        private HashSet<T> activeObjects;
        private Transform poolContainer;

        public IReadOnlyCollection<T> ActiveObjects => activeObjects;
        public int AvailableCount => availableObjects.Count;
        public int ActiveCount => activeObjects.Count;
        public int TotalCount => AvailableCount + ActiveCount;

        public static ObjectPool<T> Create(T prefab, int initialPoolSize, bool expandable)
        {
            var pool = new ObjectPool<T>(prefab, initialPoolSize, expandable);
            pool.Init();
            return pool;
        }

        private ObjectPool()
        {
        }
        
        private ObjectPool(T prefab, int initialPoolSize, bool expandable)
        {
            this.prefab = prefab;
            this.initialPoolSize = initialPoolSize;
            this.expandable = expandable;
            
            availableObjects = new Stack<T>(initialPoolSize);
            activeObjects = new HashSet<T>(initialPoolSize);
        }

        private void Init()
        {
            if (poolContainer == null)
            {
                poolContainer = new GameObject($"Pool_{prefab.name}").transform;
                PrewarmPool(initialPoolSize);
            }
        }

        private void CreateNewInstance()
        {
            T instance = Object.Instantiate(prefab, poolContainer);
            instance.gameObject.SetActive(false);
            availableObjects.Push(instance);
        }

        public T Get()
        {
            if (availableObjects.Count == 0)
            {
                if (!expandable)
                {
                    Debug.LogWarning($"Pool for {prefab.name} is empty and not expandable!");
                    return null;
                }
                CreateNewInstance();
            }

            T instance = availableObjects.Pop();
            activeObjects.Add(instance);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public bool Return(T instance)
        {
            if (instance == null) return false;

            if (!activeObjects.Remove(instance))
            {
                Debug.LogWarning($"Attempting to return an object that isn't from this pool: {instance.name}");
                return false;
            }

            instance.transform.SetParent(poolContainer);
            instance.gameObject.SetActive(false);
            availableObjects.Push(instance);
            return true;
        }

        public void ReturnAll()
        {
            var objectsToReturn = new List<T>(activeObjects);
            foreach (T instance in objectsToReturn)
            {
                Return(instance);
            }
        }

        public void PrewarmPool(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                CreateNewInstance();
            }
        }

        public bool IsPooledObject(T instance)
        {
            return activeObjects.Contains(instance) || availableObjects.Contains(instance);
        }
    }
}