using System;
using System.Windows.Controls;
using System.Windows.Media;

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
							var result = PresentationApplication.Current.Resources["Images_NodeShape"] as Image;
							log.Debug(result);
							return result;
						}
					);
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


	}

}
