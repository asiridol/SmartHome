using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using System.Runtime.CompilerServices;

namespace SmartHome.ViewModels
{
	public class ValidatableProperty<T> : BindableObject
	{
		private bool _isValid;
		private string _error;
		private T _property;

		private readonly List<ValidationRule<T>> _validationRules = new List<ValidationRule<T>>();

		public bool IsValid
		{
			get => _isValid;
			set
			{
				if (_isValid == value)
				{
					return;
				}

				_isValid = value;

				OnPropertyChanged();
			}
		}

		public string Error
		{
			get => _error;
			set
			{
				if (_error == value)
				{
					return;
				}

				_error = value;
				OnPropertyChanged();
			}
		}

		public T Property
		{
			get => _property;
			set
			{
				if (Compare<T>(_property, value))
				{
					return;
				}

				_property = value;

				OnPropertyChanged();
				CheckValidity();
			}
		}

		public void AddRule(Func<T, bool> checkFunc, string errorMessage)
		{
			var rule = new ValidationRule<T> { CheckFunc = checkFunc, ErrorMessage = errorMessage };
			_validationRules.Add(rule);
		}

		private void CheckValidity()
		{
			if (!_validationRules.Any())
			{
				return;
			}

			foreach (var rule in _validationRules)
			{
				if (!rule.CheckFunc.Invoke(Property))
				{
					IsValid = false;
					Error = rule.ErrorMessage;
					break;
				}
				else
				{
					IsValid = true;
					Error = string.Empty;
				}
			}
		}

		private bool Compare<TValue>(TValue x, TValue y)
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

		public bool Validate()
		{
			CheckValidity();
			return IsValid;
		}

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
		private bool IsDefaultOrNull<T>(T obj)
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
		{
			return EqualityComparer<T>.Default.Equals(obj, default(T));
		}

		private class ValidationRule<TValue>
		{
			public Func<TValue, bool> CheckFunc { get; set; }
			public string ErrorMessage { get; set; }
		}
	}
}
