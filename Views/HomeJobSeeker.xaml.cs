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
	public partial class HomeJobSeeker : ContentPage
	{
		readonly IList<Job> jobs = new ObservableCollection<Job>();
		readonly IList<Job> jobss = new ObservableCollection<Job>();
		readonly IList<Enterprise> ents = new ObservableCollection<Enterprise>();
		readonly IList<JSFeedback> jsfeedbacks = new ObservableCollection<JSFeedback>();
		readonly JobSeekerManager jsmanager = new JobSeekerManager();
		readonly EnterpriseManager entmanager = new EnterpriseManager();
		readonly MapManager mapmanager = new MapManager();
		readonly JobSeeker jobseeker;
		readonly string name, address;
		private Enterprise enterprise=null, ent1;
		private Submission submission;

		public HomeJobSeeker(JobSeeker jobseeker, string name, string address)
		{
			this.jobseeker = jobseeker;
			this.name = name;
			this.address = address;

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


			if (name != null)
			{
				JobList(name,address);
			}
			else
			{
				OnRefresh();
			}
			searchBar.TextChanged += (sender2, e2) => FilterContacts(searchBar.Text);
			searchBar.SearchButtonPressed += (sender2, e2) => FilterContacts(searchBar.Text);
		}

		async void OnRefresh()
		{
			// Turn on network indicator
			this.IsBusy = true;

			HomeView.ItemsSource = jobs;

			try
			{
				var jobCollection = await jsmanager.GetJobs();

				foreach (Job job in jobCollection)
				{
					if (jobs.All(b => b.Id != job.Id))
					{
						if (job.Status.Equals("Active"))
						{
							jobs.Add(job);
						}
					}
				}
			}
			finally
			{
				this.IsBusy = false;
			}
		}

		async void JobDetail(object sender, ItemTappedEventArgs e)
		{
			string view,username;
			username = ((Job)e.Item).Entusername;

			var ent = await jsmanager.GetEnterprise(username);
			await Navigation.PushModalAsync(new JobDetail(enterprise,jobseeker,(Job)e.Item, submission,view =null,ent));
		}

		async void ViewApplyJob(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new ViewApplyJob(jobseeker)));
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

		async void ViewProfile(object sender, EventArgs e)
		{
			double rate = 0;
			int divide = 0;
			string admin = null;
			var feedback = await jsmanager.GetAllJSFeedback(jobseeker.Username);

			foreach (JSFeedback jsfeedback in feedback)
			{
				if (jsfeedbacks.All(b => b.Id != jsfeedback.Id))
				{
					divide++;
					rate = rate + jsfeedback.Rating;
				}
			}
			rate = rate / divide;
			await Navigation.PushModalAsync(new ProfileJobSeeker(jobseeker,enterprise,rate,admin));
		}

		async void OnMap(object send, EventArgs e)
		{
			//await Navigation.PushModalAsync(new NavigationPage(new MapPage(jobseeker)));
		}

		async void JobList(string name, string address)
		{
			this.IsBusy = true;

			HomeView.ItemsSource = jobss;

			try
			{
				var searchent = await mapmanager.GetEntByNameAdd(name,address);
				foreach(Enterprise ent in searchent)
				{
					if(ents.All(b => b.CompanyID != ent.CompanyID)){
						ent1 = ent;
						break;
					}
				}

				var list = await mapmanager.GetJobs(ent1.Username);
				foreach (Job job in list)
				{
					if (jobss.All(b => b.Id != job.Id))
					{
						if (job.Status.Equals("Active"))
						{
							jobss.Add(job);
						}
					}
				}
			}
			finally
			{
				this.IsBusy = false;
			}
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}