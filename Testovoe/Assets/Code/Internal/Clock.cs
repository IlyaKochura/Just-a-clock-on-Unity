using System;
using TMPro;
using UnityEngine;

namespace Code
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private RectTransform _hourHand;
        [SerializeField] private RectTransform _minuteHand;
        [SerializeField] private RectTransform _secondHand;
        [SerializeField] private TMP_Text _textTime; 
        
        private const float HoursToDegrees = 360f/12f;
        private const float MinuteAndSecondsToDegrees = 360f/60f;

        public void SetTime(TimeSpan time)
        {
            _hourHand.localRotation = Quaternion.Euler(0f, 0f,  (float)time.TotalHours * -HoursToDegrees);
            _minuteHand.localRotation = Quaternion.Euler(0f, 0f, (float)time.TotalMinutes * -MinuteAndSecondsToDegrees);
            _secondHand.localRotation = Quaternion.Euler(0f, 0f, (float)time.TotalSeconds * -MinuteAndSecondsToDegrees);

            var textTime = $"{time.Hours}:{time.Minutes}:{time.Seconds}";
            _textTime.SetText(textTime);
        }
    }
}
