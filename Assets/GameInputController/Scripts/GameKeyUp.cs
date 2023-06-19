using System;
using UnityEngine;

namespace GameTools
{
    public class GameKeyUp : IGameKeyUp
    {
        public bool enable => Input.GetKeyUp(_keyCode);
        public Action up { get; private set; }

        private KeyCode _keyCode;

        public GameKeyUp(KeyCode keyCode)
        {
            _keyCode = keyCode;
        }
        public void AddListener(Action keyAction)
        {
            up += keyAction;
        }

        public void RemoveListener(Action keyAction)
        {
            up -= keyAction;
        }
        public bool HasActions()
        {
            return up != null;
        }
    }
}