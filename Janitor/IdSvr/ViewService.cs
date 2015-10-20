using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Validation;
using Xipton.Razor;

namespace Janitor.IdSvr
{
	public class ViewService : IViewService
	{
		private RazorMachine rm = new RazorMachine ();

		private ViewServiceOptions viewOptions;

		public ViewService (ViewServiceOptions viewOptions)
		{
			this.viewOptions = viewOptions;
			rm.RegisterTemplate ("~/Views/Shared/_Layout.cshtml", LoadTemplate (Path.Combine ("~", "Views", "Shared", "_Layout.cshtml")));
			rm.RegisterTemplate ("~/Views/_ViewStart.cshtml", LoadTemplate (Path.Combine ("~", "Views", "_ViewStart.cshtml")));
		}

		public async Task<Stream> Login (LoginViewModel model, SignInMessage message)
		{
			return await Render (model, "Login");
		}

		public async Task<Stream> Logout (LogoutViewModel model, SignOutMessage message)
		{
			return await Render (model, "Logout");
		}

		public async Task<Stream> LoggedOut (LoggedOutViewModel model, SignOutMessage message)
		{
			return await Render (model, "LoggedOut");
		}

		public async Task<Stream> Consent (ConsentViewModel model, ValidatedAuthorizeRequest authorizeRequest)
		{
			return await Render (model, "Consent");
		}

		public async Task<Stream> ClientPermissions (ClientPermissionsViewModel model)
		{
			return await Render (model, "ClientPermissions");
		}

		public async Task<Stream> Error (ErrorViewModel model)
		{
			return await Render (model, "Error");
		}

		protected virtual Task<Stream> Render (CommonViewModel model, string page, string clientName = null)
		{
			var file = string.Format ("~/Content/{0}.cshtml", page);
			rm.RegisterTemplate (file, LoadTemplate (Path.Combine ("~", "Content", string.Format ("{0}.cshtml", page))));
			ITemplate template = rm.ExecuteUrl (file, model);
			return Task.FromResult ((Stream)new MemoryStream (Encoding.UTF8.GetBytes (template.Result)));
		}

		private string LoadTemplate (string rPath)
		{
			var path = rPath.StartsWith ("~") ? rPath.Substring (2, rPath.Length - 2) : rPath;
			var file = string.Format (Path.Combine (this.viewOptions.TemplatePath, path));
			return File.ReadAllText (file, Encoding.UTF8);
		}
	}
}
