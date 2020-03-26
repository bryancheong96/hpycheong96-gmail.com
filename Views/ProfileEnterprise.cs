using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TemPloy.Controller;
using TemPloy.Models;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using System.Text.RegularExpressions;

namespace TemPloy.Views
{
	public class ProfileEnterprise : ContentPage
	{
		readonly Enterprise enterprise;
		readonly JobSeeker jobseeker;
		readonly double rate;
		readonly Entry addressData, contactData, passwordData, nameData, emailData;
		readonly Button saveBtn, reviewBtn, emailData2;
		readonly EnterpriseManager entmanager = new EnterpriseManager();
		readonly IList<EntFeedback> entfeedbacks = new ObservableCollection<EntFeedback>();
		readonly Label[] comment, name;
		readonly Label addressData2, contactData2, usernameLbl, usernameData, passwordLbl, statusLbl, remarkLbl;
		readonly Editor remarkData;
		readonly Picker statusData;
		private Image profilePic;
		StackLayout parent;
		private int count, checkphoto=0;
		readonly string admin;
		private MediaFile mediafile;

		public ProfileEnterprise(Enterprise enterprise, JobSeeker jobseeker, double rate,string admin)
		{
			this.enterprise = enterprise;
			this.jobseeker = jobseeker;
			this.rate = rate;
			this.admin = admin;

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

			Label hearderLbl = new Label
			{
				Text = "Enterprise Profile",
				Font = Font.SystemFontOfSize(25),
				HorizontalOptions = LayoutOptions.Center,
			};

			profilePic = new Image
			{
				HorizontalOptions = LayoutOptions.Center,
				Source = "http://temployapi2017.azurewebsites.net/api/Image/" + enterprise.PhotoName,
				//Source = "http://169.254.30.178:2345/Image/" + enterprise.PhotoName,
				WidthRequest = 200,
				HeightRequest = 200
			};

			Button photoBtn = new Button
			{
				Text = "Choose Photo"
			};
			photoBtn.Clicked += PickPhoto;

			Label nameLbl = new Label
			{
				Text = "Name"
			};
			Label nameData = new Label
			{
				Text = enterprise.CompanyName,
			};

			Label idLbl = new Label
			{
				Text = "Company Registered Number"
			};
			Label idData = new Label
			{
				Text = enterprise.CompanyID
			};

			Label addressLbl = new Label
			{
				Text = "Company Address"
			};
			addressData = new Entry
			{
				Text = enterprise.CompanyAddress
			};
			addressData2 = new Label
			{
				Text = enterprise.CompanyAddress,
			};

			Label contactLbl = new Label
			{
				Text = "Contact Number"
			};
			contactData = new Entry
			{
				Text = enterprise.CompanyContact
			};
			contactData2 = new Label
			{
				Text = enterprise.CompanyContact
			};

			if (jobseeker == null) { 
				usernameLbl = new Label
				{
					Text = "Username"
				};
				usernameData = new Label
				{
					Text = enterprise.Username
				};

				passwordLbl = new Label
				{
					Text = "Password"
				};
				passwordData = new Entry
				{
					Text = enterprise.Password
				};
			}

			Label emailLbl = new Label
			{
				Text = "Email"
			};
			emailData = new Entry
			{
				Text = enterprise.Email
			};
			emailData2 = new Button
			{
				Text = enterprise.Email,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 45,
				TextColor = Color.Blue
			};
			emailData2.FontAttributes = FontAttributes.Bold;
			emailData2.Clicked += OnEmail;


			Label rateLbl = new Label
			{
				Text = "Rating"
			};
			Label rateData = new Label
			{
				Text = double.IsNaN(rate) ? "0" : rate.ToString() + "/5",
			};

			Label commentLbl = new Label
			{
				Text = "Reviews"
			};

			List<string> item = new List<string> { "Active", "Pending", "Banned" };
			statusLbl = new Label
			{
				Text = "Enterprise Status"
			};
			statusData = new Picker
			{
				Title = "Status",
				ItemsSource = item,
				SelectedItem = enterprise.Status,
			};

			remarkLbl = new Label
			{
				Text = "Remark"
			};
			remarkData = new Editor
			{
                Text = enterprise.Remark,
				HeightRequest = 150
			};

			saveBtn = new Button
			{
				Text = "Save",
				BorderRadius = 10,
				VerticalOptions = LayoutOptions.End
			};
			saveBtn.Clicked += updateProfile;

			reviewBtn = new Button
			{
				Text = "View Comment",
				BorderRadius = 10,
				VerticalOptions = LayoutOptions.End
			};
			reviewBtn.Clicked += GetReview;

			parent = new StackLayout();

			//Enterprise View
			if (jobseeker == null && admin == null)
			{
				parent.Children.Add(hearderLbl);
				parent.Children.Add(profilePic);
				parent.Children.Add(photoBtn);
				parent.Children.Add(nameLbl);
				parent.Children.Add(nameData);
				parent.Children.Add(idLbl);
				parent.Children.Add(idData);
				parent.Children.Add(addressLbl);
				parent.Children.Add(addressData);
				parent.Children.Add(contactLbl);
				parent.Children.Add(contactData);
				parent.Children.Add(usernameLbl);
				parent.Children.Add(usernameData);
				parent.Children.Add(passwordLbl);
				parent.Children.Add(passwordData);
				parent.Children.Add(emailLbl);
				parent.Children.Add(emailData);
				parent.Children.Add(rateLbl);
				parent.Children.Add(rateData);
				parent.Children.Add(saveBtn);
				parent.Children.Add(commentLbl);
				parent.Children.Add(reviewBtn);
			}
			else
			{
				parent.Children.Add(hearderLbl);
				parent.Children.Add(profilePic);
				parent.Children.Add(nameLbl);
				parent.Children.Add(nameData);
				parent.Children.Add(idLbl);
				parent.Children.Add(idData);
				parent.Children.Add(addressLbl);
				parent.Children.Add(addressData2);
				parent.Children.Add(contactLbl);
				parent.Children.Add(contactData2);
				parent.Children.Add(emailLbl);
				parent.Children.Add(emailData2);
				parent.Children.Add(rateLbl);
				parent.Children.Add(rateData);
				if (admin == null)
				{
					parent.Children.Add(commentLbl);
					parent.Children.Add(reviewBtn);
				}
				else
				{
					parent.Children.Add(statusLbl);
					parent.Children.Add(statusData);
					parent.Children.Add(remarkLbl);
					parent.Children.Add(remarkData);
					parent.Children.Add(saveBtn);
					parent.Children.Add(commentLbl);
					parent.Children.Add(reviewBtn);
				}
			}


			Content = new ScrollView
			{
				Content = parent
			};
		}

		async void PickPhoto(object sender, EventArgs e)
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
				return;
			}

			mediafile = await CrossMedia.Current.PickPhotoAsync();

			if (mediafile == null)
				return;

			profilePic.Source = ImageSource.FromStream(() =>
			{
				return mediafile.GetStream();
			});
			checkphoto++;
		}

		async void getreview()
		{
			var feedback = await entmanager.GetAllEntFeedback(enterprise.Username);

			foreach (EntFeedback entfeedback in feedback)
			{
				if (entfeedbacks.All(b => b.Id != entfeedback.Id))
				{
					entfeedbacks.Add(entfeedback);
					count++;
				}
			}

			if (count == 0)
			{
				Label noreview = new Label
				{
					Text = "No review available right now",
					BackgroundColor = Color.Red
				};
				parent.Children.Add(noreview);
			}
			else
			{
				Label[] comment = new Label[count];
				Label[] name = new Label[count];

				for (int i = 0; i < count; i++)
				{
					name[i] = new Label
					{
						Text = entfeedbacks[i].SeekerName + " - " + entfeedbacks[i].Rating + "/5"
					};

					comment[i] = new Label
					{
						Text = entfeedbacks[i].Review,
					};
					parent.Children.Add(name[i]);
					parent.Children.Add(comment[i]);
				}
			}
		}

		void GetReview(object sender, EventArgs e)
		{
			parent.Children.Remove(reviewBtn);
			getreview();
		}

		async void updateProfile(object sender, EventArgs e)
		{
			if (jobseeker == null & admin == null)
			{
				string address = addressData.Text.TrimEnd();
				string contact = contactData.Text.TrimEnd();
				string password = passwordData.Text.TrimEnd();
				string email = emailData.Text.TrimEnd();

				Regex whitespaceregex = new Regex(@"\s{1,}");
				Regex namelimit1regex = new Regex("[A-Za-z0-9]{50,}");
				Regex contactregex = new Regex("[A-Za-z]{1,}");
				Regex emailregex = new Regex("[@]{1,}");

				if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(contact) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
				{
					await DisplayAlert("Alert Message", "Please fill up all the details.", "Cancel");
				}
				else
				{
					if(address.Length > 100)
					{
						await DisplayAlert("Alert Message", "Maximum 100 characters is allowed for company's address.", "Cancel");
					}
					else
					{
						if (whitespaceregex.IsMatch(contact) || whitespaceregex.IsMatch(password) || whitespaceregex.IsMatch(email))
						{
							await DisplayAlert("Alert Message", "No space is allowed for Username, Password, Email and Contact.", "Cancel");
						}
						else
						{
							if (contactregex.IsMatch(contact) || contact.Length > 20)
							{
								await DisplayAlert("Alert Message", "Maximum 20 numeric characters is allowed for contact.", "Cancel");
							}
							else
							{
								if(namelimit1regex.IsMatch(password) || password.Length > 50)
								{
									await DisplayAlert("Alert Message", "Maximum 50 alphanumeric characters is allowed for Password.", "Cancel");
								}
								else
								{
									if (!(emailregex.IsMatch(email)))
									{
										await DisplayAlert("Alert Message", "Missing '@' for Email.", "Cancel");
									}
									else
									{
                                        Enterprise user = new Enterprise(enterprise.CompanyName, enterprise.CompanyID, address, contact, enterprise.Username, password, email, enterprise.Status, enterprise.Remark, enterprise.PhotoName);
										var update = await entmanager.UpdateEnterprise(user);
										if (update != null)
										{
											await DisplayAlert("Update Complete", "Profile updated successfully.", "Cancel");
											await Navigation.PopModalAsync();
										}
										else
										{
											await DisplayAlert("Update Failed", "Profile fail to update.", "Cancel");
											await Navigation.PopModalAsync();
										}
									}
								}
							}
						}
					}
				}
			}
			else if (admin != null)
			{
				string userstatus = statusData.Items[statusData.SelectedIndex];

                Enterprise user = new Enterprise(enterprise.CompanyName, enterprise.CompanyID, enterprise.CompanyAddress, enterprise.CompanyContact, enterprise.Username, enterprise.Password, enterprise.Email, userstatus, enterprise.Remark, enterprise.PhotoName);
				var update = await entmanager.UpdateEnterprise(user);
				if (update != null)
				{
					await DisplayAlert("Update Complete", "Profile updated successfully.", "Cancel");
					await Navigation.PopModalAsync();
				}
				else
				{
					await DisplayAlert("Update Failed", "Profile fail to update.", "Cancel");
					await Navigation.PopModalAsync();
				}
			}
		}

		public void OnEmail(object send, EventArgs e)
		{
			var emailTask = CrossMessaging.Current.EmailMessenger;

			if (emailTask.CanSendEmail)
			{
				// Send simple e-mail to single receiver without attachments, CC, or BCC.
				emailTask.SendEmail(enterprise.Email, "Job Title - ", "Hi! " + enterprise.Email + ",");

				/*
				var email = new EmailMessageBuilder()
				  .To("test@hotmail.com")
				  .Cc(enterprise.Email)
				  .Subject("Job Title - ")
				  .Body("Hi! "+enterprise.CompanyName+",")
				  .Build();

				emailTask.SendEmail(email);*/
			}
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}