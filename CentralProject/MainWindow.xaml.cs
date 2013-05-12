using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
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
			BindCommand(GenerateLargeDocumentMenuItem, Commands.TestGenerateLargeDocument, UserTestGenerateLargeDocument);
			BindCommand(GenerateAndSaveLargeDocumentMenuItem, Commands.TestGenerateSaveLargeDocuement, TestGenerateAndSaveLargeDocument);
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

		protected void UserAddItem(Point nodePosition, Canvas canvas)
		{
			var nodeControl = new NodeControl();
			if (nodeControl.UserRename(this))
			{
				canvas.Children.Add(nodeControl);
				Canvas.SetLeft(nodeControl, nodePosition.X);
				Canvas.SetTop(nodeControl, nodePosition.Y);
			}
		}

		protected void UserAddLink(object sender, ExecutedRoutedEventArgs args)
		{
			new MouseDrop<NodeControl>().Create(Cursors.Pen, AddLink).Drop(Canvas);
		}

		protected void UserTestGenerateLargeDocument(object sender, ExecutedRoutedEventArgs args)
		{
			var countText = TextEditWindow.Perform(this, "Generate test document", "Enter the count of nodes & links", "1000");
			if (countText != null)
			{
				var count = int.Parse(countText);
				TestGenerateLargeDocument(count, count);
			}
		}

		protected void TestGenerateLargeDocument(int count, int countOfLinks)
		{
			ClearContent();
			log.Debug(() => "Now generating document: count of nodes: " + count + "; count of links: " + countOfLinks);
			var nodes = new List<NodeControl>(count);
			var random = new Random();
			for (int i = 0; i < count; ++i)
			{
				//log.Debug(() => "Node #" + i + "...");
				var node = new NodeControl(i.ToString());
				nodes.Add(node);
				Canvas.Children.Add(node);
				node.UpdateLayout();
				Canvas.SetLeft(node, random.NextDouble() * (Canvas.Width - node.ActualWidth));
				Canvas.SetTop(node, random.NextDouble() * (Canvas.Height - node.ActualHeight));
			}
			for (int i = 0; i < countOfLinks; ++i)
			{
				//log.Debug(() => "Link #" + i + "...");
				var index1 = random.Next(count);
				var index2 = random.Next(count);
				if (index1 != index2)
				{
					var link = new LinkControl().Create();
					var node1 = nodes[index1];
					var node2 = nodes[index2];
					Canvas.Children.Add(link.TheLine);
					link.BindNode1(node1);
					link.BindNode2(node2);
				}
			}
		}

		protected void TestGenerateAndSaveLargeDocument(object sender, ExecutedRoutedEventArgs args)
		{
			const int count = 1700;
			var stopWatch = Stopwatch.StartNew();
			TestGenerateLargeDocument(count, count);
			stopWatch.Stop();
			log.Debug(() => "Generate large document: time: " + stopWatch.ElapsedMilliseconds);
			stopWatch.Start();
			UserSaveFile("test.xml");
			stopWatch.Stop();
			log.Debug(() => "Generate & save: total time: " + stopWatch.ElapsedMilliseconds);
		}

		protected void AddLink(Point linkPosition, NodeControl nodeControl)
		{
			log.Debug("Now adding user link, first link control is: " + nodeControl.Caption.Text);
			var linkControl = new LinkControl().Create();
			linkControl.BindNode1(nodeControl);
			linkControl.BindPoint2MouseMove(Canvas);
			Canvas.Children.Add(linkControl.TheLine);
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
			link.BindNode2(nodes.Item2);
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
			var stopWatch = Stopwatch.StartNew();
			new ContentSerializer().Create(Canvas).SaveToFile(fileName);
			stopWatch.Stop();
			log.Debug(() => "Saving time: " + stopWatch.ElapsedMilliseconds);
		}

		protected const string ContentFileExtension = ".xml";

		protected void ClearContent()
		{
			log.Debug("Now clearing content...");
			Canvas.Children.Clear();
		}

		protected void UserOpenFile(object sender, ExecutedRoutedEventArgs args)
		{
			var openFileDialog = new Microsoft.Win32.OpenFileDialog();
			DocumentFileName = null;
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
				UserLoadFile(fileName);
			else
				MessageBox.Show(this, fileName, "File does not exist", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		protected void UserLoadFile(string fileName)
		{
			var stopWatch = Stopwatch.StartNew(); // начать отсчёт
			ClearContent(); // очистить документ
			new ContentSerializer().Create(Canvas).LoadFromFile(fileName); // загрузить документ
			stopWatch.Stop(); // остановить отчёт
			log.Debug(() => "Loading time: " + stopWatch.ElapsedMilliseconds); // вывести результат
		}

	}

}




