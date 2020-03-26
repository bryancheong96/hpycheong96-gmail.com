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
	public partial class HomeEnterprise : ContentPage
	{
		readonly IList<Job> jobs = new ObservableCollection<Job>();
		readonly IList<Submission> submissions = new ObservableCollection<Submission>();
		readonly IList<EntFeedback> entfeedbacks = new ObservableCollection<EntFeedback>();
		readonly EnterpriseManager entmanager = new EnterpriseManager();
		readonly JobSeekerManager jsmanager = new JobSeekerManager();
		readonly Enterprise enterprise;
		private JobSeeker jobseeker=null;
		private Submission submission;

		public HomeEnterprise(Enterprise enterprise)
		{
			this.enterprise = enterprise;
			InitializeComponent();
			OnRefresh();
			searchBar.TextChanged += (sender2, e2) => FilterContacts(searchBar.Text);
			searchBar.SearchButtonPressed += (sender2, e2) => FilterContacts(searchBar.Text);

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
			// Turn on network indicator
			this.IsBusy = true;
			HomeView.ItemsSource = jobs;
			try
			{
				var jobCollection = await entmanager.GetJobs(enterprise.Username);

				foreach (Job job in jobCollection)
				{
					if (jobs.All(b => b.Id != job.Id))
						//await DisplayAlert("Login", job.Title , "Cancel");
						jobs.Add(job);
				}
			}
			finally
			{
				this.IsBusy = false;
			}
		}


		async void OnCreate(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new CreateJob(enterprise));
		}

		async void ViewProfile(object sender, ItemTappedEventArgs e)
		{
			double rate = 0;
			int divide = 0;
			string admin=null;
			var feedback = await entmanager.GetAllEntFeedback(enterprise.Username);

			foreach (EntFeedback entfeedback in feedback)
			{
				if (entfeedbacks.All(b => b.Id != entfeedback.Id))
				{
					divide++;
					rate = rate + entfeedback.Rating;
				}
			}
			rate = rate / divide;
			await Navigation.PushModalAsync(new ProfileEnterprise(enterprise, jobseeker, rate,admin));
		}

		async void JobDetail(object sender, ItemTappedEventArgs e)
		{
			string view, username;
			username = ((Job)e.Item).Entusername;

			var ent = await jsmanager.GetEnterprise(username);
			await Navigation.PushModalAsync(new JobDetail(enterprise,jobseeker,(Job)e.Item,submission,view = null,ent));
		}

		void OnSearch(object sender, EventArgs e)
		{
			if (searchBar.IsVisible == true)
			{
				searchBar.IsVisible = false;
			}
			else
			{
				searchBar.IsVisible = true;
			}
		}

		private void FilterContacts(string filter)
		{
			HomeView.BeginRefresh();

			if (string.IsNullOrWhiteSpace(filter))
			{
				HomeView.ItemsSource = jobs;
			}
			else
			{
				HomeView.ItemsSource = jobs.Where(x => x.Title.ToLower().Contains(filter.ToLower()));
			}

			HomeView.EndRefresh();
		}

		async void OnApplicant(object sender, EventArgs e)
		{	
			MenuItem item = (MenuItem)sender;
			Job jobObject = item.CommandParameter as Job;
			await Navigation.PushModalAsync(new NavigationPage(new ViewSubmission(jobObject,enterprise)));
			/*
			Book book = item.CommandParameter as Book;
			if (book != null)
			{
				if (await this.DisplayAlert("Delete Book?",
					"Are you sure you want to delete the book '"
						+ book.Title + "'?", "Yes", "Cancel") == true)
				{
					this.IsBusy = true;
					try
					{
						await manager.Delete(book.ISBN);
						books.Remove(book);
					}
					finally
					{
						this.IsBusy = false;
					}

				}
			}*/
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}