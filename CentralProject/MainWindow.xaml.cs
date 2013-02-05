using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

using NLog;

using MyCSharp;

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

		protected PresentationApplicationCommands Commands
		{
			get
			{
				return PresentationApplication.Current.Commands;
			}
		}

		protected void BindCommands()
		{
			BindCommand(OpenFileMenuItem, Commands.OpenFile, UserOpenFile);
			BindCommand(SaveFileMenuItem, Commands.SaveFile, UserSaveFile);
			BindCommand(AddItemMenuItem, Commands.AddShape, UserAddItem);
		}

		protected void BindCommand(MenuItem item, RoutedUICommand command, ExecutedRoutedEventHandler handler)
		{
			CommandManager.RegisterClassCommandBinding(
				this.GetType(),
				new CommandBinding(command, handler)
			);
			item.Command = command;
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
					Canvas.Cursor = Cursors.Pen;
					Mouse.AddPreviewMouseDownHandler(this, MouseDownPenPreview);
					Canvas.MouseDown += CanvasDropItemMouseDown;
				}
				else
				{
					Canvas.Cursor = null;
					Mouse.RemovePreviewMouseDownHandler(this, MouseDownPenPreview);
					Canvas.MouseDown -= CanvasDropItemMouseDown;
				}
			}
		}

		protected void UserAddItem(object sender, ExecutedRoutedEventArgs args)
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

		protected const bool LogDebugUserAddItemNewNodePosition = false;

		protected void UserAddItem(MouseButtonEventArgs args)
		{
			Point nodePosition =
				new Point(
					args.GetPosition(Canvas).X,
					args.GetPosition(Canvas).Y
				);
			if (LogDebugUserAddItemNewNodePosition)
				#pragma warning disable 162
				log.Debug("new node position is: " + nodePosition);
				#pragma warning restore 162
			var nodeControl = new NodeControl();
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

		protected string documentFileName;

		protected string DocumentFileName
		{
			get
			{
				return documentFileName;
			}
			set
			{
				documentFileName = value;
			}
		}

		#region Save content

		protected void UserSaveFile(object sender, ExecutedRoutedEventArgs args)
		{
			log.Debug("[user_command incoming]  Save file...");
			if (DocumentFileName == null)
				UserSaveFileAs(sender, args);
		}

		protected void UserSaveFileAs(object sender, ExecutedRoutedEventArgs args)
		{
			var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
			saveFileDialog.DefaultExt = ContentFileExtension;
			var result = saveFileDialog.ShowDialog(this);
			if (result == true)
			{
				var fileName = saveFileDialog.FileName;
				UserSaveFile(fileName);
				DocumentFileName = fileName;
			}
		}

		protected void UserSaveFile(string fileName)
		{
			log.Debug("Now saving '" + fileName + "'...");
			ContentToXml().Save(fileName);
		}

		protected XDocument ContentToXml()
		{
			var result = new XDocument();
			result.Add(ContentToElement());
			return result;
		}

		protected XElement ContentToElement()
		{
			var element = new XElement(XmlContentElementName);
			WriteNodes(element);
			return element;
		}

		protected void WriteNodes(XElement element)
		{
			Assert.Assigned(element);
			Assert.Assigned(Canvas); // nodes are being searched on the Canvas
			ForEach.MatchingType<NodeControl>(
				Canvas.Children,
				(control) =>
					element.Add(control.SaveToElement())
			);
		}

		protected void ContentFromElement(XElement element)
		{
			Assert.Assigned(element);
			LoadNodes(element);
		}

		#endregion Save content

		protected const string ContentFileExtension = ".xml";

		public const string XmlContentElementName = "content";

		#region Load content

		protected void UserOpenFile(object sender, ExecutedRoutedEventArgs args)
		{
			log.Debug("[user_command incoming] Open file...");
			var openFileDialog = new Microsoft.Win32.OpenFileDialog();
			openFileDialog.DefaultExt = ContentFileExtension;
			var result = openFileDialog.ShowDialog();
			if (result == true)
			{
				var fileName = openFileDialog.FileName;
				if (File.Exists(fileName))
					UserOpenFile(fileName);
				else
					MessageBox.Show(this, fileName, "File does not exist", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		protected void UserOpenFile(string fileName)
		{
			var document = XDocument.Load(fileName);
			LoadContentFrom(document);
		}

		protected void LoadContentFrom(XDocument document)
		{
			var contentElement = document.Element(XmlContentElementName);
			Assert.Assigned(contentElement);
			LoadNodes(contentElement);
		}

		protected void LoadNodes(XElement contentElement)
		{
			foreach (XNode node in contentElement.Nodes())
			{
				var currentElement = node as XElement;
				if (currentElement != null)
				{
					if (currentElement.Name == typeof(NodeControl).Name)
					{
						var visualNode = NodeControl.Create(currentElement);
						Canvas.Children.Add(visualNode);
					}
				}
			}
		}

		#endregion

	}

}




