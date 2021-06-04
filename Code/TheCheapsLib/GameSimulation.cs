using System;

namespace TheCheaps
{
    public class GameSimulation
    {
        public int X;
        public int Y;

        public void Start()
        {
        }

        public void Step()
        {
            var now = DateTime.Now;
            this.X = (int)(320+320 * Math.Cos(now.Second+now.Millisecond/1000f));
            this.Y = (int)(240 + 240 * Math.Sin(now.Second + now.Millisecond / 1000f));

        }
    }
}