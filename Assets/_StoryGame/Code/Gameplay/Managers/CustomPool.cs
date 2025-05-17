using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace _StoryGame.Gameplay.Managers
{
    public class CustomPool<T> where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly bool _allowGrowth;
        private readonly int _poolSize;
        private readonly Transform _parent;
        private readonly IObjectResolver _container;

        private readonly Queue<T> _cache = new();
        private readonly HashSet<T> _activeObjects = new(); // Для проверки активных объектов


        public CustomPool(T prefab, int poolSize, Transform parent, IObjectResolver container, bool allowGrowth = false)
        {
            _prefab = prefab;
            _poolSize = poolSize;
            _parent = parent;
            _allowGrowth = allowGrowth;
            _container = container;

            Initialize();
        }

        private void Initialize()
        {
            for (var i = 0; i < _poolSize; i++) CreateObject();
        }

        private void CreateObject()
        {
            var go = Object.Instantiate(_prefab, _parent);
            _container.Inject(go);
            go.gameObject.SetActive(false);
            _cache.Enqueue(go);
        }

        public T Get()
        {
            if (_cache.Count > 0)
            {
                var obj = _cache.Dequeue();
                obj.gameObject.SetActive(false);
                _activeObjects.Add(obj);
                return obj;
            }

            if (!_allowGrowth) throw new NullReferenceException("Pool is empty and growth is not allowed!");

            var newObj = Object.Instantiate(_prefab, _parent);
            newObj.gameObject.SetActive(false);
            _activeObjects.Add(newObj);
            return newObj;
        }

        public void Return(T obj)
        {
            if (obj is null)
            {
                // Debug.Log("Can't return null object to pool!");
                return;
            }

            if (_activeObjects.Contains(obj))
            {
                obj.gameObject.SetActive(false);
                _activeObjects.Remove(obj);
                _cache.Enqueue(obj);
            }
            // else Debug.Log($"Object was taken not from this pool! {obj}");
        }
    }
}
