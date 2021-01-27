using System;

namespace Diagram
{
    /// <summary>
    /// Class for catch log informations.
    /// This informations can be show in console form </summary>
    public class Log //UID8455348623
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

            logUpdateEvent?.Invoke(log);

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

