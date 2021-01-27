using System;
using System.Windows.Forms;

namespace Diagram
{
    /// <summary>
    /// Time and timers functions repository
    /// </summary>
    public class Tick //UID9091710163
    {
        /// <summary>
        /// Create timer 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="tick"></param>
        /// <example>
        /// Create timer
        /// <code>
        /// Tick.timer(500, (t, args) =>
        /// {
        ///     if (t is Timer)
        ///     {
        ///         Timer timer = t as Timer;
        ///         t.Stop()
        ///     }
        /// });
        /// </code>
        /// </example>
        /// <returns></returns>
        public static Timer Timer(int interval, EventHandler tick)
        {
            Timer timer = new Timer
            {
                Interval = interval
            };
            timer.Tick += tick;
            timer.Enabled = true;
            return timer;
        }
    }
}
