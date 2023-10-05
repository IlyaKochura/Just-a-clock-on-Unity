using System;
using Code.Contracts;
using Cysharp.Threading.Tasks;

namespace Code.Internal
{
    public class DummyTimeService : ITimeService
    {
        public async UniTask<TimeSpan> GetTimeAsync()
        {
            return new TimeSpan();
        }
    }
}