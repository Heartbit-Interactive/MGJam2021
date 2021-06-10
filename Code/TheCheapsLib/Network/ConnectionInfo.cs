using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class ConnectionInfo
    {
        const double HeartbeatTimeS = 5;
        const double TimeoutTimeS = 20;
        public double Heartbeat = HeartbeatTimeS;
        public double Timeout = TimeoutTimeS;
        public int PlayerIndex { get; private set; }
        public ConnectionInfo(int playerIndex)
        {
            this.PlayerIndex = playerIndex;
        }

    }
}
