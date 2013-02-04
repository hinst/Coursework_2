using System;
using System.Windows;
using System.Windows.Input;
using MyCSharp;

namespace Coursework_2
{

	public partial class PresentationApplication : Application
	{

		public PresentationApplication()
			: base()
		{
			InitializeComponent();
		}

		new public static PresentationApplication Current
		{
			get
			{
				return (PresentationApplication) Application.Current;
			}
		}

		protected PresentationApplicationCommands commands;

		public PresentationApplicationCommands Commands
		{
			get
			{
				return AutoCreateField.Get(ref commands, () => new PresentationApplicationCommands());
			}
		}

	}

}
