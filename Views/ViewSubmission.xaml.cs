using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemPloy.Controller;
using TemPloy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TemPloy.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewSubmission : ContentPage
	{
		readonly IList<Job> jobs = new ObservableCollection<Job>();
		readonly IList<Submission> submissions = new ObservableCollection<Submission>();
		private IList<JSFeedback> jsfeedbacks = new ObservableCollection<JSFeedback>();
		private JobSeeker jobseeker;
		readonly EnterpriseManager entmanager = new EnterpriseManager();
		readonly JobSeekerManager jsmanager = new JobSeekerManager();
		readonly Job job;
		readonly Enterprise enterprise;

		public ViewSubmission(Job job, Enterprise enterprise)
		{
			this.job = job;
			this.enterprise = enterprise;
			InitializeComponent();
			OnRefresh();

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
		}

		async void OnRefresh()
		{
			this.IsBusy = true;
			SubmissionView.ItemsSource = submissions;
			try
			{
				var jobCollection = await entmanager.GetSubmission(job.Id);
				/*
				foreach (Submission submission in jobCollection)
				{
					if(submissions.All(b => b.Title != submission.Title))
					{
						await DisplayAlert("Login", submission.Id.ToString() + submission.Username, "Cancel");
						submissions.Add(submission);
						foreach(JobSeeker jobseeker in submission.JobSeeker){
							if(jobseekers.All(b => b.Username != jobseeker.Username))
							{
								await DisplayAlert("Login", jobseeker.Username, "Cancel");
								jobseekers.Add(jobseeker);
							}
						}
					}
				}*/
				
				foreach (Job job in jobCollection)
				{
					if (jobs.All(b => b.Id != job.Id))
						jobs.Add(job);
					
					foreach (Submission submission in job.Submit)
					{
						if(submissions.All(b => b.Id != submission.Id))
						{
							//await DisplayAlert("Login", submission.Id.ToString(), "Cancel");
							submissions.Add(submission);
						}
					}
				}
			}
			finally
			{
				this.IsBusy = false;
			}
		}

		async void ViewProfile(object sender, ItemTappedEventArgs e)
		{
			Submission submit = (Submission)e.Item;
			int count = 0, divide=0;
			double rate = 0, total=0;
			string admin = null;

			var searchjs = await entmanager.GetJobSeekerByUsername(submit.Username);
			if(searchjs != null)
			{
				jobseeker = searchjs;
			}

			var feedback = await jsmanager.GetAllJSFeedback(jobseeker.Username);

			foreach (JSFeedback jsfeedback in feedback)
			{
				if (jsfeedbacks.All(b => b.Id != jsfeedback.Id))
				{
					divide++;
					rate += jsfeedback.Rating;
				}
			}

			rate = rate / divide;

			//tap method to view user profile
			if (job.Status.Equals("End") && (submit.Status.Equals("User Approved") || submit.Status.Equals("JS Completed")))
			{
				var check = await entmanager.CheckJSFeedback(submit.Id);

				foreach (JSFeedback jsfeedback in check)
				{
					if (jsfeedbacks.All(b => b.Id != jsfeedback.Id))
					{
						if(jsfeedback.Submissionid != submit.Id)
						{
							count++;
						}
					}
				}

				if (count == 0)
				{
					await Navigation.PushModalAsync(new Feedback(job, jobseeker, enterprise, submit));
				}
				else
				{
					// view job seeker profile
					await Navigation.PushModalAsync(new ProfileJobSeeker(jobseeker, enterprise, rate, admin));
				}
			}
			else
			{
				//view job seeker profile
				await Navigation.PushModalAsync(new ProfileJobSeeker(jobseeker, enterprise, rate, admin));
			}
		}

		async void AcceptJob(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender;
			Submission applicant = item.CommandParameter as Submission;

			if (applicant.Status.Equals("Ent Approved") || applicant.Status.Equals("User Approved"))
			{
				await DisplayAlert("Alert Message", applicant.Username + " already accpted.", "Cancel");
			}
			else if (job.Status.Equals("End") || applicant.Status.Equals("JS Completed"))
			{
				await DisplayAlert("Alert Message", "This job has already ended/completed. Accept applicant is not allow in this stage anymore.", "Cancel");
			}
			else if (await this.DisplayAlert("Accept Applicant?", "Are you sure you want to accept applicant, " + applicant.Username + " ?", "Yes", "Cancel") == true)
			{
				this.IsBusy = true;
				try
				{
					applicant.Status = "Ent Approved";
					var accept = await entmanager.UpdateSubmission(applicant);
					if (accept != null)
					{
						await DisplayAlert("Accepted Applicant", applicant.Username + " Accept Successful.", "Cancel");
					}
					else
					{
						await DisplayAlert("Failed", applicant.Username + " fail to accept.", "Cancel");
					}
				}
				finally
				{
					this.IsBusy = false;
				}

			}
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}
 