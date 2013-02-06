using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

using NLog;

using MyCSharp;

using MyWPF;

namespace Coursework_2
{

	public class NodeControl : Border
	{

		public NodeControl(string text = DefaultText)
			: base()
		{
			Initialize();
			InitializeContent(text);
		}

		protected void Initialize()
		{
			Background = Brushes.Transparent;
			MouseDown += MouseDownHandler;
			MouseMove += MouseMoveHandler;
		}

		protected void InitializeContent(string text)
		{
			panel = CreatePanel(text);
			Child = panel;
		}

		public static NodeControl Create(XElement element)
		{
			var result = new NodeControl();
			result.LoadFromElement(element);
			return result;
		}

		public const string DefaultText = "no caption";

		protected Logger log = LogManager.GetCurrentClassLogger();

		protected StackPanel panel;
		protected TextBlock textBlock;
		protected Image nodeShape;

		protected void InitializeNodeShape()
		{
			nodeShape = CreateNodeShape();
		}

		protected void InitializeTextBlock(string text)
		{
			textBlock = new TextBlock();
			textBlock.Text = text;
		}

		protected StackPanel CreatePanel(string text)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			InitializeNodeShape();
			result.Children.Add(nodeShape);
			InitializeTextBlock(text);
			result.Children.Add(textBlock);
			ForEach.MatchingType<FrameworkElement>(
				result.Children,
				element => element.Margin = new Thickness(3)
			);
			result.ContextMenu = CreateContextMenu();
			return result;
		}

		protected ContextMenu CreateContextMenu()
		{
			var menu = new ContextMenu();
			menu.Items.Add(CreateRemoveMenuItem());
			return menu;
		}

		protected MenuItem CreateRemoveMenuItem()
		{
			var item = new MenuItem();
			item.Header = "Remove item";
			item.Click += 
				delegate(object sender, RoutedEventArgs agrs) 
				{
					RemoveMe(this);
				};
			return item;
		}

		protected void RemoveMe(NodeControl me)
		{
			ParentCanvas.Children.Remove(me);
		}

		protected Image CreateNodeShape()
		{
			var result = PresentationApplication.Current.Resources["Image_NodeShape"] as Image;
			result.Cursor = Cursors.SizeAll;
			return result;
		}

		protected Point PanelMouseRelativePosition;

		public Canvas ParentCanvas
		{
			get
			{
				return this.LogicalNavigateUp<Canvas>();
			}
		}

		protected void MouseDownHandler(object sender, MouseEventArgs args)
		{
			PanelMouseRelativePosition = args.GetPosition(this);
		}

		protected static bool draggingMoving;

		protected bool DraggingMoving
		{
			set
			{
				if (draggingMoving != value)
				{
					draggingMoving = value;
					if (draggingMoving)
						ParentCanvas.MouseMove += DragMove;
					else
						ParentCanvas.MouseMove -= DragMove;
				}
			}
			get
			{
				return draggingMoving;
			}
		}

		protected void MouseMoveHandler(object sender, MouseEventArgs args)
		{
			if (args.LeftButton == MouseButtonState.Pressed && Mouse.DirectlyOver == nodeShape && false == DraggingMoving)
				DraggingMoving = true;
		}

		protected void DragMove(object sender, MouseEventArgs args)
		{
			DraggingMoving = args.LeftButton == MouseButtonState.Pressed;
			if (DraggingMoving)
			{
				Canvas.SetLeft(this, args.GetPosition(ParentCanvas).X - PanelMouseRelativePosition.X);
				Canvas.SetTop(this, args.GetPosition(ParentCanvas).Y - PanelMouseRelativePosition.Y);
			}
			else
				DraggingMoving = false;
		}

		protected Thickness defaultContentMargin;

		protected Thickness DefaultContentMargin
		{
			get
			{
				return AutoCreateField.Get(ref defaultContentMargin, () => new Thickness(1.5));
			}
		}

		protected Color DefaultBrushColor
		{
			get
			{
				return Colors.Black;
			}
		}

		public string Text
		{
			get
			{
				return textBlock != null ? textBlock.Text : null;
			}
			set
			{
				if (textBlock != null)
					textBlock.Text = value;
			}
		}

		protected const string TextAttributeName = "Text";
		protected const string LeftAttributeName = "Left";
		protected const string TopAttributeName = "Top";

		public XElement SaveToElement()
		{
			var result = new XElement(GetType().Name);
			SaveToElement(result);
			return result;
		}

		public void SaveToElement(XElement element)
		{
			element.SetAttributeValue(TextAttributeName, Text);
			element.SetAttributeValue(LeftAttributeName, Canvas.GetLeft(this));
			element.SetAttributeValue(TopAttributeName, Canvas.GetTop(this));
		}

		public void LoadFromElement(XElement element)
		{
			Text = element.Attribute(TextAttributeName).Value;
			Canvas.SetLeft(this, double.Parse(element.Attribute(LeftAttributeName).Value));
			Canvas.SetTop(this, double.Parse(element.Attribute(TopAttributeName).Value));
		}

	}

}
