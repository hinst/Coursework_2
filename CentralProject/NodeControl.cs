using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

using NLog;

using MyCSharp;

namespace Coursework_2
{

	public class NodeControl : Border
	{

		public NodeControl(string text = DefaultText)
			: base()
		{
			InitializeTextBlock(text);
			Child = CreateContent();
			Initialize();
		}

		public static NodeControl Create(XElement element)
		{
			var result = new NodeControl();
			result.LoadFromElement(element);
			return result;
		}

		public const string DefaultText = "no caption";

		protected Logger log = LogManager.GetCurrentClassLogger();

		protected TextBlock textBlock;

		protected void InitializeTextBlock(string text)
		{
			textBlock = new TextBlock();
			textBlock.Text = text;
		}

		protected StackPanel CreateContent()
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			result.Children.Add(NodeShape);
			result.Children.Add(textBlock);
			ForEach.MatchingType<FrameworkElement>(
				result.Children,
				element => { element.Margin = new Thickness(3); }
			);
			return result;
		}

		protected Image nodeShape;

		protected Image NodeShape
		{
			get
			{
				return 
					AutoCreateField.Get(
						ref nodeShape, 
						() => 
						{
							var result = PresentationApplication.Current.Resources["Image_NodeShape"] as Image;
							return result;
						}
					);
			}
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

		protected void Initialize()
		{
			Background = Brushes.Transparent;
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
			Canvas.SetLeft(this, int.Parse(element.Attribute(LeftAttributeName).Value));
			Canvas.SetTop(this, int.Parse(element.Attribute(TopAttributeName).Value));
		}

	}

}
