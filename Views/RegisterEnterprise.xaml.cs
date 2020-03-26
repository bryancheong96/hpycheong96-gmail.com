using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
	public partial class RegisterEnterprise : ContentPage
	{
		readonly EnterpriseManager entmanager = new EnterpriseManager();
		readonly IList<Enterprise> ents = new ObservableCollection<Enterprise>();
		private MediaFile mediafile;

		public RegisterEnterprise()
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

			pickphoto.Clicked += async (sender, args) =>
			{
				if (!CrossMedia.Current.IsPickPhotoSupported)
				{
					await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
					return;
				}

				mediafile = await CrossMedia.Current.PickPhotoAsync();

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

		async void SubmitRegisterEnterprise(object sender, EventArgs e)
		{
			string Name = companyname.Text;
			string Id = companyid.Text;
			string Address = companyaddress.Text;
			string Contact = companycontact.Text;
			string Username = username.Text;
			string Password = password.Text;
			string Email = email.Text;
			string version = "Enterprise";
			string Status = "Pending";

			Regex whitespaceregex = new Regex(@"\s{1,}");
			Regex namelimit1regex = new Regex("[A-Za-z0-9]{50,}");
			Regex namelimit2regex = new Regex("[0-9]{1,}");
			Regex contactregex = new Regex("[A-Za-z]{1,}");
			Regex emaileregex = new Regex("[@]{1,}");

			if (!(string.IsNullOrEmpty(Name)) && !(string.IsNullOrEmpty(Id)) && !(string.IsNullOrEmpty(Address)) && !(string.IsNullOrEmpty(Contact))
				&& !(string.IsNullOrEmpty(Username)) && !(string.IsNullOrEmpty(Password)) && !(string.IsNullOrEmpty(Email)))
			{
				if (Name.Length > 100)
				{
					await DisplayAlert("Alert Message", "Maximum 100 characters is allowed for company's name.", "Cancel");
				}
				else if (Address.Length > 100)
				{
					await DisplayAlert("Alert Message", "Maximum 100 characters is allowed for company's address.", "Cancel");
				}
				else if (contactregex.IsMatch(Contact) || Contact.Length > 20)
				{
					await DisplayAlert("Alert Message", "Maximum 20 numeric characters is allowed for contact.", "Cancel");
				}
				else if (Username.Length > 50 || Password.Length > 50 || Email.Length > 50)
				{
					await DisplayAlert("Alert Message", "Maximum 50 characters is allowed for Username, Password and Email.", "Cancel");
				}
				else if (namelimit1regex.IsMatch(Username) || namelimit1regex.IsMatch(Password) || namelimit1regex.IsMatch(Email))
				{
					await DisplayAlert("Alert Message", "Maximum 50 alphanumeric characters is allowed for Username, Password and Email.", "Cancel");
				}
				else if (whitespaceregex.IsMatch(Username) || whitespaceregex.IsMatch(Password) || whitespaceregex.IsMatch(Email) || whitespaceregex.IsMatch(Contact))
				{
					await DisplayAlert("Alert Message", "No space is allowed for Username, Password, Email and Contact.", "Cancel");
				}
				else if (!(emaileregex.IsMatch(Email)))
				{
					await DisplayAlert("Alert Message", "Missing '@' for Email.", "Cancel");
				}
				else
				{
					var checkid = await entmanager.CheckId(Id);
					int count = 0;

					foreach(Enterprise ent in checkid)
					{
						if(ents.All(b => b.Username != ent.Username))
						{
							if(ent.CompanyID == Id)
							{
								ents.Add(ent);
								count++;
							}
						}
					}

					if (count == 0)
					{
						var checkusername = await entmanager.CheckUsername(Username);

						if (checkusername == null)
						{
							try
							{
								var photo = await entmanager.SendPhoto(mediafile);

								if (photo != null)
								{
									string Photoname = photo.Trim('"');

									var register = await entmanager.RegisterEnterprise(Name, Id, Address, Contact, Username, Password, Email, Status, Photoname);
									if (register != null)
									{
										await DisplayAlert("Enterprise Registration", "Register Completed!", "Cancel");
										await Navigation.PopModalAsync();
									}
									else
									{
										await DisplayAlert("Enterprise Registration", "Register Failed!", "Cancel");
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
					else
					{
						await DisplayAlert("Alert Message", "Company's ID has already registered.", "Cancel");
					}
				}
			}
			else
			{
				await DisplayAlert("Alert Message", "Please fill up all the details.", "Cancel");
			}
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}