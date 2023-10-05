using System;
using Code.Contracts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code
{
    public class ClockController : MonoBehaviour
    {
        [SerializeField] private Clock _clock;

        private ITimeService _timeService;

        private readonly float Timeout = 3600f;
        private float _delayUpdateFromServer;
        private TimeSpan _time;
        private float _elapsedTime;

        public void Initialize(ITimeService timeService)
        {
            _timeService = timeService;
        }

        private void Start()
        {
            GetUpdatedServerTimeAsync().Forget();
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
            GetUpdatedServerTimeAsync().Forget();
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
                
                Debug.Log($"[Timer] Updated time per hour");
                SetClock();

                _delayUpdateFromServer = Timeout;
            }
        }

        private async UniTask GetUpdatedServerTimeAsync()
        {
            var time = await _timeService.GetTimeAsync();

            _time = time;
        }
    }
}