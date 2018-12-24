using System;
namespace SmartHome.ViewModels
{
	public class ProgressAwareViewModel : ViewModelBase
	{
		private bool _isBusy;

		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set
			{
				Set(() => IsBusy, ref _isBusy, value);
			}
		}
	}
}
