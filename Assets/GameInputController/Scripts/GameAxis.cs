using System;
using UnityEngine;
using System.Linq;

namespace GameTools
{
    public class GameAxis:IGameAxis
    {
        public string axisName { get; }
        public float value => Input.GetAxis(axisName);
        public bool active { get; set; }
        private Action<float> _gameAxisAction;

        public GameAxis(string axisName)
        {
            this.axisName = axisName;
        }
        

        public void AddListener(Action<float> axisAction)
        {
            _gameAxisAction += axisAction;
        }

        public void RemoveListener(Action<float> axisAction)
        {
            _gameAxisAction -= axisAction;
        }

        public void RemoveAllListener()
        {
            _gameAxisAction = null;
        }

        public void Invoke()
        {
            _gameAxisAction.Invoke(value);
        }

        public bool Contains(Action<float> axisAction)
        {
            return _gameAxisAction != null && _gameAxisAction.GetInvocationList().Contains(axisAction);
        }

        public bool HasActions()
        {
            return _gameAxisAction != null;
        }
    }
}