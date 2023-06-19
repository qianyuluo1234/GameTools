using System;
using UnityEngine;

namespace GameTools
{
    public class GameKeyDown :IGameKeyDown
    {
        public bool enable => Input.GetKeyDown(_keyCode);
        public Action down { get; private set; }

        private KeyCode _keyCode;

        public GameKeyDown(KeyCode keyCode)
        {
            _keyCode = keyCode;
        }

        public void AddListener(Action keyAction)
        {
            down += keyAction;
        }

        public void RemoveListener(Action keyAction)
        {
            down -= keyAction;
        }
        
        public bool HasActions()
        {
            return down != null;
        }
    }
}