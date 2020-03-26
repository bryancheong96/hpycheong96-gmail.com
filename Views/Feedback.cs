using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TemPloy.Controller;
using TemPloy.Models;
using Xamarin.Forms;

namespace TemPloy.Views
{
	public class Feedback : ContentPage
	{
		readonly Editor commentTxt;
		readonly Slider slider;
		readonly Label label;
		readonly Job job;
		readonly JobSeeker jobseeker;
		readonly Submission submission;
		readonly Enterprise enterprise;
		readonly JobSeekerManager jsmanager = new JobSeekerManager();
		readonly EnterpriseManager entmanager = new EnterpriseManager();

		public Feedback(Job job, JobSeeker jobseeker, Enterprise enterprise ,Submission submission)
		{
			this.job = job;
			this.jobseeker = jobseeker;
			this.enterprise = enterprise;
			this.submission = submission;

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

			DisplayAlert("Message", "Please complete the feedback form in order for you to review back your application.", "Cancel");

			Label hearderLbl = new Label
			{
				Text = "Feedback",
				Font = Font.SystemFontOfSize(30),
				HorizontalOptions = LayoutOptions.Center,
			};

			Label commentLbl = new Label
			{
				Text = "Comment",
			};

			commentTxt = new Editor
			{
				HeightRequest = 200,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			
			Label ratelbl = new Label
			{
				Text = "Rating",
			};

			slider = new Slider
			{
				Minimum = 0,
				Maximum = 5,
			};
			slider.ValueChanged += OnSliderValueChanged;

			label = new Label
			{
				Text = "Slider value is 0",
				HorizontalOptions = LayoutOptions.Center,
				//VerticalOptions = LayoutOptions.CenterAndExpand
			};

			// Accomodate iPhone status bar.
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);


			Button Savebtn = new Button()
			{
				//BackgroundColor = existingBook != null ? Color.Gray : Color.Green,
				TextColor = Color.White,
				Text = "Save",
				BorderRadius = 0, // corner of the button
				HeightRequest = 50,
				VerticalOptions = LayoutOptions.EndAndExpand,
			};
			Savebtn.Clicked += OnSave;

			Content = new StackLayout
			{
				Spacing = 0, //no idea
				Children = { hearderLbl, commentLbl, commentTxt, ratelbl, slider, label, Savebtn },
			};
		}

		void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
		{
			label.Text = String.Format("Slider value is {0:F1}", e.NewValue);
		}

		async void OnSave(object sender, EventArgs e)
		{
			int id = 0;
			string comment = commentTxt.Text.TrimEnd(); 
			double rate = Math.Round(slider.Value,2);
			DateTime today = DateTime.Today;

			//Enterprise Feedback to Applicant
			if (enterprise != null)
			{
				JSFeedback jsfeedback = new JSFeedback(id, enterprise.CompanyName, enterprise.Username, jobseeker.Name, jobseeker.Username, submission.Id, today, comment, rate);
				var savejsfeedback = await entmanager.SaveFeedback(jsfeedback);
				if (savejsfeedback != null)
				{
					submission.Status = "Ent Completed";
					var updatesubmission = await entmanager.UpdateSubmission(submission);
					if (updatesubmission != null)
					{
						await DisplayAlert("Submitted Successful", "Feedback submitted successfully.", "Cancel");
						await Navigation.PopModalAsync();
					}
					else
					{
						await DisplayAlert("Error Message", "Something wents wrong in updating status.", "Cancel");
					}
				}
				else
				{
					await DisplayAlert("Sumission Failed", "Feedback fail to submit.", "Cancel");
					await Navigation.PopModalAsync();
				}
			}
			else
			{
				var ent = await jsmanager.GetEnterprise(job.Entusername);
				EntFeedback entfeedback = new EntFeedback(id, ent.CompanyName, job.Entusername, jobseeker.Name, jobseeker.Username, submission.Id, today, comment, rate);
				var convert = JsonConvert.SerializeObject(entfeedback);
				await DisplayAlert("Submitted Successful", convert, "Cancel");
				var savefeedback = await jsmanager.SaveEntFeedback(entfeedback);
				if (savefeedback != null)
				{
					submission.Status = "User Completed";
					var updatesubmission = await jsmanager.UpdateSubmission(submission);
					if (updatesubmission != null)
					{
						await DisplayAlert("Submitted Successful", "Feedback submitted successfully.", "Cancel");
						await Navigation.PopModalAsync();
					}
					else
					{
						await DisplayAlert("Error Message", "Something wents wrong in updating status.", "Cancel");
					}
				}
				else
				{
					await DisplayAlert("Sumission Failed", "Feedback fail to submit.", "Cancel");
					await Navigation.PopModalAsync();
				}
			}
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}