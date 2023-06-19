using System;
using UnityEngine;

namespace GameTools
{
    public interface IGameKey
    {
        KeyCode keyCode { get; }
        bool active { get; set; }
        bool activeDoubleClick { get; set; }
        IGameKeyPress keyPress { get; set; }
        IGameKeyDown keyDown { get; set; }
        IGameKeyUp keyUp { get; set; }

        IGameKeyDoubleClick  keyDoubleClick{ get; set; }

        void Invoke();
        bool HasActions();
    }
}