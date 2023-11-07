using NLog;
using RepoInspector.src.Utils;
using Smee.IO.Client.Dto;
using System;
using System.Text.RegularExpressions;

namespace RepoInspector.src.Anomalies
{
    /// <summary>
    /// Base class for implementing anomaly detection strategies.
    /// </summary>
    abstract class BaseAnomaly : IAnomaly
    {
        protected Logger Log { get; private set; }
        private const string GithubEventKey = "x-github-event";
        private const string WebhookSecretKey = "x-hub-signature-256";
        private const string secretPrefixRegex = "^sha256=";
        protected AppConfig config;
        private const string webhookSecretKey = "GITHUB_SECRET";
        private readonly string _secret;

        /// <summary>
        /// Initializes a new instance of the BaseAnomaly class.
        /// </summary>
        public BaseAnomaly()
        {
            config = new AppConfig();
            Log = LogManager.GetLogger(GetType().ToString());
            _secret = Environment.GetEnvironmentVariable(webhookSecretKey);
        }

        /// <summary>
        /// Gets the name of the GitHub event type associated with this anomaly.
        /// </summary>
        public abstract string EventName { get; }

        /// <summary>
        /// Gets the name of the anomaly for identification and configuration access.
        /// </summary>
        public abstract string AnomalyName { get; }

        /// <summary>
        /// Defines the action to take when a suspicious event is detected.
        /// </summary>
        public abstract void Act();

        /// <summary>
        /// Determines whether a given SmeeEvent payload is suspicious and should trigger the action.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to analyze.</param>
        /// <returns>True if the event is suspicious; otherwise, false.</returns>
        public abstract bool IsSuspicious(SmeeEvent payload);

        /// <summary>
        /// Executes the anomaly detection logic on a SmeeEvent payload and triggers the action if needed.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to analyze.</param>
        public void Run(SmeeEvent payload)
        {
            Log.Debug($"Running {AnomalyName} anomaly...");

            // Validate the webhook secret for security
            if (!ValidateWebhookSecret(payload))
            {
                Log.Warn("Webhook secret isn't valid!");
                return;
            }

            Log.Debug("Webhook secret verified successfully");

            // Extract and validate the GitHub event type
            if (!payload.Data.Headers.TryGetValue(GithubEventKey, out string payloadEvent)) return;

            if (!string.Equals(payloadEvent, EventName)) return;

            // Check if the JSON payload is valid
            if (!IsJsonPayloadValid(payload)) return;

            // Check if the event is suspicious and take action if needed
            if (IsSuspicious(payload))
            {
                Log.Debug($"{AnomalyName} found suspicious event, handling it now");
                Act();
                Log.Debug($"Finish handling {AnomalyName} suspicious event");
            }
        }

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
        public abstract bool IsJsonPayloadValid(SmeeEvent payload);

        /// <summary>
        /// Validates the webhook secret to ensure the payload's integrity and authenticity.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to be validated.</param>
        /// <returns>
        ///   <c>true</c> if the provided secret in the payload matches the expected secret;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public bool ValidateWebhookSecret(SmeeEvent payload)
        {
            // Try to extract secret hash from the payload's headers.
            if (!payload.Data.Headers.TryGetValue(WebhookSecretKey, out string webhookSecret))
            {
                // Secret not found in headers.
                return false;
            }

            // Remove "sha256=" prefix from the secret hash.
            webhookSecret = Regex.Replace(webhookSecret, secretPrefixRegex, String.Empty);

            // Calculate the HMAC-SHA256 hash of the payload's body using the expected secret.
            byte[] expectedHash = CryptoUtils.ComputeHMACSHA256Hash(payload.Data.Body, _secret);

            // Convert the calculated hash to a hexadecimal string for comparison.
            string expectedHashInString = CryptoUtils.ToHexString(expectedHash);

            // Compare the calculated hash with the provided secret hash.
            return string.Equals(expectedHashInString, webhookSecret, StringComparison.OrdinalIgnoreCase);
        }
    }
}
