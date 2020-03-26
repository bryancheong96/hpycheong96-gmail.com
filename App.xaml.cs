using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemPloy.Views;
using Xamarin.Forms;

namespace TemPloy
{
	public partial class App : Application
	{
		string version = "Jobseeker";
		public App()
		{
			InitializeComponent();

			MainPage = new LoginPage(version);
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
