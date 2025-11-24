#nullable disable

namespace Diagram
{
    /// <summary>
    /// Class for catch log information.
    /// This information can be show in console form </summary>
    public class Log 
    {

        public delegate void logUpdate(string log);

        public logUpdate logUpdateEvent;

        /// <summary>
        /// All messages saved in log </summary>
        private string log = "";

        /// <summary>
        /// Get message from program and save it to log.
        /// If console windows is updated then update window</summary>
        /// <param name="message">Message witch will by saved in log</param>
        public void Write(string message)
        {
            log = log + message + "\n";

            logUpdateEvent?.Invoke(message);

        }

        /// <summary> 
        /// Get all text in log.</summary>
        /// <returns>String width complete log</returns>
        public string GetText()
        {
            return log;
        }
    }
}

