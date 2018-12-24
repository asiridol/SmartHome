using System;
using System.IO;
using SmartHome.Services.FileSystem;
namespace SmartHome.iOS.Services.FileSystem
{
	public class IosFilePaths : IFilePaths
	{
		public string DocumentsPath
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
			}
		}
	}
}
