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
	public partial class HomeAdmin2 : ContentPage
	{
		readonly IList<JobSeeker> jobseekers = new ObservableCollection<JobSeeker>();
		readonly IList<JSFeedback> jsfeedbacks = new ObservableCollection<JSFeedback>();
		readonly AdminManager admanager = new AdminManager();
		readonly JobSeekerManager jsmanager = new JobSeekerManager();
		readonly Enterprise enterprise;

		public HomeAdmin2()
		{
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
			SwitchList.Text = "Enterprise List";
			HomeView.ItemsSource = jobseekers;
			Header.Text = "User List";
			try
			{
				var userCollection = await admanager.GetAllJobSeeker();

				foreach (JobSeeker jobseeker in userCollection)
				{
					if (jobseekers.All(b => b.Username != jobseeker.Username))
						//await DisplayAlert("Login", job.Title , "Cancel");
						jobseekers.Add(jobseeker);
				}
			}
			finally
			{
				this.IsBusy = false;
			}		
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
				HomeView.ItemsSource = jobseekers;
			}
			else
			{
				HomeView.ItemsSource = jobseekers.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
			}

			HomeView.EndRefresh();
		}

		async void ViewProfile(object sender, ItemTappedEventArgs e)
		{
			double rate = 0;
			int divide = 0;
			string admin = "true";

			JobSeeker selectedUser = (JobSeeker)e.Item;
			var feedback = await jsmanager.GetAllJSFeedback(selectedUser.Username);

			foreach (JSFeedback jsfeedback in feedback)
			{
				if (jsfeedbacks.All(b => b.Id != jsfeedback.Id))
				{
					divide++;
					rate = rate + jsfeedback.Rating;
				}
			}
			rate = rate / divide;
			await Navigation.PushModalAsync(new ProfileJobSeeker(selectedUser, enterprise, rate, admin));
		}

		async void EntList(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}