using System;
using Code.Contracts;
using Cysharp.Threading.Tasks;

namespace Code.Internal
{
    public class LocalTimeService : ITimeService
    {
        public async UniTask<TimeSpan> GetTimeAsync()
        {
            var time = DateTime.Now.ToTimeSpan();

            return time;
        }
    }
}