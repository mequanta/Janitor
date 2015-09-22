using System;
using System.IO;
using System.Reflection;
using Owin;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Janitor;

namespace Janitor.SelfHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var path =
                Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..",
                    "..", "..", "src", "Janitor", "Content"));
            Console.WriteLine(path);
            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = new PathString("/Content"),
                FileSystem = new PhysicalFileSystem(path)
            });

			global::Janitor.Startup.Configure (app);
        }
    }
}
