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
	public partial class ViewApplyJob : ContentPage
	{
		readonly IList<Submission> submissions = new ObservableCollection<Submission>();
		readonly IList<Job> jobs = new ObservableCollection<Job>();
		private IList<EntFeedback> entfeedbacks = new ObservableCollection<EntFeedback>();
		readonly JobSeekerManager jsmanager = new JobSeekerManager();
		readonly JobSeeker jobseeker;
		readonly Enterprise enterprise;
		private Job job;

		public ViewApplyJob(JobSeeker jobseeker)
		{
			this.jobseeker = jobseeker;
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
		}

		async void OnRefresh(object sender, EventArgs e)
		{
			this.IsBusy = true;
			SubmissionView.ItemsSource = submissions;
			try
			{
				var jobCollection = await jsmanager.GetApplyJob(jobseeker.Username);

				foreach (Submission submission in jobCollection)
				{
					if (submissions.All(b => b.Id != submission.Id))
						submissions.Add(submission);
				}
			}
			finally
			{
				this.IsBusy = false;
			}
		}

		async void OnDismiss(object sender, ItemTappedEventArgs e)
		{
			Submission submit = (Submission)e.Item;
			//submission = submit;
			int id = submit.Jobid;
			string view = "Valued";

			var searchent = await jsmanager.GetEntById(id);
			if(searchent != null) { 
				job = searchent;
			}

			var entsearch = await jsmanager.GetEntByJobid(id);

			Job work = entsearch;

			var ent = await jsmanager.GetEnterprise(work.Entusername);


			if (job.Status.Equals("Active") && submit.Status.Equals("Ent Approved"))
			{
				//accept by job seeker
				if(await this.DisplayAlert("Accept Job", "Congratulation! Your application has been accepted by the company. Do you want to accept the job?", "Yes", "Cancel") == true)
				{
					submit.Status = "User Approved";
					var approve = await jsmanager.UpdateSubmission(submit);
					await DisplayAlert("Message", "Application has been accepted successfully. In the mean time, please contact the company if has any inquire regards to the job.", "Cancel");
				}
			}
			else if (job.Status.Equals("Active") && submit.Status.Equals("User Approved"))
			{
				await Navigation.PushModalAsync(new JobDetail(enterprise, jobseeker, job, submit,view,ent));
			}
			else if (job.Status.Equals("End") && submit.Status.Equals("Ent Approved"))
			{
				await DisplayAlert("Message", "Job has already ended.", "Cancel");
				await Navigation.PushModalAsync(new JobDetail(enterprise, jobseeker, job, submit,view,ent));
			}
			else if (job.Status.Equals("End") && submit.Status.Equals("User Completed"))
			{
				await Navigation.PushModalAsync(new JobDetail(enterprise, jobseeker, job, submit,view,ent));
			}
			else if (job.Status.Equals("End") && (submit.Status.Equals("User Approved") || submit.Status.Equals("Ent Completed")))
			{
				//feedback
				int count = 0;
				var check = await jsmanager.CheckEntFeedback(id);

				foreach (EntFeedback entfeedback in check)
				{
					if (entfeedbacks.All(b => b.Id != entfeedback.Id))
					{
						if (entfeedback.Submissionid != submit.Id)
						{
							count++;
						}
					}
				}

				if (count == 0)
				{
					await Navigation.PushModalAsync(new Feedback(job, jobseeker, enterprise, (Submission)e.Item));
				}
				else
				{
					await Navigation.PushModalAsync(new JobDetail(enterprise, jobseeker, job, submit,view,ent));
				}
			}
			else
			{
				await DisplayAlert("Message", "Your application still under the consideration of the company.", "Cancel");
				await Navigation.PushModalAsync(new JobDetail(enterprise, jobseeker, job, submit,view,ent));
			}
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}