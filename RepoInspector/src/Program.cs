using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using RepoInspector.src.Anomalies;
using Smee.IO.Client;

namespace RepoInspector.src
{
    class Program
    {
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

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(" > Hit CTRL-C in order to stop everything.");
            Console.WriteLine();
            Console.ResetColor();

            var smeeClient = new SmeeClient(smeeUri);
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
                Console.WriteLine($"Unhandled exception: {ex.Message}");
                // Log or handle the error as needed.
            }

            Console.WriteLine("Finish executing. Thank you!");
        }

        private static List<Type> GetImplementingAnomalies()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(IAnomaly).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                .ToList();
        }

        private static void SetupSmeeEventHandlers(Uri smeeUri, SmeeClient smeeClient, List<Type> implementingTypes)
        {
            smeeClient.OnConnect += (sender, a) => Console.WriteLine($"Connected to Smee.io ({smeeUri.AbsoluteUri}){Environment.NewLine}");
            smeeClient.OnDisconnect += (sender, a) => Console.WriteLine($"Disconnected from Smee.io ({smeeUri.AbsoluteUri}){Environment.NewLine}");
            smeeClient.OnMessage += (sender, smeeEvent) =>
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                foreach (Type implementingType in implementingTypes)
                {
                    try
                    {
                        var instance = (IAnomaly)Activator.CreateInstance(implementingType);
                        instance.Run(smeeEvent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating instance of {implementingType.Name}: {ex.Message}");
                        // Log or handle the error as needed.
                    }
                }

                Console.ResetColor();
                Console.WriteLine();
            };
            smeeClient.OnPing += (sender, a) => Console.WriteLine($"Ping from Smee{Environment.NewLine}");
            smeeClient.OnError += (sender, e) => Console.WriteLine($"Error was raised ({e.GetType()}: {e.Message}{Environment.NewLine}");
        }
    }
}
