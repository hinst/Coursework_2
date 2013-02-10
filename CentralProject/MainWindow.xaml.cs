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
			BindCommand(SaveAsFileMenuItem, Commands.SaveAsFile, UserSaveAsFile);
			BindCommand(AddItemMenuItem, Commands.AddShape, UserAddItem);
			BindCommand(AddLinkMenuItem, Commands.DrawLink, UserAddLink);
		}

		protected void BindCommand(MenuItem item, RoutedUICommand command, ExecutedRoutedEventHandler handler)
		{
			CommandManager.RegisterClassCommandBinding(
				this.GetType(),
				new CommandBinding(command, handler)
			);
			item.Command = command;
		}

		protected void UserAddItem(object sender, ExecutedRoutedEventArgs args)
		{
			new MouseDrop<Canvas>().Create(Cursors.Pen, UserAddItem).Drop(Canvas);
		}

		protected void UserAddLink(object sender, ExecutedRoutedEventArgs args)
		{
			new MouseDrop<NodeControl>().Create(Cursors.Pen, UserAddLink).Drop(Canvas);
		}

		protected void UserAddItem(Point nodePosition, Canvas canvas)
		{
			var nodeControl = new NodeControl();
			canvas.Children.Add(nodeControl);
			Canvas.SetLeft(nodeControl, nodePosition.X);
			Canvas.SetTop(nodeControl, nodePosition.Y);
		}

		protected void UserAddLink(Point linkPosition, NodeControl control)
		{
			log.Debug("Now adding user link, first link point is: " + control.LinkPoint);
			var linkControl = new LinkControl().Create(new Tuple<NodeControl, NodeControl>(control, null));
			linkControl.BindPoint2MouseMove(Canvas);
			Canvas.Children.Add(linkControl.TheLine);
		}

		protected void TestAddShape()
		{
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
			if (DocumentFileName == null)
				UserSaveAsFile(sender, args);
			else
				UserSaveFile(DocumentFileName);
		}

		protected void UserSaveAsFile(object sender, ExecutedRoutedEventArgs args)
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

		protected void ClearContent()
		{
			Canvas.Children.Clear();
		}

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
				UserOpenFileSafe(fileName);
			}
		}

		protected void UserOpenFileSafe(string fileName)
		{
			if (File.Exists(fileName))
				UserOpenFile(fileName);
			else
				MessageBox.Show(this, fileName, "File does not exist", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		protected void UserOpenFile(string fileName)
		{
			var document = XDocument.Load(fileName);
			LoadContentFrom(document);
		}

		protected void LoadContentFrom(XDocument document)
		{
			ClearContent();
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
					if (currentElement.Name == typeof(NodeControl).Name)
						LoadNodeControl(currentElement);
			}
		}

		protected void LoadNodeControl(XElement nodeElement)
		{
			var visualNode = NodeControl.Create(nodeElement);
			Canvas.Children.Add(visualNode);
		}

		#endregion

	}

}




