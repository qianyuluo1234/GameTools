using System.Collections;
using System.Collections.Generic;
using GameTools;
using UnityEngine;

namespace Example.InputController
{
    public class GameInputControllerExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //注册horizontal轴事件
            GameInputController.instance.AddAxisListener((v) => { Debug.Log($"h: {v}"); }, EGameInputAxis.X);
            //注册按键z事件1， 日志
            GameInputController.instance.AddKeyDownListener(() => { Debug.Log($"key down z"); }, KeyCode.Z);
            //注册按键z事件2  禁用Horizontal轴
            GameInputController.instance.AddKeyDownListener(
                () => { GameInputController.instance.SetActiveAxis(EGameInputAxis.X, false); }, KeyCode.Z);
            //注册按键x事件1 日志
            GameInputController.instance.AddKeyUpListener(() => { Debug.Log($"key up x"); }, KeyCode.X);
            //注册按键x事件2 启用Horizontal轴
            GameInputController.instance.AddKeyUpListener(
                () => { GameInputController.instance.SetActiveAxis(EGameInputAxis.X, true); }, KeyCode.X);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}