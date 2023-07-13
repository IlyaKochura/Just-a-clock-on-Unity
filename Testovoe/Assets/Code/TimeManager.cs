using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code
{
    public static class TimeManager
    {
        private static readonly string FirstNtpServer = "ntp0.ntp-servers.net";
        private static readonly string SecondNtpServer = "time.windows.com";

        private static readonly string[] Servers = { FirstNtpServer, SecondNtpServer };
        
        private static CancellationTokenSource _cancellationTokenSource;
        private static DateTime _defTime;
        
        public static async UniTask<DateTime> TryGetTimeAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;
            
            _defTime = new DateTime(1900, 1, 1, 0, 0, 0 , DateTimeKind.Utc);

            foreach (var server in Servers)
            {
                var time = await GetDataTimeFromServerAsync(server, token);
                
                if (time != _defTime)
                {
                    _cancellationTokenSource.Cancel();
                    return time;
                }
            }

            return _defTime;
        }

        private static async UniTask<DateTime> GetDataTimeFromServerAsync(string ntpServer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            
            var ntpData = new byte[48];
            
            ntpData[0] = 0x1B;

            try
            {
                var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            
                var ipEndPoint = new IPEndPoint(addresses[0], 123);
                
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                await socket.ConnectAsync(ipEndPoint);

                socket.ReceiveTimeout = 100;

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return _defTime;
            }

            const byte serverReplyTime = 40;
            
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
            
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
            
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + (fractPart * 1000) / 0x100000000L;
            
            var networkDateTime = _defTime.AddMilliseconds((long)milliseconds);

            Debug.Log($"[Network] Updated TimeData from Server {ntpServer}");
            
            return networkDateTime.ToLocalTime();
        }
        
        static uint SwapEndianness(ulong x)
        {
            return (uint) (((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }
    }
}
