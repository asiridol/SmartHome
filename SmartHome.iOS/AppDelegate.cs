using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xfx;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartHome.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//

		private TaskCompletionSource<bool> _initializedCompletionSource;

		public override bool WillFinishLaunching(UIApplication uiApplication, NSDictionary launchOptions)
		{
			_initializedCompletionSource = new TaskCompletionSource<bool>();
			return base.WillFinishLaunching(uiApplication, launchOptions);
		}

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			try
			{
				Xfx.XfxControls.Init();
				Forms.Init();
				LoadApplication(new App(new IosPlatformInitializer()));
			}
			finally
			{
				_initializedCompletionSource.TrySetResult(true);
			}

			return base.FinishedLaunching(app, options);
		}

		public Task EnsureInitializedAsync()
		{
			return _initializedCompletionSource.Task;
		}
	}
}
