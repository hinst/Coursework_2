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
						() => new RoutedUICommand("Add shape", "AddShape", GetType())
					);
			}
		}

		protected RoutedUICommand openFile;

		public RoutedUICommand OpenFile
		{
			get
			{
				return AutoCreateField.Get(
					ref openFile, 
					() => new RoutedUICommand("Open file", "OpenFile", GetType())
				);
			}
		}

		protected RoutedUICommand saveFile;

		public RoutedUICommand SaveFile
		{
			get
			{
				return AutoCreateField.Get(
						ref saveFile,
						() => new RoutedUICommand("Save file", "SaveFile", GetType())
					);
			}
		}

		protected RoutedUICommand saveAsFile;

		public RoutedUICommand SaveAsFile
		{
			get
			{
				return AutoCreateField.Get(
						ref saveAsFile,
						() => new RoutedUICommand("Save as file...", "SaveAsFile", GetType())
					);
			}
		}

		protected RoutedUICommand drawLink;

		public RoutedUICommand DrawLink
		{
			get
			{ 
				return AutoCreateField.Get(
						ref drawLink,
						() => new RoutedUICommand("Draw link...", "DrawLink", GetType())
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
