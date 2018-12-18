using System;
using System.ComponentModel;
namespace SmartHome.ViewModels
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public virtual void Cleanup()
		{
		}

		protected void RaisePropertyChanged(string nameOfProperty)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
		}
	}
}
