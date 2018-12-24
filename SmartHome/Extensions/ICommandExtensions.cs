using System;
using System.Windows.Input;
namespace SmartHome.Extensions
{
	public static class ICommandExtensions
	{
		public static void Execute(this ICommand command)
		{
			command.Execute(null);
		}
	}
}
