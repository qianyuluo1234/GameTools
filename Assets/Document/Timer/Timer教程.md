# Timer教程

## 初始化

使用本计时器工具首先需要进行初始化，调用TimerManager.instance.Init()函数即可完成初始化。全局仅执行一次。

关于Init函数，有两个重载，其涉及对计时器采用不同的排序方式。

Init(int deltaTime)  采用异步排序的方式初始化。deltaTime表示异步排序的时间间隔

Init(MonoBehaviour owner) 采用协程的方式初始化。owner表示执行协程的对象，该协程每帧末执行

## 清理

TimerManager.instance.Clear()

Clear()函数的调用表示计时器工具的功能结束，重新使用计时器需要调用初始化Init函数。故通常只需要在app结束时调用即可。该函数设计的初衷是为解决在释放lua环境时，仍然有计时器占用lua资源的问题(通常来说就是仍在运行或休眠中的包含lua函数的计时器)。所以在lua环境中使用时，需要在lua环境结束前调用。

## 计时器更新

TimerManager.instance.Tick()

由于TimerManager并未继承MonoBehaviour,故计时器更新需要将Tick方法在一个全局的MonoBehaviour对象的Update函数中调用。