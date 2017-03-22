using System;
using System.Web.Http;

namespace TestService
{
	/// <summary>Global.asax file</summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>Регистрация</summary>
		/// <param name="config">Конфиг</param>
		public static void Register(HttpConfiguration config)
		{
			// Web API routes
			config.MapHttpAttributeRoutes();

			GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
		}

		/// <summary>Application_Start event handler</summary>
		/// <param name="sender">banana banana banana</param>
		/// <param name="e">banana banana banana</param>
		protected void Application_Start(Object sender, EventArgs e)
		{
			GlobalConfiguration.Configure(Global.Register);
		}

	}
}