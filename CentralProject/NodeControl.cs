using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using System.Collections.Generic;

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
			InitializePositionPropertiesChangedNotification();
		}

		public static readonly DependencyProperty LinkPointXProperty = DependencyProperty.Register("LinkPointX", typeof(double), typeof(NodeControl));

		public static readonly DependencyProperty LinkPointYProperty = DependencyProperty.Register("LinkPointY", typeof(double), typeof(NodeControl));

		public double LinkPointX
		{
			get
			{
				return (double) this.GetValue(LinkPointXProperty);
			}
		}

		public double LinkPointY
		{
			get
			{
				return (double) this.GetValue(LinkPointYProperty);
			}
		}

		public Point LinkPoint
		{
			get
			{
				return new Point(LinkPointX, LinkPointY);
			}
		}

		protected List<PropertyChangeNotifier> propertyChangeNotifiers;

		protected List<PropertyChangeNotifier> PropertyChangeNotifiers
		{
			get
			{
				return AutoCreateField.Get(ref propertyChangeNotifiers, () => new List<PropertyChangeNotifier>());
			}
		}

		protected void InitializePositionPropertiesChangedNotification()
		{
			var left = new PropertyChangeNotifier(this, Canvas.LeftProperty);
			left.ValueChanged += LeftPropertyChangedHandler;
			PropertyChangeNotifiers.Add(left);

			var top = new PropertyChangeNotifier(this, Canvas.TopProperty);
			top.ValueChanged += TopPropertyChangedHandler;
			PropertyChangeNotifiers.Add(top);
		}

		protected Point RelativeThingCenter
		{
			get
			{
				return new Point(Thing.ActualWidth / 2, Thing.ActualHeight / 2);
			}
		}

		protected bool LogDebugLinkPointPropertyChangedHandler { get { return false; } }

		protected void LeftPropertyChangedHandler(object sender, EventArgs e)
		{
			ParentCanvas.UpdateLayout();
			var left = Canvas.GetLeft(this);
			double linkPointX = -1;
			for (int i = 0; i < 3; ++i)
				linkPointX = Thing.TranslatePoint(RelativeThingCenter, ParentCanvas).X;
			SetValue(LinkPointXProperty, linkPointX);
			if (LogDebugLinkPointPropertyChangedHandler)
				log.Debug("LinPointXProperty assigned to: " + linkPointX + "; left is: " + left + "; rtc is: " + RelativeThingCenter);
		}

		protected void TopPropertyChangedHandler(object sender, EventArgs e)
		{
			ParentCanvas.UpdateLayout();
			var top = Canvas.GetTop(this);
			double linkPointY = -1;
			for (int i = 0; i < 3; ++i)
				linkPointY = Thing.TranslatePoint(RelativeThingCenter, ParentCanvas).Y;
			SetValue(LinkPointYProperty, linkPointY);
			if (LogDebugLinkPointPropertyChangedHandler)
				log.Debug("LinPointYProperty assigned to: " + linkPointY + "; top is: " + top + "; rtc is: " + RelativeThingCenter);
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

		public StackPanel Panel
		{
			get
			{
				return panel;
			}
		}

		protected TextBlock caption;

		public TextBlock Caption
		{
			get
			{
				return caption;
			}
		}

		protected Image thing;

		public Image Thing
		{
			get
			{
				return thing;
			}
		}

		protected void InitializeThing()
		{
			thing = CreateThing();
			Thing.Cursor = Cursors.SizeAll;
		}

		protected static Brush defaultCaptionBackgroundBrush;

		protected static Brush DefaultCaptionBackgroundBrush
		{
			get
			{
				return
					AutoCreateField.Get(
						ref defaultCaptionBackgroundBrush,
						() =>
						{
							var color = Colors.White;
							color.A = byte.MaxValue / 3 * 2;
							return new SolidColorBrush(color);
						}
					);
			}
		}

		protected void InitializeCaption(string text)
		{
			caption = new TextBlock();
			caption.Background = DefaultCaptionBackgroundBrush;
			caption.Text = text;
		}

		protected StackPanel CreatePanel(string text)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			InitializeThing();
			result.Children.Add(thing);
			InitializeCaption(text);
			result.Children.Add(caption);
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
			MenuItem item = new MenuItem();
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

		protected MenuItem CreateRemoveLinkMenuItem(LinkControl link)
		{
			var opposite = link.LinkedNodes.Item1 == this ? link.LinkedNodes.Item1 : link.LinkedNodes.Item2;
			var item = new RemoveLinkMenuItem().Create(link, opposite);
			return item;
		}

		protected void AddRemoveLinkContextMenuItem(LinkControl link)
		{
			ContextMenu.Items.Add(CreateRemoveLinkMenuItem(link));
		}

		protected Image CreateThing()
		{
			var result = PresentationApplication.Current.Resources["Image_NodeShape"] as Image;
			return result;
		}

		protected Point PanelMouseRelativePosition;

		public Canvas ParentCanvas
		{
			get
			{
				return this.NavigateUp<Canvas>();
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
			if (args.LeftButton == MouseButtonState.Pressed && Mouse.DirectlyOver == thing && false == DraggingMoving)
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
			element.SetAttributeValue(TextAttributeName, Caption.Text);
			element.SetAttributeValue(LeftAttributeName, Canvas.GetLeft(this));
			element.SetAttributeValue(TopAttributeName, Canvas.GetTop(this));
		}

		public void LoadFromElement(XElement element)
		{
			Caption.Text = element.Attribute(TextAttributeName).Value;
			Canvas.SetLeft(this, double.Parse(element.Attribute(LeftAttributeName).Value));
			Canvas.SetTop(this, double.Parse(element.Attribute(TopAttributeName).Value));
		}

	}

}
