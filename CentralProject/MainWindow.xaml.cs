using System;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

using NLog;

using MyWPF;

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
					Mouse.AddPreviewMouseDownHandler(this, MouseDownPenPreview);
				}
				else
				{
					Mouse.OverrideCursor = null;
					Mouse.RemovePreviewMouseDownHandler(this, MouseDownPenPreview);
				}
			}
		}

		protected void AddShape(object sender, ExecutedRoutedEventArgs args)
		{
			log.Debug("User commands: add shape");
			AddShapeState = true;
		}

		protected void MouseDownPenPreview(object sender, MouseButtonEventArgs args)
		{
			log.Debug(MethodBase.GetCurrentMethod());
			if (args.ChangedButton == MouseButton.Right)
				AddShapeState = false;
		}

		protected void CanvasMouseDown(object sender, MouseButtonEventArgs args)
		{
			if (args.ChangedButton == MouseButton.Left)
				UserAddItem(args);
		}

		void UserAddItem(MouseButtonEventArgs args)
		{
			var nodeControl = new NodeControl();
			Canvas.Children.Add(nodeControl);
			Canvas.SetLeft(nodeControl, args.GetPosition(Canvas).X);
			Canvas.SetTop(nodeControl, args.GetPosition(Canvas).Y);
		}

	}

}
