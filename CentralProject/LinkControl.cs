using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Reflection;

using NLog;

using MyCSharp;
using MyWPF;

namespace Coursework_2
{

	public class LinkControl
	{

		protected static DependencyProperty reverseProperty;

		public static DependencyProperty ReverseProperty 
		{
			get
			{
				return AutoCreateField.Get(ref reverseProperty, () => DependencyProperty.RegisterAttached( "Reverse", typeof(LinkControl), typeof(LinkControl) ));
			}
		}

		protected Logger log = LogManager.GetCurrentClassLogger();

		public LinkControl Create()
		{
			theLine = CreateLine();
			return this;
		}

		protected Line CreateLine()
		{
			var result = new Line();
			result.Stroke = DefaultLineBrush;
			result.SetValue(ReverseProperty, this);
			result.IsHitTestVisible = false;
			return result;
		}

		public void DetachLineProperty()
		{
			TheLine.SetValue(ReverseProperty, null);
		}

		protected static Brush defaultLineBrush;

		protected static Brush CreateDefaultLineBrush()
		{
			var color = new Color();
			color.A = byte.MaxValue / 3 * 2;
			color.R = 0;
			color.G = 0;
			color.B = 0;
			var result = new SolidColorBrush(color);
			return result;
		}

		protected static Brush DefaultLineBrush
		{
			get
			{
				return AutoCreateField.Get(ref defaultLineBrush, () => CreateDefaultLineBrush());
			}
		}

		protected Tuple<NodeControl, NodeControl> linkedNodes;

		public Tuple<NodeControl, NodeControl> LinkedNodes
		{
			get
			{
				return linkedNodes;
			}
			set
			{
				linkedNodes = value;
			}
		}

		protected Line theLine;

		public Line TheLine
		{
			get
			{
				return theLine;
			}
		}

		protected void BindCoordinate(DependencyProperty property, string path, NodeControl control)
		{
			var binding = new Binding(path);
			binding.Source = control;
			binding.Mode = BindingMode.OneWay;
			TheLine.SetBinding(property, binding);
		}

		protected void BindPoint(DependencyProperty xProperty, DependencyProperty yProperty, NodeControl control)
		{
			BindCoordinate(xProperty, "LinkPointX", control);
			BindCoordinate(yProperty, "LinkPointY", control);
		}

		public void BindPoint1(NodeControl control)
		{
			BindPoint(Line.X1Property, Line.Y1Property, control);
		}

		public void BindPoint2(NodeControl control)
		{
			BindPoint(Line.X2Property, Line.Y2Property, control);
		}

		/// <summary>
		/// Mouse move
		/// </summary>
		/// <param name="canvas"></param>
		public void BindPoint2MouseMove(Canvas canvas)
		{
			MouseEventHandler move = 
				delegate(object sender, MouseEventArgs e)
				{
					theLine.X2 = e.GetPosition(canvas).X;
					theLine.Y2 = e.GetPosition(canvas).Y;
				};
			canvas.PreviewMouseMove += move;
			MouseButtonEventHandler cancel = null;
			cancel = 
				delegate(object sender, MouseButtonEventArgs e)
				{
					canvas.PreviewMouseMove -= move;
					canvas.PreviewMouseDown -= cancel;
				};
			canvas.PreviewMouseDown += cancel;
		}

		public Canvas ParentCanvas
		{
			get
			{
				return TheLine.NavigateUp<Canvas>();
			}
		}

		public void Remove()
		{
			DetachLineProperty();
			var parentCanvas = ParentCanvas;
			if (parentCanvas != null)
				ParentCanvas.Children.Remove(TheLine);
		}

	}

}
