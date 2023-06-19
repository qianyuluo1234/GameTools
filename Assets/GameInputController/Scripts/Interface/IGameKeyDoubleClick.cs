using System;
using System.Diagnostics;

namespace GameTools
{
    public interface IGameKeyDoubleClick
    {
        int count { get; set; }
        int interval { get; set; }
        Stopwatch timer { get; }
        Action doubleClick { get; }
        Action doubleClickTimeout { get; }
        void AddListener(Action keyAction);
        void RemoveListener(Action keyAction);
        bool HasActions();
        void OnClickDown();
    }
}