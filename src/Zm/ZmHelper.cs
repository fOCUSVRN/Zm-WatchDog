using Newtonsoft.Json;
using NLog;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ZmWatchDog.Zm
{
    public static class ZmHelper
    {
        public static ZmResponse GetStat(string ip, int port)
        {
            using (var soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                soc.ReceiveTimeout = 15000;

                var ipAdd = IPAddress.Parse(ip);
                var remoteEp = new IPEndPoint(ipAdd, port);
                soc.Connect(remoteEp);

                var byData = Encoding.ASCII.GetBytes(@"{""id"":1, ""method"":""getstat""}");
                soc.Send(byData);



                var buffer = new byte[2048];
                var iRx = soc.Receive(buffer);
                var chars = new char[iRx];

                var d = Encoding.UTF8.GetDecoder();
                var charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                var recv = new String(chars);

                LogManager.GetCurrentClassLogger().Info($"GetStat: {recv}");

                return JsonConvert.DeserializeObject<ZmResponse>(recv);
            }
        }
    }
}
