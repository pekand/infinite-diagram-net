using System.ComponentModel;

namespace Diagram
{
    public class TimerTimer : System.Windows.Forms.Timer
    {
        public TimerTimer() : base()
        {
        }

        public TimerTimer(IContainer container) : this()
        {
            ArgumentNullException.ThrowIfNull(container);

            container.Add(this);
        }
    }
}
