using Plugin.Share;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemPloy.Controller;
using TemPloy.Models;
using TemPloy.Views;
using Xamarin.Forms;

namespace TemPloy
{
	public class LoginPage : ContentPage
	{
		readonly Entry usernameData, passwordData;
		readonly JobSeekerManager jsmanager = new JobSeekerManager();
		readonly EnterpriseManager entmanager = new EnterpriseManager();
		readonly AdminManager adminmanager = new AdminManager();
		string status;

		public LoginPage(string version)
		{
			status = version;

			Label hearderLbl = new Label
			{
				Text = version == "Enterprise" ? "Enterprise Login" : "JobSeeker Login",
                FontSize = 30,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Start,
				TextColor = Color.Wheat,
				Opacity = 0.6
			};

			Label usernameLbl = new Label
			{
				Text = "Name",
				Font = Font.SystemFontOfSize(18),
				TextColor = Color.Black,
				VerticalOptions = LayoutOptions.Start
			};
			usernameData = new Entry
			{
				Text = null,
				TextColor = Color.Black,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				Placeholder = "Username",
				PlaceholderColor = Color.Black,
				BackgroundColor = Color.White,
				Opacity = 0.5,
				VerticalOptions = LayoutOptions.Start
			};

			Label passwordlbl = new Label
			{
				Text = "Password",
				Font = Font.SystemFontOfSize(18),
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.Black,
				VerticalOptions = LayoutOptions.Start
			};
			passwordData = new Entry
			{
				Text = null,
				TextColor = Color.Black,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				IsPassword = true,
				Placeholder = "Password",
				PlaceholderColor = Color.Black,
				BackgroundColor = Color.White,
				Opacity = 0.5,
				VerticalOptions = LayoutOptions.StartAndExpand
			};

			Button Loginbtn = new Button()
			{
				BorderWidth = 2,
				BorderColor = Color.White,
				BackgroundColor = Color.Transparent,
				TextColor = Color.Black,
				Text = "Login",
				BorderRadius = 50, // corner of the button
				HeightRequest = 50,
				Font = Font.SystemFontOfSize(25),
				FontAttributes = FontAttributes.Bold,
				Opacity = 0.7,
				VerticalOptions = LayoutOptions.Start
			};
			Loginbtn.Clicked += OnLogin;

			Button Registerbtn = new Button()
			{
				BorderWidth = 0,
				TextColor = Color.MidnightBlue,
				Text = "Register",
				Font = Font.SystemFontOfSize(18),
				FontAttributes = FontAttributes.Bold,
				HeightRequest = 40,
				VerticalOptions = LayoutOptions.Start,
				BackgroundColor = Color.Transparent
			};
			Registerbtn.Clicked += OnRegister;

			Button Versionbtn = new Button()
			{
				TextColor = Color.MidnightBlue,
				BackgroundColor = Color.Transparent,
				Text = version == "Enterprise" ? "Job Seeker Login" : "Enterprise Login",
				Font = Font.SystemFontOfSize(18),
				FontAttributes = FontAttributes.Bold,
				HeightRequest = 40,
				VerticalOptions = LayoutOptions.Center,
			};
			Versionbtn.Clicked += OnVersion;

			BackgroundImage = "building1.jpeg";

			/*
			Button map = new Button()
			{
				TextColor = Color.MidnightBlue,
				BackgroundColor = Color.Transparent,
				Text = "Map"
			};
			map.Clicked += Map;*/

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					this.Padding = new Thickness(10, 20, 10, 5);
					break;
				default:
					this.Padding = new Thickness(10, 0, 10, 5);
					break;
			}

			Content = new StackLayout
			{
				Spacing = 10, //no idea
				Children = { hearderLbl, Versionbtn, usernameLbl, usernameData, passwordlbl, passwordData, Loginbtn, Registerbtn}
			};
			
		}

		async void OnLogin(object sender, EventArgs e)
		{
			Button button = (Button)sender;
			button.IsEnabled = false;
			this.IsBusy = true;
			try
			{
				string username = usernameData.Text;
				string password = passwordData.Text;
				//bool user = false;

				if (!(string.IsNullOrEmpty(username)) && !(string.IsNullOrEmpty(password)))
				{
					if (status == "Enterprise")
					{
						if (username.Equals("admin_temploy"))
						{
							var adminlogin = await adminmanager.LoginAdmin(username, password);
							if (adminlogin != null)
							{
								await Navigation.PushModalAsync(new NavigationPage(new HomeAdmin()));
							}
							else
							{
								await DisplayAlert("Login Failed", "Wrong Username and/or Password!", "Cancel");
							}

						}
						else
						{
							var entlogin = await entmanager.LoginEnterprise(username, password);
							if (string.IsNullOrEmpty(entlogin.Username))
							{
								await DisplayAlert("Enterprise Login Failed", "Wrong Username and/or Password!", "Cancel");
							}
							else if (entlogin.Status.Equals("Pending"))
							{
								await DisplayAlert("Alert Message", "You account still waiting approval from admin.", "Cancel");
							}
							else if (entlogin.Status.Equals("Banned"))
							{
								await DisplayAlert("Alert Message", "Your account has been banned by admin.", "Cancel");
							}
							else
							{
								await DisplayAlert("Enterprise Login Successful", "Login Successful!", "Cancel");
								await Navigation.PushModalAsync(new NavigationPage(new HomeEnterprise(entlogin)));
							}
						}
					}
					else
					{
						var jslogin = await jsmanager.LoginJobSeeker(username, password);
						if (string.IsNullOrEmpty(jslogin.Username))
						{
							await DisplayAlert("JS Login Failed", "Wrong Username and/or Password!" + jslogin.Username, "Cancel");
						}
						else if (jslogin.Status.Equals("Banned"))
						{
							await DisplayAlert("Alert Message", "Your account has been banned by admin.", "Cancel");
						}
						else
						{
							string entusername = null, address = null;
							await DisplayAlert("JS Login Successful", "Login Successful!", "Cancel");
							await Navigation.PushModalAsync(new NavigationPage(new HomeJobSeeker(jslogin, entusername, address)));
						}
						
					}
				}
				else
				{
					await DisplayAlert("Login Failed", "Please enter Username and Password.", "Re-Login");
				}

			}
			finally
			{
				this.IsBusy = false;
				button.IsEnabled = true;
			}
		}

		async void OnRegister(object sender, EventArgs e)
		{
			if (status == "Enterprise")
			{
				await Navigation.PushModalAsync(new RegisterEnterprise());
			}
			else
			{
                await Navigation.PushModalAsync(new NavigationPage((new RegisterJobSeeker())));
			}
		}

		async void OnVersion(object send, EventArgs e)
		{
			string change;
			change = status == "Enterprise" ? "Jobseeker" : "Enterprise";
			//await Navigation.PushModalAsync(new LoginPage(change));
			if (status.Equals("Jobseeker"))
			{
				await Navigation.PushModalAsync(new LoginPage(change));
			}
			else
			{
				await Navigation.PopModalAsync();
			}
		}
	}
}