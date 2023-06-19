using System;
using UnityEngine;

namespace GameTools
{
    public class GameKeyPress : IGameKeyPress
    {
        public bool enable => Input.GetKey(_keyCode);
        public Action press { get; private set; }
        private KeyCode _keyCode;
        public GameKeyPress(KeyCode keyCode)
        {
            _keyCode = keyCode;
        }
        public void AddListener(Action keyAction)
        {
            press += keyAction;
        }

        public void RemoveListener(Action keyAction)
        {
            press -= keyAction;
        }

        public bool HasActions()
        {
            return press != null;
        }
    }
}