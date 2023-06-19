using System;
using System.Diagnostics;
using UnityEngine;

namespace GameTools
{
    public class GameKey : IGameKey
    {
        public KeyCode keyCode { get; }
        public bool active { get; set; } = true;
        public bool activeDoubleClick { get; set; } = false;
        public IGameKeyPress keyPress { get; set; }
        public IGameKeyDown keyDown { get; set; }
        public IGameKeyUp keyUp { get; set; }
        public IGameKeyDoubleClick keyDoubleClick { get; set; }
        
        /// <summary>
        /// 注册双击事件后自动激活，没有双击事件时为false
        /// 避免无谓的双击检测
        /// </summary>

        public GameKey(KeyCode keyCode)
        {
            this.keyCode = keyCode;
        }
        
        public void Invoke()
        {
            if (keyDoubleClick != null && activeDoubleClick)
            {
                keyDoubleClick.doubleClickTimeout?.Invoke();
            }

            if (keyPress != null && keyPress.enable)
            {
                keyPress.press.Invoke();
            }

            if (keyDown!= null && keyDown.enable)
            {
                keyDown.down.Invoke();
            }
            else if (keyUp != null && keyUp.enable)
            {
                keyUp.up.Invoke();
            }
        }

        public bool HasActions()
        {
            return (keyPress != null ? keyPress.HasActions() : false)
                   || (keyDown != null ? keyDown.HasActions() : false)
                   || (keyUp != null ? keyUp.HasActions() : false)
                   || (keyDoubleClick != null ? keyDoubleClick.HasActions() : false);
        }
    }
}