using System;
using Cysharp.Threading.Tasks;

namespace Code.Contracts
{
    public interface ITimeService
    {
        UniTask<TimeSpan> GetTimeAsync();
    }
}