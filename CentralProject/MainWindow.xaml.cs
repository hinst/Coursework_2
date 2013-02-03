using System;
using System.Windows;
using System.Windows.Input;
using System.Reflection;

using NLog;

namespace Coursework_2
{

	public partial class MainWindow : Window
	{

		protected Logger log = LogManager.GetCurrentClassLogger();

		public MainWindow()
			: base()
		{
			log.Debug("Now creating...");
			InitializeComponent();
			BindCommands();
		}

		protected void BindCommands()
		{
			CommandManager.RegisterClassCommandBinding(
				this.GetType(),
				new CommandBinding(PresentationApplication.Current.Commands.AddShape, AddShape)
			);
			AddShapeMenuItem.Command = PresentationApplication.Current.Commands.AddShape; // this is the part I want to move in the xaml file
		}

		protected bool addShapeState;

		public bool AddShapeState
		{
			get
			{
				return addShapeState;
			}
			set
			{
				addShapeState = value;
				if (AddShapeState)
				{
					Mouse.OverrideCursor = Cursors.Pen;
					Mouse.AddPreviewMouseDownHandler(this, MousePenPreview);
				}
				else
				{
					Mouse.OverrideCursor = null;
					Mouse.RemovePreviewMouseDownHandler(this, MousePenPreview);
				}
			}
		}

		protected void AddShape(object sender, ExecutedRoutedEventArgs args)
		{
			log.Debug("User commands: add shape");
			AddShapeState = true;
		}

		void MousePenPreview(object sender, MouseButtonEventArgs e)
		{
			log.Debug(MethodBase.GetCurrentMethod());
			if (e.ChangedButton == MouseButton.Right)
				AddShapeState = false;
		}

	}

}
