using Code.Internal;
using UnityEngine;

namespace Code
{
    public class InitializeClock : MonoBehaviour
    {
        [Header("Service Selector")]
        [SerializeField] private bool UseNetworkService = true;
        
        [SerializeField] private ClockController _clockController;

        private void Awake()
        {
            if (UseNetworkService)
            {
                _clockController.Initialize(new NetworkTimeService());
                return;
            }
            
            _clockController.Initialize(new LocalTimeService());
        }
    }
}