using System;
using System.Windows.Controls;
using System.Windows.Media;

using MyCSharp;

namespace Coursework_2
{

	public class NodeControl : Border
	{

		public NodeControl(string text = DefaultText)
			: base()
		{
			InitializeTextBlock(text);
		}

		protected TextBlock textBlock;

		protected void InitializeTextBlock(string text)
		{
			textBlock = new TextBlock();
			textBlock.Text = text;
			AddLogicalChild(textBlock);
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
			BorderBrush = new SolidColorBrush(DefaultBrushColor);
		}

		public const string DefaultText = "no caption";

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
