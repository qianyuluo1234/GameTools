using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameTools
{
    public class PoolManager
    {
        private static PoolManager _instance = null;
        private static object _lock = new object();
        public static PoolManager instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new PoolManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private Dictionary<int, IPoolBase> _poolDic = null;

        private PoolManager() { }

        /// <summary>
        /// 对象池管理类的初始化方法
        /// </summary>
        public void Init()
        {
            _poolDic = new Dictionary<int, IPoolBase>();
        }
        /// <summary>
        /// 清理对象池管理类
        /// 调用该方法后，若想使用对象池，必须先调用初始化方法
        /// </summary>
        public void Clear(bool isDestroy = true)
        {
            foreach (var pool in _poolDic.Values)
            {
                pool.Clear(isDestroy);
            }
            _poolDic.Clear();
            _poolDic = null;
        }

        /// <summary>
        /// 创建指定id的对象池
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="capacity">对象池容量。超过容量数量后的对象在回收后销毁。-1为无限容量</param>
        /// <param name="spawnType"></param>
        /// <param name="createFunc"></param>
        /// <param name="initAction"></param>
        /// <param name="resetAction"></param>
        /// <returns></returns>
        public bool CreatePool(int poolId,int capacity, Type spawnType, Func<Object> createFunc, Action<Object> destroyAction, Action<Object> initAction = null, Action<Object> resetAction = null)
        {
            if (_poolDic.ContainsKey(poolId))
            {
                Debug.Log($"{poolId} pool已经存在!!!");
                return false;
            }
            _poolDic.Add(poolId, new PoolBase(capacity, spawnType, createFunc, destroyAction, initAction, resetAction));
            return true;
        }

        /// <summary>
        /// 移除指定id的对象池
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="isDestroy"></param>
        /// <returns></returns>
        public bool RemovePool(int poolId, bool isDestroy = true)
        {
            if (!_poolDic.ContainsKey(poolId))
            {
                Debug.Log($"{poolId} pool已经移除");
                return false;
            }
            _poolDic[poolId].Clear(isDestroy);
            _poolDic.Remove(poolId);
            return true;
        }

        /// <summary>
        /// 从指定对象池中获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolId"></param>
        /// <returns></returns>
        public T Load<T>(int poolId) where T : Object
        {
            if (!_poolDic.ContainsKey(poolId))
            {
                return null;
            }

            return (T)_poolDic[poolId].Load();
        }

        public Object Load(int poolId)
        {
            if (!_poolDic.ContainsKey(poolId))
            {
                return null;
            }
            return _poolDic[poolId].Load();
        }

        /// <summary>
        /// 向指定对象池中存入实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolId"></param>
        /// <param name="t"></param>
        public void Save<T>(int poolId, T t) where T : Object
        {
            if (!_poolDic.ContainsKey(poolId))
            {
                return;
            }
            _poolDic[poolId].Save(t);
        }
        public void Save(int poolId, Object o)
        {
            if (!_poolDic.ContainsKey(poolId))
            {
                return;
            }
            _poolDic[poolId].Save(o);
        }

        /// <summary>
        /// 从指定对象池中移除实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolId"></param>
        /// <param name="t"></param>
        public void Destroy<T>(int poolId, T t) where T : Object
        {
            if (!_poolDic.ContainsKey(poolId))
            {
                return;
            }
            _poolDic[poolId].Destroy(t);
        }
        /// <summary>
        /// 从指定对象池中移除实例
        /// </summary>
        /// <param name="poolId"></param>
        /// <param name="o"></param>
        public void Destroy(int poolId, Object o)
        {
            if (!_poolDic.ContainsKey(poolId))
            {
                return;
            }
            _poolDic[poolId].Destroy(o);
        }
    }
}