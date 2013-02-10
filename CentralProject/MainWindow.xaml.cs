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

using VisualInteraction = Microsoft.VisualBasic.Interaction;

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

		protected const string NodeControlNameText = "Node name:";

		protected const string NewNodeControlInputBoxTitle = "New node";

		protected void UserAddItem(Point nodePosition, Canvas canvas)
		{
			var nodeControl = new NodeControl();
			nodeControl.Caption.Text =
				VisualInteraction.InputBox(NodeControlNameText, NewNodeControlInputBoxTitle, nodeControl.Caption.Text);
			canvas.Children.Add(nodeControl);
			Canvas.SetLeft(nodeControl, nodePosition.X);
			Canvas.SetTop(nodeControl, nodePosition.Y);
		}

		protected void UserAddLink(object sender, ExecutedRoutedEventArgs args)
		{
			new MouseDrop<NodeControl>().Create(Cursors.Pen, AddLink).Drop(Canvas);
		}

		protected void AddLink(Point linkPosition, NodeControl nodeControl)
		{
			log.Debug("Now adding user link, first link control is: " + nodeControl.Caption.Text);
			var linkControl = new LinkControl().Create();
			linkControl.BindPoint1(nodeControl);
			linkControl.BindPoint2MouseMove(Canvas);
			Canvas.Children.Add(linkControl.TheLine);
			Canvas.SetZIndex(linkControl.TheLine, -1);
			new MouseDrop<NodeControl>().Create(Cursors.Pen,
				(Point linkPosition2, NodeControl nodeControl2) => 
					AddLinkFinal(linkControl, new Tuple<NodeControl, NodeControl>(nodeControl, nodeControl2)),
				() => AddLinkCancel(linkControl)
			).Drop(Canvas);
		}

		protected void AddLinkCancel(LinkControl link)
		{
			log.Debug("Now aborting user link...");
			link.Remove();		
		}

		protected void AddLinkFinal(LinkControl link, Tuple<NodeControl, NodeControl> nodes)
		{
			log.Debug("Now linking: '{0}' & '{1}'", nodes.Item1.Caption.Text, nodes.Item2.Caption.Text);
			link.BindPoint2(nodes.Item2);
			link.LinkedNodes = new Tuple<NodeControl, NodeControl>(nodes.Item1, nodes.Item2);
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




