using System;
using System.Collections.Generic;

using Xamarin.Forms;
using SmartHome.ViewModels;
using SmartHome.Extensions;

namespace SmartHome.Views
{
	public partial class MainPage : ProgressAwareViewModelPage<MainPageViewModel>
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			ViewModel.InitCommand.Execute();
			RoomsList.ItemSelected += RoomSelected;
			ViewModel.ClearSelection += ClearRoomSelection;
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			RoomsList.ItemSelected -= RoomSelected;
			ViewModel.ClearSelection -= ClearRoomSelection;
			base.OnDisappearing();
		}

		private void ClearRoomSelection(object sender, EventArgs e)
		{
			RoomsList.SelectedItem = null;
		}

		private void RoomSelected(object sender, SelectedItemChangedEventArgs e)
		{
			ViewModel.RoomSelectedCommand.Execute(e.SelectedItem);
		}
	}
}
