using System;
using SmartHome.Controls;
using SmartHome.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SelectableViewCell), typeof(SelectableViewCellRenderer))]
namespace SmartHome.iOS.CustomRenderers
{
	public class SelectableViewCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);

			if (item is SelectableViewCell selectableViewCell)
			{
				cell.SelectedBackgroundView = new UIView
				{
					BackgroundColor = selectableViewCell.SelectedBackgroundColor.ToUIColor(),
				};
			}

			return cell;
		}
	}
}
