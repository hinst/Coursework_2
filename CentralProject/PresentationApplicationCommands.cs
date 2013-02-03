using System;
using System.Windows.Input;

using MyCSharp;

namespace Coursework_2
{

	public class PresentationApplicationCommands
	{

		protected RoutedUICommand addShape;

		public RoutedUICommand AddShape
		{
			get
			{
				return AutoCreateField.Get(
						ref addShape,
						() => new RoutedUICommand("Add shape", "AddShape", this.GetType())
					);
			}
		}

		protected static PresentationApplicationCommands commands;

		public static PresentationApplicationCommands Commands
		{
			get
			{
				return AutoCreateField.Get(ref commands, () => new PresentationApplicationCommands());
			}
		}

	}

}
