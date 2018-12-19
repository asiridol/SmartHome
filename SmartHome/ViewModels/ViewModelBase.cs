using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
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

		protected bool Set<TValue>(Expression<Func<TValue>> member, ref TValue originalProperty, TValue value)
		{
			bool success = false;
			string propertyName = string.Empty;

			if (Compare(originalProperty, value))
			{
				return success;
			}

			var memberSelectorExpression = member.Body as MemberExpression;

			if (memberSelectorExpression != null)
			{
				var property = memberSelectorExpression.Member as PropertyInfo;
				propertyName = property.Name;

				if (property != null)
				{
					originalProperty = value;
					success = true;
				}
			}

			if (success)
			{
				RaisePropertyChanged(propertyName);
			}

			return success;
		}

		private bool Compare<T>(T x, T y)
		{
			if (IsDefaultOrNull(x) && IsDefaultOrNull(y))
			{
				return true;
			}

			if (IsDefaultOrNull(x) || IsDefaultOrNull(y))
			{
				return false;
			}

			return x.Equals(y);
		}

		private bool IsDefaultOrNull<T>(T obj)
		{
			return EqualityComparer<T>.Default.Equals(obj, default(T));
		}
	}
}
