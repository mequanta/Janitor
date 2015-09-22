using IdentityServer3.Core.Logging;
using Microsoft.Owin.Hosting;
using Serilog;
using System;
using System.Threading;

namespace Janitor.SelfHost
{
	internal class Program
	{
		private static void Main (string[] args)
		{
			ManualResetEvent evt = new ManualResetEvent (false);
			Console.Title = "IdentityServer3 SelfHost";

			Log.Logger = new LoggerConfiguration ().MinimumLevel.Debug ()
				.WriteTo
				.LiterateConsole (outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
				.CreateLogger ();
			var port = ((args != null && args.Length > 1) ? args [0] : null) ?? Environment.GetEnvironmentVariable ("PORT") ?? "44002";
			var url = string.Format ("http://+:{0}/", port);
			using (WebApp.Start<Startup> (url)) {
				Console.WriteLine ("\n\nServer listening at {0}. Press ctrl+c to stop", url);
				Console.CancelKeyPress += (sender, e) => evt.Set ();
				evt.WaitOne ();
				Console.WriteLine ("Exited");
			}
		}
	}
}