using Smee.IO.Client.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoInspector.src.Anomalies
{
    /// <summary>
    /// Defines the contract for implementing anomaly detection strategies.
    /// </summary>
    interface IAnomaly
    {
        /// <summary>
        /// Determines whether a given SmeeEvent payload is suspicious and should trigger the action.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to analyze.</param>
        /// <returns>True if the event is suspicious; otherwise, false.</returns>
        public bool IsSuspicious(SmeeEvent payload);

        /// <summary>
        /// Defines the action to take when a suspicious event is detected.
        /// </summary>
        public void Act();

        /// <summary>
        /// Executes the anomaly detection logic on a SmeeEvent payload and triggers the action if needed.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to analyze.</param>
        public void Run(SmeeEvent payload);
    }
}
