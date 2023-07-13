using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code
{
    public class ClockController : MonoBehaviour
    {
        [SerializeField] private Clock _clock;
        
        private readonly float Timeout = 3600f;
        private float _delayUpdateFromServer;
        private TimeSpan _time;
        private long _incrementalTime;
        private float _elapsedTime;
        private DateTime _timeFromServer;

        private void Start()
        { 
            GetUpdatedServerTimeAsync().Forget();
            _time = new TimeSpan(0, _timeFromServer.Hour, _timeFromServer.Minute, _timeFromServer.Second);
            _delayUpdateFromServer = Timeout;

            SetClock();
        }

        private void SetClock()
        {
            if (_clock == null) 
            {
                return;
            }
            
            _clock.SetTime(_time);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            UpdateTimeFromServer();
        }
        
        void Update()
        {
            UpdateClockPerHour();
            UpdateTime();
        }

        private void UpdateTime()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= 1f)
            {
                _time += TimeSpan.FromSeconds(1);
                SetClock();
                _elapsedTime = 0f;
            }
        }

        private void UpdateClockPerHour()
        {
            _delayUpdateFromServer -= Time.deltaTime;
            if (_delayUpdateFromServer <= 0)
            {
                GetUpdatedServerTimeAsync().Forget();
                var timeSpan = new TimeSpan(0, _timeFromServer.Hour, _timeFromServer.Minute, _timeFromServer.Second);
                if (_time != timeSpan)
                {
                    Debug.Log($"[Timer] Updated time per hour");
                    _time = timeSpan;
                    SetClock();
                }
                _delayUpdateFromServer = Timeout;
            }
        }

        private void UpdateTimeFromServer()
        {
            GetUpdatedServerTimeAsync().Forget();
            _time = new TimeSpan(0, _timeFromServer.Hour, _timeFromServer.Minute, _timeFromServer.Second);
        }
        
        private async UniTask GetUpdatedServerTimeAsync()
        {
            var time = await TimeManager.TryGetTimeAsync();

            _timeFromServer = time;
        }
    }
}
