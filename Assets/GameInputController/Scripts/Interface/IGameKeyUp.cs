using System;

namespace GameTools
{
    public interface IGameKeyUp
    {
        bool enable { get; }
        Action up { get; }
        void AddListener(Action keyAction);
        void RemoveListener(Action keyAction);
        bool HasActions();
    }
}