using Xamarin.Forms;
using Xfx;

namespace SmartHome.Behaviors
{
	public class LowerCaseEntityBehavior : Behavior<XfxEntry>
	{
		protected override void OnAttachedTo(XfxEntry entry)
		{
			entry.TextChanged += OnEntryTextChanged;
			base.OnAttachedTo(entry);
		}

		protected override void OnDetachingFrom(XfxEntry entry)
		{
			entry.TextChanged -= OnEntryTextChanged;
			base.OnDetachingFrom(entry);
		}

		void OnEntryTextChanged(object sender, TextChangedEventArgs args)
		{
			((Entry)sender).Text = args.NewTextValue.ToLower();
		}
	}
}
