using System;
using Xamarin.Forms;
using SmartHome.Views;
using SmartHome.ViewModels;
using System.ComponentModel;
using SmartHome.Android.CustomRenderers;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ProgressAwarePage), typeof(ProgressAwarePageRenderer))]
namespace SmartHome.Android.CustomRenderers
{
	public class ProgressAwarePageRenderer : PageRenderer
	{
		private const double FadeDuration = 0.2;

		private UIActivityIndicatorView _progressView;
		private ProgressAwarePage _currentPage;
		private UIVisualEffectView _effectView;
		private UILabel _pleaseWaitLabel;

		private ProgressAwarePage CurrentPage
		{
			get { return _currentPage; }
			set
			{
				if (_currentPage == value)
				{
					return;
				}

				if (_currentPage != null)
				{
					_currentPage.PropertyChanged -= PropertyChanged;
				}

				if (value == null)
				{
					return;
				}

				_currentPage = value;
				_currentPage.PropertyChanged += PropertyChanged;

				UpdateVisibility();
			}
		}

		private void UpdateVisibility()
		{
			if (_progressView == null || _effectView == null)
			{
				return;
			}

			UIApplication.SharedApplication.KeyWindow.EndEditing(true);

			_progressView.Hidden = !CurrentPage.ShowProgress;

			if (CurrentPage.ShowProgress)
			{
				FadeIn();
				_progressView.StartAnimating();
			}
			else
			{
				FadeOut();
				_progressView.StopAnimating();
			}
		}

		private void FadeIn()
		{
			if (!_effectView.Hidden && _effectView.Alpha == 1f)
			{
				return;
			}

			_effectView.Hidden = false;

			UIView.Animate(FadeDuration,
			() => _effectView.Alpha = 1f, () =>
			{
			});
		}

		private void FadeOut()
		{
			if (_effectView.Hidden && _effectView.Alpha == 0f)
			{
				return;
			}

			_effectView.Hidden = false;

			UIView.Animate(FadeDuration,
			() => _effectView.Alpha = 0f, () =>
			{
				_effectView.Hidden = true;
			});
		}

		private void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ProgressAwarePage.ShowProgress))
			{
				UpdateVisibility();
			}
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || Element == null)
			{
				return;
			}

			if (e.NewElement is ProgressAwarePage progressAwarePage)
			{
				CurrentPage = progressAwarePage;
			}

			SetupInterface();
		}

		private void SetupInterface()
		{
			_effectView = new UIVisualEffectView() { TranslatesAutoresizingMaskIntoConstraints = false };
			_effectView.UserInteractionEnabled = false;
			_effectView.BackgroundColor = UIColor.FromRGB(68, 28, 28).ColorWithAlpha(0.3f);
			//_effectView.BackgroundColor = UIColor.Red;
			_effectView.Effect = UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight);
			_progressView = new UIActivityIndicatorView() { TranslatesAutoresizingMaskIntoConstraints = false };
			_progressView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
			_progressView.Color = UIColor.FromRGB(178, 0, 0);
			_progressView.UserInteractionEnabled = false;

			_pleaseWaitLabel = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
			_pleaseWaitLabel.Text = "Please wait...";
			_pleaseWaitLabel.Font = UIFont.SystemFontOfSize(18f, UIFontWeight.Thin);
			_pleaseWaitLabel.TextAlignment = UITextAlignment.Center;
			_pleaseWaitLabel.TextColor = UIColor.FromRGB(178, 0, 0);

			_effectView.ContentView.AddSubview(_progressView);
			_effectView.ContentView.AddSubview(_pleaseWaitLabel);
			View.AddSubview(_effectView);

			var constraints = new[]
			{
				_effectView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
				_effectView.TopAnchor.ConstraintEqualTo(View.TopAnchor),
				_effectView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
				_effectView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor),
				_progressView.CenterXAnchor.ConstraintEqualTo(_effectView.CenterXAnchor),
				_progressView.CenterYAnchor.ConstraintEqualTo(_effectView.CenterYAnchor),
				_progressView.WidthAnchor.ConstraintEqualTo(100f),
				_progressView.HeightAnchor.ConstraintEqualTo(_progressView.WidthAnchor),
				_pleaseWaitLabel.CenterXAnchor.ConstraintEqualTo(_effectView.CenterXAnchor),
				_pleaseWaitLabel.WidthAnchor.ConstraintEqualTo(_effectView.WidthAnchor),
				_pleaseWaitLabel.TopAnchor.ConstraintEqualTo(_progressView.TopAnchor, 70f)
			};

			NSLayoutConstraint.ActivateConstraints(constraints);

			if (CurrentPage != null)
			{
				_effectView.Alpha = CurrentPage.ShowProgress ? 1f : 0f;
				_effectView.Hidden = !CurrentPage.ShowProgress;
			}

			View.SetNeedsUpdateConstraints();
		}

		protected override void Dispose(bool disposing)
		{
			InvokeOnMainThread(() =>
			{
				_progressView?.RemoveFromSuperview();
				_progressView.Dispose();
				_progressView = null;

				_effectView?.RemoveFromSuperview();
				_effectView.Dispose();
				_effectView = null;

				_pleaseWaitLabel?.RemoveFromSuperview();
				_pleaseWaitLabel.Dispose();
				_pleaseWaitLabel = null;
			});

			CurrentPage = null;

			base.Dispose(disposing);
		}
	}
}
