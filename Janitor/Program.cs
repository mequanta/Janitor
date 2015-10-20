using System;
using Serilog;
using Microsoft.Owin.Hosting;
using Janitor;
using System.IO;
using System.Threading;


namespace Janitor
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var evt = new ManualResetEvent (false);

            var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."));
            Console.WriteLine(basePath);
            Console.Title = "IdentityServer3 SelfHost";

            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .LiterateConsole(outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();
            var port = ((args != null && args.Length > 1) ? args [0] : null) ?? Environment.GetEnvironmentVariable ("PORT") ?? "44002";
            var url = string.Format ("http://+:{0}/", port);
            var startOptions = new StartOptions();
            startOptions.Urls.Add(url);
            using (WebApp.Start(url, (appBuilder) =>
            {
                new Startup(basePath).Configuration(appBuilder);
            }))
            {
                Console.WriteLine("\n\nServer listening at {0}. Press ctrl+c to stop", url);
                Console.CancelKeyPress += (sender, e) => evt.Set ();
                evt.WaitOne ();
                Console.WriteLine ("Exited");
            }
        }
    }
}
