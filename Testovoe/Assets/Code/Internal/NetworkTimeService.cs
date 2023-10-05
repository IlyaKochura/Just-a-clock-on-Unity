using System;
using System.Net;
using System.Net.Sockets;
using Code.Contracts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Internal
{
    public class NetworkTimeService : ITimeService
    {
        private const string NtpServer = "ntp0.ntp-servers.net";
        
        private DateTime _defTime;

        public async UniTask<TimeSpan> GetTimeAsync()
        {
            _defTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            var time = (await GetDataTimeFromServerAsync(NtpServer)).ToTimeSpan();

            var timeSpanDefTime = _defTime.ToTimeSpan();

            if (time == timeSpanDefTime)
            {
                return timeSpanDefTime;
            }

            return time;
        }

        private async UniTask<DateTime> GetDataTimeFromServerAsync(string ntpServer)
        {
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

        private uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                          ((x & 0x0000ff00) << 8) +
                          ((x & 0x00ff0000) >> 8) +
                          ((x & 0xff000000) >> 24));
        }
    }
}