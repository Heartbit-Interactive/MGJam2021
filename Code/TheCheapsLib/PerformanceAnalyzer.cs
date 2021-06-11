using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheCheapsLib
{
    public class PerformanceAnalyzer
    {
        public static void PrepMessage(NetOutgoingMessage msg)
        {
            msg.Write(DateTime.Now.Ticks);
        }
        public static void OnMessageReceived(NetIncomingMessage msg, bool server2client)
        {
            var sent_ticks = msg.ReadInt64();
            var sent = new DateTime(sent_ticks);
            var received = msg.ReceiveTime;
            var now = DateTime.Now;
            var process = (NetTime.Now - received) * 1000;
            if (server2client)
            {
                s2c_total_delay.Add((now - sent).TotalMilliseconds - process);
                s2c_process_delay.Add(process);
                s2c_count++;
                if (s2c_count >= 120)
                {
                    System.Diagnostics.Debug.WriteLine($"+S2C Delivery time: {s2c_total_delay.Average():0.0} ms, process delay: {s2c_process_delay.Average():0.0} ms");
                    s2c_count = 0;
                    s2c_total_delay.Clear();
                    s2c_process_delay.Clear();
                }
            }
            else
            {
                c2s_total_delay.Add((now - sent).TotalMilliseconds - process);
                c2s_process_delay.Add(process);
                c2s_count++;
                if (c2s_count >= 120)
                {
                    System.Diagnostics.Debug.WriteLine($"+C2S Delivery time: {c2s_total_delay.Average():0.0} ms, process delay: {c2s_process_delay.Average():0.0} ms");
                    c2s_count = 0;
                    c2s_total_delay.Clear();
                    c2s_process_delay.Clear();
                }
            }
        }
        static List<double> s2c_total_delay = new List<double>();
        static List<double> s2c_process_delay = new List<double>();
        private static int s2c_count;
        static List<double> c2s_total_delay = new List<double>();
        static List<double> c2s_process_delay = new List<double>();
        private static int c2s_count;
    }
}