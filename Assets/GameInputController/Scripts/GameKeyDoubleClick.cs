using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace GameTools
{
    public class GameKeyDoubleClick : IGameKeyDoubleClick
    {
        public int count { get; set; }
        public int interval { get; set; } = 400;
        public Stopwatch timer { get; private set; }
        public Action doubleClick { get; private set; }
        public Action doubleClickTimeout { get; private set; }
        
        public void AddListener(Action keyAction)
        {
            doubleClick += keyAction;
        }

        public void RemoveListener(Action keyAction)
        {
            doubleClick -= keyAction;
        }
        
        public bool HasActions()
        {
            return doubleClick != null;
        }

        private void Start()
        {
            Debug.Log("double click start...");
            timer = Stopwatch.StartNew();
            doubleClickTimeout = Timeout;
        }

        private void End()
        {
            Debug.Log($"double click end... click interval: {timer.ElapsedMilliseconds}");
            count = 0;
            timer.Stop();
            timer = null;
            doubleClickTimeout = null;
        }

        public void OnClickDown()
        {
            count++;
            if (count == 1)
            {
                Start();   
            }
            else if (count == 2)
            {
                End();
                doubleClick.Invoke();  
            }
        }
        private void Timeout()
        {
            if (timer.ElapsedMilliseconds > interval)
            {
                Debug.Log($"double click timeout. {timer.ElapsedMilliseconds}");
                End();
            }
        }
        
    }
}