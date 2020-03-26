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
	public partial class HomeAdmin : ContentPage
	{
		readonly JobSeeker jobseeker;
		readonly Enterprise enterprise;
		readonly AdminManager admanager = new AdminManager();
		readonly EnterpriseManager entmanager = new EnterpriseManager();
		readonly IList<Enterprise> enterprises = new ObservableCollection<Enterprise>();
		readonly IList<EntFeedback> entfeedbacks = new ObservableCollection<EntFeedback>();

		public HomeAdmin()
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
		
			SwitchList.Text = "User List";
			HomeView.ItemsSource = enterprises;
			Header.Text = "Enterprise List";
			try
			{
				var entCollection = await admanager.GetAllEnt();

				foreach (Enterprise enterprise in entCollection)
				{
					if (enterprises.All(b => b.Username != enterprise.Username))
						//await DisplayAlert("Login", job.Title , "Cancel");
						enterprises.Add(enterprise);
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
				HomeView.ItemsSource = enterprises;
			}
			else
			{
				HomeView.ItemsSource = enterprises.Where(x => x.CompanyName.ToLower().Contains(filter.ToLower()));
			}

			HomeView.EndRefresh();
		}

		async void ViewProfile(object sender, ItemTappedEventArgs e)
		{
			double rate = 0;
			int divide = 0;
			string admin = "true";

			Enterprise selectedEnt = (Enterprise)e.Item;
			var feedback = await entmanager.GetAllEntFeedback(selectedEnt.Username);
		
			foreach (EntFeedback entfeedback in feedback)
			{
				if (entfeedbacks.All(b => b.Id != entfeedback.Id))
				{
					divide++;
					rate = rate + entfeedback.Rating;
				}
			}
			rate = rate / divide;
			await Navigation.PushModalAsync(new ProfileEnterprise(selectedEnt, jobseeker, rate, admin));
		}

		async void UserList(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new HomeAdmin2()));
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}