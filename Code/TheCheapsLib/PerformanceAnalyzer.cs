using Lidgren.Network;
using System;

namespace TheCheapsLib
{
    public class PerformanceAnalyzer
    {
        public static void PrepMessageFromServer(NetOutgoingMessage msg)
        {
            msg.Write(DateTime.Now.Ticks);
        }
        public static void AddMessageFromServer(NetIncomingMessage msg)
        {
            var sent_ticks = msg.ReadInt64();
            var sent = new DateTime(sent_ticks);
            var received = msg.ReceiveTime;
            var now = DateTime.Now;

            System.Diagnostics.Debug.WriteLine($"{now.ToShortTimeString()} Server message sent {(now - sent).TotalMilliseconds:0.0} ms ago, received {(NetTime.Now - received)*1000:0.0} ms ago");
        }
    }
}