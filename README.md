# GameTools

　　这是一个工具集合。会不定时的进行更新，如有需求，请自行下载即可。

## 声明

　　本项目仅是基于以往的工作学习经验，故其中可能会存在一些问题。使用者请考虑好工具是否在适用范围。

## Tools

　　以下是本集合中包含的工具及其简介。

### １.Timer

　　本计时器使用Stopwatch类来实现计时功能。计时器提供了两种初始化方案，一种是采用异步排序的方式(传入异步排序更新的时间)，另一种是帧末排序的方式(传入执行协程的Monobehavior对象)。其中排序是对计时器的优化方法，其核心是对计时器到循环点的剩余时间的从小到大排序。计时器的更新部分需要在Unity的Update函数中进行。

　　计时器的所有行为均由TimerManager进行管理，TimerManager是一个单例类，相关使用方法请查看[Timer教程.md](./Assets/Document/Timer/Timer教程.md)。如需了解详情，请自行查看源码。目前该计时器暂不支持timeScale(没有做相关的实现，后续会去实现该功能，但时间不定)。

### ２.ObjectPool

          简单的对象池封装，支持Lua。建议自行创建维护一个Pool Id List来对所有的Pool使用与管理 

### ３.GameInputController

　　GameInputController是对Unity中Input系统的简单封装。将原本通过调用Input类实现的按键和轴行为包装成使用GameInputController类进行统一注册管理的事件系统。

## 尾语

　　最后希望以上工具能对您的工作有小小的帮助。