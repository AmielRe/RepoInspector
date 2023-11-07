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
        /// Validates the JSON payload of a SmeeEvent for correctness and required fields.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to validate.</param>
        /// <returns>
        ///   <c>true</c> if the JSON payload is valid; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///   This method performs checks to ensure the provided JSON payload is in the expected format
        ///   and contains the necessary fields. If the payload is valid, it returns <c>true</c>;
        ///   otherwise, it returns <c>false</c>.
        /// </remarks>
        public bool IsJsonPayloadValid(SmeeEvent payload);

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
