using System;

namespace GameTools
{
    public interface IGameKeyDown
    {
        bool enable { get; }
        Action down { get; }
        void AddListener(Action keyAction);
        void RemoveListener(Action keyAction);
        bool HasActions();
    }
}