using GameTools;
using UnityEngine;

namespace Example.Timer
{
    public class TimerExample : MonoBehaviour
    {
        public GameObject cube;
        public GameObject sphere;
        public GameObject capsule;

        private int _cubeTimerId;
        private int _sphereTimerId;
        private int _capsuleTimerId;

        private bool _sphereTimerPause = false;
        private bool _sphereTimerRunning = false;
        private void Awake()
        {
            TimerManager.instance.Init();
        }

        private void Start()
        {
            //执行一次
            _cubeTimerId = TimerManager.instance.AddTimerByMilliseconds(1000,
                () =>
                {
                    cube.transform.rotation = cube.transform.rotation * Quaternion.AngleAxis(30f, Vector3.forward);
                }, false);
            //执行无限次,不自动移除
            _sphereTimerId = TimerManager.instance.AddTimerByMilliseconds(50,
                () =>
                {
                    sphere.transform.position = sphere.transform.position + Vector3.right * Random.Range(-1f, 1f);
                }, null, 0, false);
            //执行n次,自动移除
            int tempCount = 0;
            _capsuleTimerId = TimerManager.instance.AddTimerBySeconds(0.1f,
                () =>
                {
                    capsule.transform.position = capsule.transform.position + Vector3.forward * Random.Range(-1f, 1f);
                    tempCount++;
                    Debug.Log($"capsuleTimer runing count: {tempCount}");
                }, null, 10);


            TimerManager.instance.Start(_cubeTimerId);
            TimerManager.instance.Start(_sphereTimerId);
            TimerManager.instance.Start(_capsuleTimerId);
        }


        private void Update()
        {
            TimerManager.instance.Tick();

            if (Input.GetKeyDown(KeyCode.P))
            {
                _sphereTimerPause = !_sphereTimerPause;
                TimerManager.instance.Pause(_sphereTimerId, _sphereTimerPause);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                _sphereTimerRunning = !_sphereTimerRunning;
                
                if(_sphereTimerRunning) 
                    TimerManager.instance.Stop(_sphereTimerId);
                else
                    TimerManager.instance.Restart(_sphereTimerId);
            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TimerManager.instance.Restart(_cubeTimerId);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TimerManager.instance.Restart(_capsuleTimerId);
                Debug.Log("capsult timer autoDestroy == true");
            }

        }

        private void OnDestroy()
        {
            TimerManager.instance.Clear();
        }
    }
}