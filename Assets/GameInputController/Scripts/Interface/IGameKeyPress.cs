using System;

namespace GameTools
{
    public interface IGameKeyPress
    {
        bool enable { get; }
        Action press { get; }
        void AddListener(Action keyAction);
        void RemoveListener(Action keyAction);
        bool HasActions();
    }
}