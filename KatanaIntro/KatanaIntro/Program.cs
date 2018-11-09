using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.Hosting;
using System.IO;
using System.Web.Http;

namespace KatanaIntro
{
	using AppFunc = Func<IDictionary<string, object>, Task>;

	public class Program
	{
		//static void Main(string[] args)
		//{
		//	string uri = "http://localhost:8080";

		//	using (WebApp.Start<Startup>(uri))
		//	{
		//		Console.WriteLine("Started succesfull...");
		//		Console.ReadKey();
		//		Console.WriteLine("Stopped...");
		//	}
		//}

		public class Startup
		{
			public void Configuration(IAppBuilder app)
			{
				app.Use(async (enviroment, next) =>
				{
					foreach (var item in enviroment.Environment)
					{
						Console.WriteLine("{0}:{1}", item.Key, item.Value);
					}

					await next();
				});

				app.Use(async (enviroment, next) =>
				{
					Console.WriteLine("Requesting: ", enviroment.Request.Path);
					await next();
					Console.WriteLine("Response: ", enviroment.Response.StatusCode);
				});

				ConfigureWebApi(app);

				app.HelloWorld();
			}

			private void ConfigureWebApi(IAppBuilder app)
			{
				var config = new HttpConfiguration();

				config.Routes.MapHttpRoute("DefaultApi",
					"api/{controller}/{id}", 
					new { id = RouteParameter.Optional});

				app.UseWebApi(config);
			}
		}
	}

	public static class AppBuilderExtenions
	{
		public static void HelloWorld(this IAppBuilder app)
		{
			app.Use<HelloComponent>();
		}
	}

	public class HelloComponent
	{
		AppFunc _next;
		public HelloComponent(AppFunc next)
		{
			_next = next;
		}

		public Task Invoke(IDictionary<string, object> enviroment)
		{
			//await _next(enviroment);
			var responsive = enviroment["owin.ResponseBody"] as Stream;

			using (var writer = new StreamWriter(responsive))
			{
				return writer.WriteAsync("Hello Owin!!!!");
			}
		}
	}
}
