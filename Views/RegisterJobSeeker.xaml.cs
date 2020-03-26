using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemPloy.Controller;
using TemPloy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TemPloy.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterJobSeeker : ContentPage
	{
		readonly JobSeekerManager manager = new JobSeekerManager();
		private MediaFile mediafile;

		public RegisterJobSeeker()
		{
			InitializeComponent();

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					var a = new ToolbarItem
					{
						Text = "back",

					};
					a.Clicked += BackPage;

					ToolbarItems.Add(a);
					break;
				default:
					this.Padding = new Thickness(10, 0, 10, 5);
					break;
			}

			// Accomodate iPhone status bar.
			//this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			pickphoto.Clicked += async (sender, args) =>
			{
				if (!CrossMedia.Current.IsPickPhotoSupported)
				{
					await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
					return;
				}

				mediafile = await CrossMedia.Current.PickPhotoAsync();
				/*
				mediafile = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
				{
					PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
				});
				*/

				if (mediafile == null)
					return;

				image.Source = ImageSource.FromStream(() =>
				{
					//var stream = mediafile.GetStream();
					//file.Dispose();

					return mediafile.GetStream();
				});
			};
		}

		async void SubmitRegisterJobSeeker(object sender, EventArgs e)
		{
			string Name = name.Text.TrimEnd();
			string Username = username.Text.TrimEnd();
			string Pass = password.Text.TrimEnd();
			string Email = email.Text.TrimEnd();
			string version, Status = "Active";
			Regex whitespaceregex = new Regex(@"\s{1,}");
			Regex namelimit1regex = new Regex("[A-Za-z0-9]{50,}");
			Regex namelimit2regex = new Regex("[0-9]{1,}");
			Regex emaileregex = new Regex("[@]{1,}");
		
			if (!(string.IsNullOrEmpty(Name)) && !(string.IsNullOrEmpty(Username)) && !(string.IsNullOrEmpty(Pass)) && !(string.IsNullOrEmpty(Email)))
			{
				if (namelimit1regex.IsMatch(username.Text) || namelimit1regex.IsMatch(password.Text) || namelimit1regex.IsMatch(email.Text))
				{
					await DisplayAlert("Alert Message", "Maximum 50 characters allowed for Name, Username, Password and Email.", "Cancel");
				}
				else
				{
					if(whitespaceregex.IsMatch(username.Text) || whitespaceregex.IsMatch(password.Text) || whitespaceregex.IsMatch(email.Text))
					{
						await DisplayAlert("Alert Message", "No space is allowed for Username, Password and Email.", "Cancel");
					}
					else
					{
						if (namelimit2regex.IsMatch(Name) || Name.Length > 50)
						{
							await DisplayAlert("Alert Message", "No numeric character allowed for name.", "Cancel");
						}
						else
						{
							if (!(emaileregex.IsMatch(email.Text)))
							{
								await DisplayAlert("Alert Message", "Missing '@' for Email.", "Cancel");
							}
							else
							{
								var checkusername = await manager.CheckUsername(Username);

								if (checkusername == null)
								{
									try
									{
										var photo = await manager.SendPhoto(mediafile);
										await DisplayAlert("Job Seeker Registration", photo, "Cancel");

										if (photo != null)
										{
											string Photoname = photo.Trim('"');
											await DisplayAlert("Job Seeker Registration", Photoname, "Cancel");

											var register = await manager.RegisterJobSeeker(Name, Username, Pass, Email, Status, Photoname);
											if (register != null)
											{
												await DisplayAlert("Job Seeker Registration", "Register Completed!", "Cancel");
												await Navigation.PopModalAsync();
											}
											else
											{
												await DisplayAlert("Error Message", "Register Failed!", "Cancel");
											}
										}
										else
										{
											await DisplayAlert("Alert Message", "Something goes wrong.", "OK");
										}
									}
									catch (Exception ex)
									{
										await DisplayAlert("Alert Message", "Please select profile picture.", "OK");
									}
								}
								else
								{
									await DisplayAlert("Alert Message", "Username has been used. Please choose another username.", "OK");
								}
							}
						}
					}
				}
			}
			else
			{
				await DisplayAlert("Alert Message", "Please fill in all the details.", "Cancel");
			}
		}

		/*
		async void SendPhoto(object sender, EventArgs e)
		{
			
			var content = new MultipartFormDataContent();
			content.Add(new StreamContent(mediafile.GetStream()),"\"file\"",$"\"{mediafile.Path}\"");
			using (var httpclient = new HttpClient())
			{
				var uploadservice = "http://169.254.30.178:2345/api/imageupload";
				var httpResponseMessage = await httpclient.PostAsync(uploadservice, content);
				string text = await httpResponseMessage.Content.ReadAsStringAsync();
				Name.Text = text;
			}
		}*/

        async void BackPage(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
	}
}