using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameTools
{
    public enum EGameInputAxis : sbyte
    {
        X = 0,
        Y,
        MouseX,
        MouseY,
    }

    public class GameInputController : MonoBehaviour
    {
        private static GameInputController _instance = null;

        public static GameInputController instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GameInputController>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameInputController");
                        _instance = go.AddComponent<GameInputController>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        private Dictionary<sbyte, IGameAxis> _axisDic;
        private Dictionary<int, IGameKey> _keyDic;

        private void Awake()
        {
            _axisDic = new Dictionary<sbyte, IGameAxis>()
            {
                {(sbyte) EGameInputAxis.X, new GameAxis("Horizontal")},
                {(sbyte) EGameInputAxis.Y, new GameAxis("Vertical")},
                {(sbyte) EGameInputAxis.MouseX, new GameAxis("Mouse X")},
                {(sbyte) EGameInputAxis.MouseY, new GameAxis("Mouse Y")},
            };
            _keyDic = new Dictionary<int, IGameKey>();
        }

        private void Update()
        {
            foreach (var axis in _axisDic.Values)
            {
                if (axis.active)
                {
                    axis.Invoke();
                }
            }

            foreach (var key in _keyDic.Values)
            {
                if (key.active)
                {
                    key.Invoke();
                }
            }
        }

        public void AddAxisListener(Action<float> axisAction, EGameInputAxis axis)
        {
            sbyte axisId = (sbyte) axis;
            if (!_axisDic[axisId].HasActions() && !_axisDic[axisId].active && axisAction != null)
            {
                _axisDic[axisId].active = true;
            }
            _axisDic[axisId].AddListener(axisAction);
        }
        public void RemoveAxisListener(Action<float> axisAction, EGameInputAxis axis)
        {
            sbyte axisId = (sbyte) axis;

            if (!_axisDic[axisId].HasActions())
            {
                return;
            }
            
            _axisDic[axisId].RemoveListener(axisAction);
            if (!_axisDic[axisId].HasActions() && _axisDic[axisId].active)
            {
                _axisDic[axisId].active = false;
            }

        }

        public void RemoveAllAxisListener(EGameInputAxis axis)
        {
            _axisDic[(sbyte) axis].RemoveAllListener();
        }



        public bool Contains(Action<float> axisAction, EGameInputAxis axis)
        {
            return _axisDic[(sbyte) axis].Contains(axisAction);
        }

        public bool SetActiveAxis(EGameInputAxis axis,bool value)
        {
            if (_axisDic[(sbyte) axis].HasActions())
            {
                _axisDic[(sbyte) axis].active = value;
                return true;
            }
            return false;
        }
        public void AddKeyPressListener(Action keyAction, KeyCode keyCode)
        {
            int keyId = (int) keyCode;
            if (!_keyDic.ContainsKey(keyId))
            {
                _keyDic.Add(keyId, new GameKey(keyCode));
            }
            if (_keyDic[keyId].keyPress == null)
            {
                _keyDic[keyId].keyPress = new GameKeyPress(keyCode);
            }
            _keyDic[keyId].keyPress.AddListener(keyAction);
        }
        public void AddKeyDownListener(Action keyAction, KeyCode keyCode)
        {
            int keyId = (int) keyCode;
            if (!_keyDic.ContainsKey(keyId))
            {
                _keyDic.Add(keyId, new GameKey(keyCode));
            }
            if (_keyDic[keyId].keyDown == null)
            {
                _keyDic[keyId].keyDown = new GameKeyDown(keyCode);
            }
            _keyDic[keyId].keyDown.AddListener(keyAction);
        }
        public void AddKeyUpListener(Action keyAction, KeyCode keyCode)
        {
            int keyId = (int) keyCode;
            if (!_keyDic.ContainsKey(keyId))
            {
                _keyDic.Add(keyId, new GameKey(keyCode));
            }
            if (_keyDic[keyId].keyUp == null)
            {
                _keyDic[keyId].keyUp = new GameKeyUp(keyCode);
            }
            _keyDic[keyId].keyUp.AddListener(keyAction);
        }
        public void AddDoubleClickListener(Action keyAction, KeyCode keyCode)
        {
            int keyId = (int) keyCode;
            if (!_keyDic.ContainsKey(keyId))
            {
                _keyDic.Add(keyId, new GameKey(keyCode));
            }
            if (_keyDic[keyId].keyDoubleClick == null)
            {
                _keyDic[keyId].keyDoubleClick = new GameKeyDoubleClick();
            }
            _keyDic[keyId].keyDoubleClick.AddListener(keyAction);

            if (_keyDic[keyId].keyDoubleClick.HasActions() && !_keyDic[keyId].activeDoubleClick)
            {
                AddKeyDownListener(_keyDic[keyId].keyDoubleClick.OnClickDown, keyCode);
                _keyDic[keyId].activeDoubleClick = true;
            }
        }
        
        public void RemoveKeyPressListener(Action keyAction, KeyCode keyCode)
        {
            int keyId = (int) keyCode;
            if (!_keyDic.ContainsKey(keyId) || _keyDic[keyId].keyPress == null)
            {
                return;
            }
            _keyDic[keyId].keyPress.RemoveListener(keyAction);

            if (_keyDic[keyId].keyPress.HasActions())
            {
                return;
            }
            
            if (!_keyDic[keyId].HasActions())
            {
                _keyDic.Remove(keyId);
            }
            else
            {
                _keyDic[keyId].keyPress = null;
            }

        }
        public void RemoveKeyDownListener(Action keyAction, KeyCode keyCode)
        {
            int keyId = (int) keyCode;
            if (!_keyDic.ContainsKey(keyId) || _keyDic[keyId].keyDown == null)
            {
                return;
            }
            _keyDic[keyId].keyDown.RemoveListener(keyAction);

            if (_keyDic[keyId].keyDown.HasActions())
            {
                return;
            }
            
            if (!_keyDic[keyId].HasActions())
            {
                _keyDic.Remove(keyId);
            }
            else
            {
                _keyDic[keyId].keyDown = null;
            }
        }
        public void RemoveKeyUpListener(Action keyAction, KeyCode keyCode)
        {
            int keyId = (int) keyCode;
            if (!_keyDic.ContainsKey(keyId) || _keyDic[keyId].keyUp == null)
            {
                return;
            }
            _keyDic[keyId].keyUp.RemoveListener(keyAction);

            if (_keyDic[keyId].keyUp.HasActions())
            {
                return;
            }
            
            if (!_keyDic[keyId].HasActions())
            {
                _keyDic.Remove(keyId);
            }
            else
            {
                _keyDic[keyId].keyUp = null;
            }
        }
        public void RemoveDoubleClickListener(Action keyAction, KeyCode keyCode)
        {
            int keyId = (int) keyCode;
            
            if (!_keyDic.ContainsKey(keyId) || _keyDic[keyId].keyDown == null || _keyDic[keyId].keyDoubleClick == null)
            {
               return;
            }
            _keyDic[keyId].keyDoubleClick.RemoveListener(keyAction);
            if (_keyDic[keyId].keyDoubleClick.HasActions())
            {
                return;
            }
            IGameKeyDoubleClick doubleClick = _keyDic[keyId].keyDoubleClick;
            _keyDic[keyId].keyDoubleClick = null;
            _keyDic[keyId].activeDoubleClick = false;
            RemoveKeyDownListener(doubleClick.OnClickDown, keyCode);
        }

        public void SetActiveKey(KeyCode keyCode, bool value)
        {
            _keyDic[(int) keyCode].active = value;
        }
        
        public void Clear()
        {
            foreach (var axis in _axisDic.Values)
            {
                if (axis.HasActions())
                {
                    axis.RemoveAllListener();
                    axis.active = false;
                }
            }

            _keyDic.Clear();
        }
    }
}