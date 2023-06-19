using System;

namespace GameTools
{
    public interface IGameAxis
    {
        public string axisName { get; }
        float value { get; }
        bool active { get; set; }
        void AddListener(Action<float> axisAction);
        void RemoveListener(Action<float> axisAction);
        void RemoveAllListener();
        void Invoke();
        bool Contains(Action<float> axisAction);

        bool HasActions();
    }
    
}