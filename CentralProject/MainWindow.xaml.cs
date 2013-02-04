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
					Canvas.MouseDown += CanvasDropItemMouseDown;
				}
				else
				{
					Mouse.OverrideCursor = null;
					Mouse.RemovePreviewMouseDownHandler(this, MouseDownPenPreview);
					Canvas.MouseDown -= CanvasDropItemMouseDown;
				}
			}
		}

		protected void AddShape(object sender, ExecutedRoutedEventArgs args)
		{
			AddShapeState = true;
		}

		protected void MouseDownPenPreview(object sender, MouseButtonEventArgs args)
		{
			log.Debug(MethodBase.GetCurrentMethod());
			if (args.ChangedButton == MouseButton.Right)
				AddShapeState = false;
		}

		protected void CanvasDropItemMouseDown(object sender, MouseButtonEventArgs args)
		{
			if (args.ChangedButton == MouseButton.Left)
			{
				UserAddItem(args);
				AddShapeState = false;
			}
		}

		protected const bool TestAddShapeEnabled = false;

		protected const bool UserAddItemAddsTestShape = false;

		protected Style NodeControlStyle
		{
			get
			{
				return Resources["NodeBorderStyle"] as Style;
			}
		}

		protected void UserAddItem(MouseButtonEventArgs args)
		{
			Point nodePosition =
				new Point(
					args.GetPosition(Canvas).X,
					args.GetPosition(Canvas).Y
				);
			log.Debug("new node position is: " + nodePosition);
			var nodeControl = new NodeControl();
			nodeControl.Style = NodeControlStyle;
			Canvas.Children.Add(nodeControl);
			Canvas.SetLeft(nodeControl, nodePosition.X);
			Canvas.SetTop(nodeControl, nodePosition.Y);
		}

		protected void TestAddShape()
		{
			log.Debug(MethodBase.GetCurrentMethod().Name + "...");
			var rectangle = CreateTestingShape();
			Canvas.Children.Add(rectangle);
			Canvas.SetLeft(rectangle, 100);
			Canvas.SetTop(rectangle, 100);
		}

		protected Shape CreateTestingShape()
		{
			var result = new Rectangle();
			result.Width = 60;
			result.Height = 60;
			result.Fill = Brushes.Black;
			return result;
		}

	}

}
