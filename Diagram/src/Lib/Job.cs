using System;
using System.ComponentModel;

namespace Diagram
{
    /// <summary>
    /// repository for thread related functions</summary>
    public class Job //UID8736422044
    {
        /// <summary>
        /// run task in thread </summary>
        /// <example> 
        /// This of use doJob method
        /// <code>
        /// Job.DoJob(
        ///    new DoWorkEventHandler(
        ///        delegate (object o, DoWorkEventArgs args)
        ///        {
        ///            // run in new thread
        ///        }
        ///    ),
        ///    new RunWorkerCompletedEventHandler(
        ///        delegate (object o, RunWorkerCompletedEventArgs args)
        ///        {
        ///            // complete
        ///        }
        ///    )
        /// );
        /// </code>
        /// </example>
        public static void DoJob(DoWorkEventHandler doJob = null, RunWorkerCompletedEventHandler afterJob = null)
        {
            try
            {
                BackgroundWorker bw = new BackgroundWorker {
                    WorkerSupportsCancellation = true
                };
                bw.WorkerReportsProgress = true;
                bw.DoWork += doJob;
                bw.RunWorkerCompleted += afterJob;
                bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Program.log.Write("get link name error: " + ex.Message);
            }
        }
    }
}
