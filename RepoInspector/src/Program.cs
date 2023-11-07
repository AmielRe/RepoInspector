using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RepoInspector.src.Anomalies;
using Smee.IO.Client;

namespace RepoInspector.src
{
    public class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static CancellationTokenSource source;
        private const string SmeeURLConfigKey = "SmeeURL";

        /// <summary>
        /// Main entry point of the RepoInspector application.
        /// </summary>
        /// <param name="args">Command-line arguments (not used).</param>
        static async Task Main(string[] args)
        {
            var config = new AppConfig();
            List<Type> implementingTypes = GetImplementingAnomalies();

            source = new CancellationTokenSource();
            var token = source.Token;

            var smeeUri = new Uri(config.GetString(SmeeURLConfigKey));

            // Set console text color to green for information messages
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.Info(" > Hit CTRL-C in order to stop everything.");
            Console.ResetColor();

            var smeeClient = new SmeeClient(smeeUri);

            // Set up event handlers for Smee.io interactions
            SetupSmeeEventHandlers(smeeUri, smeeClient, implementingTypes);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                source.Cancel();
                eventArgs.Cancel = true;
            };

            try
            {
                await smeeClient.StartAsync(token);
            }
            catch (Exception ex)
            {
                // Log unhandled exception
                Logger.Error(ex);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Logger.Info("Finish executing. Thank you!");
            Console.ResetColor();
        }

        /// <summary>
        /// Retrieves a list of types that implement the IAnomaly interface within the current assembly.
        /// </summary>
        /// <returns>A list of types representing anomaly detection implementations.</returns>
        private static List<Type> GetImplementingAnomalies()
        {
            // Get all types in the current assembly that implement the IAnomaly interface and are not abstract or interfaces.
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(IAnomaly).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                .ToList();
        }

        /// <summary>
        /// Sets up event handlers for the SmeeClient to handle connection, disconnection, messages, and errors.
        /// </summary>
        /// <param name="smeeUri">The URI of the Smee.io service being connected to.</param>
        /// <param name="smeeClient">The SmeeClient instance for which event handlers are being set up.</param>
        /// <param name="implementingTypes">A list of types implementing the IAnomaly interface for anomaly detection.</param>
        private static void SetupSmeeEventHandlers(Uri smeeUri, SmeeClient smeeClient, List<Type> implementingTypes)
        {
            smeeClient.OnConnect += (sender, a) => Logger.Info($"Connected to Smee.io ({smeeUri.AbsoluteUri})");
            smeeClient.OnDisconnect += (sender, a) => Logger.Info($"Disconnected from Smee.io ({smeeUri.AbsoluteUri})");
            smeeClient.OnMessage += (sender, smeeEvent) =>
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                // Iterate through implementing types to detect anomalies
                foreach (Type implementingType in implementingTypes)
                {
                    try
                    {
                        var instance = (IAnomaly)Activator.CreateInstance(implementingType);
                        instance.Run(smeeEvent);
                    }
                    catch (Exception ex)
                    {
                        // Log exceptions that occur while detecting anomalies
                        Logger.Error(ex);
                    }
                }

                Console.ResetColor();
            };
            smeeClient.OnPing += (sender, a) => Logger.Info("Ping from Smee");
            smeeClient.OnError += (sender, e) => Logger.Error(e);
        }
    }
}
