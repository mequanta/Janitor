using IdentityServer3.Core.Logging;
using Microsoft.Owin.Hosting;
using Serilog;
using System;
using System.Threading;

namespace Janitor.SelfHost
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.Title = "IdentityServer3 SelfHost";

			Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
				.WriteTo
				.LiterateConsole(outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}")
				.CreateLogger();

			const string url = "http://+:44002/";
			using (WebApp.Start<Startup>(url))
			{
				Console.WriteLine("\n\nServer listening at {0}. Press enter to stop", url);
				Console.ReadLine();
			}
		}
	}
}