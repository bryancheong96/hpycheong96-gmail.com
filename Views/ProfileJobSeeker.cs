using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TemPloy.Controller;
using TemPloy.Models;
using Xamarin.Forms;
using Plugin.Messaging;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Text.RegularExpressions;

namespace TemPloy.Views
{
	public class ProfileJobSeeker : ContentPage
	{
		readonly JobSeeker jobseeker;
		readonly Enterprise enterprise;
		readonly Label rateData, usernameLbl,passwordLbl, nameData2, usernameData, statusLbl, remarkLbl;
		readonly Entry passwordData, nameData, emailData;
		readonly Editor remarkData;
		readonly Button saveBtn,reviewBtn, emailData2;
		readonly JobSeekerManager jsmanager = new JobSeekerManager();
		readonly IList<JSFeedback> jsfeedbacks = new ObservableCollection<JSFeedback>();
		private int count = 0, checkphoto = 0;
		private Image profilePic;
		readonly double rate;
		readonly string admin;
		readonly Label[] comment,name;
		readonly Picker statusData;
		private MediaFile mediafile;
		StackLayout parent;

		public ProfileJobSeeker(JobSeeker jobseeker, Enterprise enterprise, double rate, string admin)
		{
			this.jobseeker = jobseeker;
			this.enterprise = enterprise;
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
				Text = "Profile",
				Font = Font.SystemFontOfSize(25),
				HorizontalOptions = LayoutOptions.Center,
			};

			profilePic = new Image
			{
				HorizontalOptions = LayoutOptions.Center,
				Source = "http://temployapi2017.azurewebsites.net/api/Image/" + jobseeker.PhotoName,
				//Source = "http://169.254.30.178:2345/Image/" + jobseeker.PhotoName,
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
			nameData = new Entry
			{
				Text = jobseeker.Name
			};
			nameData2 = new Label
			{
				Text = jobseeker.Name
			};

			Label emailLbl = new Label
			{
				Text = "Email"
			};
			emailData = new Entry
			{
				Text = jobseeker.Email
			};
			emailData2 = new Button
			{
				Text = jobseeker.Email,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 45,
			};
			emailData2.FontAttributes = FontAttributes.Bold;
			emailData2.Clicked += OnEmail;

			if (enterprise == null && admin == null)
			{
				usernameLbl = new Label
				{
					Text = "Username"
				};
				usernameData = new Label
				{
					Text = jobseeker.Username
				};

				passwordLbl = new Label
				{
					Text = "Password"
				};
				passwordData = new Entry
				{
					Text = jobseeker.Password
				};
			}

			Label rateLbl = new Label
			{
				Text = "Rating"
			};
			rateData = new Label
			{
				Text = double.IsNaN(rate) ? "0" : rate.ToString() + "/5",
			};

			Label commentLbl = new Label
			{
				Text = "Reviews"
			};

			List<string> item = new List<string> { "Active", "Banned" };
			statusLbl = new Label
			{
				Text = "User Status"
			};
			statusData = new Picker
			{
				Title = "Status",
				ItemsSource = item,
				SelectedItem = jobseeker.Status,
			};

			remarkLbl = new Label
			{
				Text = "Remark"
			};
			remarkData = new Editor
			{
				Text = jobseeker.Remark,
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

			//Jobseeker View
			if(enterprise == null && admin == null)
			{
				parent.Children.Add(hearderLbl);
				parent.Children.Add(profilePic);
				parent.Children.Add(photoBtn);
				parent.Children.Add(nameLbl);
				parent.Children.Add(nameData);
				parent.Children.Add(emailLbl);
				parent.Children.Add(emailData);
				parent.Children.Add(usernameLbl);
				parent.Children.Add(usernameData);
				parent.Children.Add(passwordLbl);
				parent.Children.Add(passwordData);
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
				parent.Children.Add(nameData2);
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
			var feedback = await jsmanager.GetAllJSFeedback(jobseeker.Username);

			foreach (JSFeedback jsfeedback in feedback)
			{
				if(jsfeedbacks.All(b => b.Id != jsfeedback.Id))
				{
					jsfeedbacks.Add(jsfeedback);
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
						Text = jsfeedbacks[i].EntName + " - " + jsfeedbacks[i].Rating + "/5"
					};

					comment[i] = new Label
					{
						Text = jsfeedbacks[i].Review,
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
			if (enterprise == null && admin == null)
			{
				string name = nameData.Text.TrimEnd();
				string email = emailData.Text.TrimEnd();
				string password = passwordData.Text.TrimEnd();
				Regex whitespaceregex = new Regex(@"\s{1,}");
				Regex usernamelimitregex = new Regex("[A-Za-z0-9]{50,}");
				Regex namelimitregex = new Regex("[0-9]{1,}");
				Regex emailregex = new Regex("[@]{1,}");

				if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
				{
					await DisplayAlert("Alert Message", "Please fill up all the details.", "Cancel");
				}
				else if (usernamelimitregex.IsMatch(password) || usernamelimitregex.IsMatch(email) || password.Length > 50 || email.Length > 50)
				{
					await DisplayAlert("Alert Message", "Maximum 50 characters allowed for Name, Password and Email.", "Cancel");
				}
				else if (namelimitregex.IsMatch(name) || name.Length > 50)
				{
					await DisplayAlert("Alert Message", "No numeric character allowed for name.", "Cancel");
				}
				else if (whitespaceregex.IsMatch(password) || whitespaceregex.IsMatch(email))
				{
					await DisplayAlert("Alert Message", "No space is allowed for Username, Password and Email.", "Cancel");
				}
				else if (!(emailregex.IsMatch(email)))
				{
					await DisplayAlert("Alert Message", "Missing '@' for Email.", "Cancel");
				}
				else
				{
					string photoname = jobseeker.PhotoName;
					if(checkphoto != 0)
					{
						var photo = await jsmanager.SendPhoto(mediafile);
						photoname = photo.Trim('"');
					}
					JobSeeker user = new JobSeeker(name, jobseeker.Username, password, email, jobseeker.Status,jobseeker.Remark,photoname);
					var update = await jsmanager.UpdateProfile(user);
					if(update != null)
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
			}else if (admin != null)
			{
				string status = statusData.Items[statusData.SelectedIndex];
				string remark = remarkData.Text;
				if (string.IsNullOrEmpty(status))
				{
					await DisplayAlert("Alert Message", "Please select the status of the user.", "Cancel");
				}
				else
				{
					JobSeeker user = new JobSeeker(jobseeker.Name, jobseeker.Username, jobseeker.Password, jobseeker.Email, status, remark,jobseeker.PhotoName);
					var update = await jsmanager.UpdateProfile(user);
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

		public void OnEmail(object send, EventArgs e)
		{
			string to = jobseeker.Email;
			var emailTask = CrossMessaging.Current.EmailMessenger;
			if (emailTask.CanSendEmail)
			{
				// Send simple e-mail to single receiver without attachments, CC, or BCC.
				//emailTask.SendEmail("plugins@xamarin.com", "Xamarin Messaging Plugin", "Hello from your friends at Xamarin!");

				var email = new EmailMessageBuilder()
				  .To(to)
				  .Subject("Job Title - ")
				  .Body("Hi! " + jobseeker.Name + ",")
				  .Build();

				emailTask.SendEmail(email);
			}
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}