using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameTools
{
    public class PoolBase : IPoolBase
    {
        private int _capacity = -1;
        private Type _spawnType = null;
        private List<Object> _activeList;
        private List<Object> _inactiveList;
        private Func<Object> _createFunc;
        private Action<Object> _destroyAction;
        private Action<Object> _initAction;
        private Action<Object> _resetAction;

        public PoolBase(int capacity,Type spawnType, Func<Object> createFunc, Action<Object> destroyAction,
            Action<Object> initAction = null, Action<Object> resetAction = null)
        {
            if (createFunc == null || destroyAction == null)
            {
                throw new ArgumentException("创建方法或移除方法为空!!!");
            }

            if (capacity < -1)
            {
                capacity = -1;
            }

            _capacity = capacity;
            _spawnType = spawnType;
            _createFunc = createFunc;
            _destroyAction = destroyAction;
            _initAction = initAction;
            _resetAction = resetAction;

            _activeList = new List<Object>();
            _inactiveList = new List<Object>();
        }

        public Object Load()
        {
            Object o = null;
            if (_activeList.Count <= 0)
            {
                o = _createFunc.Invoke();
            }
            else
            {
                int lastIndex = _inactiveList.Count - 1;
                o = _inactiveList[lastIndex];
                _inactiveList.RemoveAt(lastIndex);
            }

            _initAction?.Invoke(o);
            _activeList.Add(o);
            return o;
        }

        public void Save(Object o)
        {
            if (o == null || o.GetType() != _spawnType)
            {
                return;
            }
            
            if (_capacity == -1 || (_capacity > -1 && _inactiveList.Count < _capacity))
            {
                if (!_activeList.Contains(o))
                {
                    Debug.Log($"activeList未找到 {o}");
                }
                else
                {
                    _activeList.Remove(o);
                }
                
                _resetAction?.Invoke(o);
                _inactiveList.Add(o);
            }
            else
            {
                Destroy(o);
            }
        }

        public void Destroy(Object o)
        {
            if (o == null || o.GetType() != _spawnType)
            {
                return;
            }

            if (_activeList.Contains(o))
            {
                _activeList.Remove(o);
            }
            else if (_inactiveList.Contains(o))
            {
                _inactiveList.Remove(o);
            }
            else
            {
                Debug.Log($"未在池中找到 {o} ");
            }

            _destroyAction.Invoke(o);
        }

        public void Clear(bool isDestroy)
        {
            if (isDestroy)
            {
                foreach (var item in _activeList)
                {
                    Destroy(item);
                }

                foreach (var item in _inactiveList)
                {
                    Destroy(item);
                }
            }

            _activeList.Clear();
            _inactiveList.Clear();
            _spawnType = null;
            _createFunc = null;
            _initAction = null;
            _resetAction = null;
            _activeList = null;
            _inactiveList = null;
        }
    }
}