using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Code
{
    public class TimeController : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private readonly string UrlTime = "http://worldclockapi.com/api/json/utc/now";

        private static DateTime _defTime;

        private void Start()
        {
            _button.onClick.AddListener(() => GetMoscowTimeAsync().Forget());
        }

        private async UniTask GetMoscowTimeAsync()
        {
            var request = UnityWebRequest.Get(UrlTime);
            request.timeout = 3000;
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                return;
            }
            
            string json = request.downloadHandler.text;
            var gg = json.Substring(30, 16);
            DateTime utcTime = DateTime.ParseExact(gg, "yyyy-MM-ddTHH:mm", null);
            // Moscow Time UTC +3
            var moscowTime = utcTime.AddHours(3);
            Alert(moscowTime.ToString(CultureInfo.InvariantCulture));
        }

        private void Alert(string text)
        {
            string jsCode = "alert('Current time in Moscow: " + text + "');";
            Application.ExternalEval(jsCode);
        }
    }
}