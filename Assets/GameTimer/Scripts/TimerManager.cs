using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Init
// Update(){ TimerManager.instance.Tick(); }
// int tid = TimerManager.instance.AddTimerBy...    -> Start(tid)    -> (Pause(tid) -> Stop(tid) -> Restart(tid)) / DestroyTimer(tid)
// TimerManager.instance.Clear

/// <summary>
/// 计时器管理类
/// </summary>
public class TimerManager
{
    private static TimerManager _instance = null;
    private static object _lcok = new object();

    public static TimerManager instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lcok)
                {
                    if (_instance == null)
                    {
                        _instance = new TimerManager();
                    }
                }
            }

            return _instance;
        }
    }

    private Dictionary<int, BaseTimer> _readyTimerDic;

    /// <summary>
    /// 运行中的计时器字典
    /// </summary>
    private Dictionary<int, BaseTimer> _runTimerDic;

    /// <summary>
    /// 休眠计时器集合(需要手动释放Clear释放)
    /// </summary>
    private Dictionary<int, BaseTimer> _sleepTimerDic;

    private List<BaseTimer> _timerQueue;

    private MonoBehaviour _owner = null;

    /// <summary>
    /// 计时器池
    /// </summary>
    private List<BaseTimer> _timePools;

    private int _poolCapacity;
    private bool dirty { get; set; } = false;
    private bool enable { get; set; } = false;
    public int timerCount { get; private set; }

    private int _deltaTime = 10;

    private TimerManager()
    {
    }

    /// <summary>
    /// 初始化函数
    /// </summary>
    /// <param name="poolCapacity">计时器池容量。
    /// 超过数量时，多余未执行或停止执行的计时器将及时销毁。
    /// 计时器属性 autoDestroy = false 的计时器除外</param>
    /// <param name="owner"></param>
    public void Init(int poolCapacity, MonoBehaviour owner)
    {
        _readyTimerDic = new Dictionary<int, BaseTimer>();
        _runTimerDic = new Dictionary<int, BaseTimer>();
        _sleepTimerDic = new Dictionary<int, BaseTimer>();
        _timePools = new List<BaseTimer>();
        _timerQueue = new List<BaseTimer>();
        _poolCapacity = poolCapacity;
        timerCount = 0;
        _owner = owner;

        dirty = false;
        enable = true;

        owner.StartCoroutine(DirtyCheckOnEndOfFrame());
    }

    /// <summary>
    /// 初始化函数
    /// </summary>
    /// <param name="poolCapacity">计时器池容量。
    /// 超过数量时，多余未执行或停止执行的计时器将及时销毁。
    /// 计时器属性 autoDestroy = false 的计时器除外</param>
    /// <param name="deltaTime">
    /// 计时器实际的刷新仍然是靠Tick函数在Unity主线程中Update函数实现,
    /// 该参数是计时器管理类对计时器做排序的时间间隔(ms)，该排序操作在其他线程中执行</param>
    public void Init(int poolCapacity, uint deltaTime = 10)
    {
        _readyTimerDic = new Dictionary<int, BaseTimer>();
        _runTimerDic = new Dictionary<int, BaseTimer>();
        _sleepTimerDic = new Dictionary<int, BaseTimer>();
        _timePools = new List<BaseTimer>();
        _timerQueue = new List<BaseTimer>();
        _poolCapacity = poolCapacity;
        timerCount = 0;
        _deltaTime = (int) deltaTime;

        dirty = false;
        enable = true;
        Task.Run(DirtySortAsync);
    }
    
    public void Clear()
    {
        dirty = false;
        enable = false;
        timerCount = 0;
        _deltaTime = 16;
    }

    /// <summary>
    /// 新建毫秒计时器
    /// </summary>
    /// <param name="interval">计时器间隔</param>
    /// <param name="completeAction">计时器完成事件</param>
    /// <param name="autoDestroy">是否自动移除，默认为true</param>
    /// <returns></returns>
    public int AddTimerByMilliseconds(uint interval, Action completeAction, bool autoDestroy = true)
    {
        BaseTimer newTimer = null;
        if (_timePools.Count > 0)
        {
            newTimer = _timePools[_timePools.Count - 1];
            _timePools.Remove(newTimer);
        }
        else
        {
            newTimer = new BaseTimer();
            timerCount++;
        }

        _readyTimerDic.Add(newTimer.tId, newTimer);
        newTimer.Init(interval, completeAction, autoDestroy);

        return newTimer.tId;
    }

    /// <summary>
    /// 新建毫秒计时器
    /// </summary>
    /// <param name="interval">计时器间隔</param>
    /// <param name="loopAction">计时器循环事件</param>
    /// <param name="completeAction">计时器完成事件</param>
    /// <param name="loop">循环次数，默认为1。 0为无限循环</param>
    /// <param name="autoDestroy">是否自动移除，默认为true</param>
    /// <returns></returns>
    public int AddTimerByMilliseconds(uint interval, Action loopAction, Action completeAction, uint loop = 1,
        bool autoDestroy = true)
    {
        BaseTimer newTimer = null;
        if (_timePools.Count > 0)
        {
            newTimer = _timePools[_timePools.Count - 1];
            _timePools.Remove(newTimer);
        }
        else
        {
            newTimer = new BaseTimer();
            timerCount++;
        }

        _readyTimerDic.Add(newTimer.tId, newTimer);
        newTimer.Init(interval, loopAction, completeAction, loop, autoDestroy);

        return newTimer.tId;
    }

    /// <summary>
    /// 新建秒计时器
    /// </summary>
    /// <param name="interval">计时器间隔</param>
    /// <param name="completeAction">计时器完成事件</param>
    /// <param name="autoDestroy">是否自动移除，默认为true</param>
    /// <returns></returns>
    public int AddTimerBySeconds(float interval, Action completeAction, bool autoDestroy = true)
    {
        BaseTimer newTimer = null;
        if (_timePools.Count > 0)
        {
            newTimer = _timePools[_timePools.Count - 1];
            _timePools.Remove(newTimer);
        }
        else
        {
            newTimer = new BaseTimer();
            timerCount++;
        }

        _readyTimerDic.Add(newTimer.tId, newTimer);
        newTimer.Init((uint) (interval * 1000), completeAction, autoDestroy);

        return newTimer.tId;
    }

    /// <summary>
    /// 新建秒计时器
    /// </summary>
    /// <param name="interval">计时器间隔</param>
    /// <param name="loopAction">计时器循环事件</param>
    /// <param name="completeAction">计时器完成事件</param>
    /// <param name="loop">循环次数，默认为1。 0为无限循环</param>
    /// <param name="autoDestroy">是否自动移除，默认为true</param>
    /// <returns></returns>
    public int AddTimerBySeconds(float interval, Action loopAction, Action completeAction, uint loop = 1,
        bool autoDestroy = true)
    {
        BaseTimer newTimer = null;
        if (_timePools.Count > 0)
        {
            newTimer = _timePools[_timePools.Count - 1];
            _timePools.Remove(newTimer);
        }
        else
        {
            newTimer = new BaseTimer();
            timerCount++;
        }

        _readyTimerDic.Add(newTimer.tId, newTimer);

        newTimer.Init((uint) Math.Round(interval * 1000), loopAction, completeAction, loop, autoDestroy);

        return newTimer.tId;
    }

    /// <summary>
    /// 删除计时器
    /// </summary>
    /// <param name="tId"></param>
    public void DestroyTimer(int tId)
    {
        if (_readyTimerDic.ContainsKey(tId))
        {
            _readyTimerDic.Remove(tId);
            timerCount--;
        }
        else if (_runTimerDic.ContainsKey(tId))
        {
            Stop(tId);
        }
        else if (_sleepTimerDic.ContainsKey(tId))
        {
            _sleepTimerDic.Remove(tId);
            timerCount--;
        }
    }

    /// <summary>
    /// 计时器开始
    /// </summary>
    /// <param name="tId"></param>
    public void Start(int tId)
    {
        if (!_readyTimerDic.ContainsKey(tId))
        {
            Debug.Log($"<color=red>未找到计时器: {tId}</color>");
            return;
        }

        BaseTimer timer = _readyTimerDic[tId];
        _runTimerDic.Add(tId, timer);
        _runTimerDic[tId].Start();
        dirty = true;
    }

    /// <summary>
    /// 计时器停止
    /// </summary>
    /// <param name="tId"></param>
    public void Stop(int tId)
    {
        if (RemoveRunningTimer(tId, out BaseTimer timer))
        {
            timer.Stop();
            _timerQueue.Remove(timer);
        }
    }

    /// <summary>
    /// 计时器暂停
    /// </summary>
    /// <param name="tId"></param>
    /// <param name="value"></param>
    public void Pause(int tId, bool value)
    {
        if (!_runTimerDic.ContainsKey(tId))
        {
            return;
        }

        _runTimerDic[tId].Pause(value);

        dirty = true;
    }

    /// <summary>
    /// 计时器重新开始
    /// </summary>
    /// <param name="tId"></param>
    public void Restart(int tId)
    {
        if (_runTimerDic.ContainsKey(tId))
        {
            _runTimerDic[tId].Restart();
        }
        else if (_sleepTimerDic.ContainsKey(tId))
        {
            BaseTimer timer = _sleepTimerDic[tId];
            _sleepTimerDic.Remove(tId);
            _runTimerDic.Add(tId, timer);
            timer.Restart();
        }
        else if (_readyTimerDic.ContainsKey(tId))
        {
            Start(tId);
        }
        else
        {
            throw new ArgumentNullException($"not found timer. tid: <color=red>{tId}</color> ");
        }

        dirty = true;
    }

    public long GetActiveTime(int tId)
    {
        if (_runTimerDic.ContainsKey(tId))
        {
            return _runTimerDic[tId].GetActiveTime();
        }

        return 0;
    }

    /// <summary>
    /// 更新函数
    /// </summary>
    public void Tick()
    {
        if (!enable || _runTimerDic.Count <= 0)
        {
            return;
        }

        List<int> _removeIdList = new List<int>();
        lock (_timerQueue)
        {
            foreach (var timer in _timerQueue)
            {
                if (timer.GetLoopTimeRemaining() > 0)
                {
                    break;
                }

                timer.Execute();
                dirty = true;
                if (timer.state == TimerState.Stop)
                {
                    _removeIdList.Add(timer.tId);
                }
            }
        }

        foreach (var tid in _removeIdList)
        {
            RemoveRunningTimer(tid, out _);
        }
    }

    private bool RemoveRunningTimer(int tId, out BaseTimer timer)
    {
        timer = null;
        if (!_runTimerDic.ContainsKey(tId))
        {
            return false;
        }

        timer = _runTimerDic[tId];
        if (!timer.autoDestroy)
        {
            _sleepTimerDic.Add(timer.tId, timer);
        }
        else if (_timePools.Count < _poolCapacity)
        {
            timer.Reset();
            _timePools.Add(timer);
        }
        else
        {
            timerCount--;
        }

        return true;
    }

    /// <summary>
    /// 排序
    /// 按照剩余触发时间的从小到大排序
    /// </summary>
    private void Sort()
    {
        List<BaseTimer> bufferTimerQueue = new List<BaseTimer>();
        foreach (var timer in _runTimerDic.Values)
        {
            if (timer.state != TimerState.Running)
            {
                continue;
            }

            bufferTimerQueue.Add(timer);
        }

        bufferTimerQueue.Sort((t1, t2) => t1.GetLoopTimeRemaining().CompareTo(t2.GetLoopTimeRemaining()));
        lock (_timerQueue)
        {
            _timerQueue.Clear();
            _timerQueue.AddRange(bufferTimerQueue);
        }

        dirty = false;
    }

    /// <summary>
    /// 将标脏后的排序统一放置在帧末
    /// 优化排序次数
    /// 降低一帧内创建多个计时器时多次排序的问题
    /// </summary>
    /// <returns></returns>
    private IEnumerator DirtyCheckOnEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        _owner.StopCoroutine(DirtyCheckOnEndOfFrame());
        if (enable)
        {
            if (dirty)
            {
                Sort();
            }

            _owner.StartCoroutine(DirtyCheckOnEndOfFrame());
        }
        else
        {
            ClearData();
        }
    }

    private async Task DirtySortAsync()
    {
        while (enable)
        {
            if (dirty)
            {
                Sort();
            }

            await Task.Delay(_deltaTime);
        }

        ClearData();
    }

    private void ClearData()
    {
        foreach (var item in _runTimerDic)
        {
            item.Value.Stop();
        }

        _readyTimerDic.Clear();
        _runTimerDic.Clear();
        _sleepTimerDic.Clear();
        _timePools.Clear();
        _timerQueue.Clear();

        _readyTimerDic = null;
        _runTimerDic = null;
        _sleepTimerDic = null;
        _timePools = null;
        _timerQueue = null;

        _owner = null;
    }
}
