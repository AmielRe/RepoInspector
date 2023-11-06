using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RepoInspector.src.Anomalies;
using Smee.IO.Client;

namespace RepoInspector.src
{
    class Program
    {
        private static CancellationTokenSource source;

        static async Task Main(string[] args)
        {
            // Get all types that implement the IAnomaly interface in the current assembly.
            List<Type> implementingTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(IAnomaly).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                .ToList();

            source = new CancellationTokenSource();
            var token = source.Token;

            // Static for now, we will later make it configurable
            var smeeUri = new Uri("https://smee.io/OTddXLye7f5D5vh");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(" > Hit CTRL-C in order to stop everything.");
            Console.WriteLine();
            Console.ResetColor();

            var smeeCli = new SmeeClient(smeeUri);
            smeeCli.OnConnect += (sender, a) => Console.WriteLine($"Connected to Smee.io ({smeeUri}){Environment.NewLine}");
            smeeCli.OnDisconnect += (sender, a) => Console.WriteLine($"Disconnected from Smee.io ({smeeUri}){Environment.NewLine}");
            smeeCli.OnMessage += (sender, smeeEvent) =>
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                // Create instances of the implementing types and call the BaseFunction method.
                foreach (Type implementingType in implementingTypes)
                {
                    var instance = (IAnomaly)Activator.CreateInstance(implementingType);
                    instance.Run(smeeEvent);
                }

                Console.ResetColor();
                Console.WriteLine();
            };
            smeeCli.OnPing += (sender, a) => Console.WriteLine($"Ping from Smee{Environment.NewLine}");
            smeeCli.OnError += (sender, e) => Console.WriteLine($"Error was raised (Disconnect/Anything else: {e.Message}{Environment.NewLine}");

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                source.Cancel();
                eventArgs.Cancel = true;
            };

            await smeeCli.StartAsync(token);
            Console.WriteLine("Finish executing. Thank you!");
        }
    }
}
