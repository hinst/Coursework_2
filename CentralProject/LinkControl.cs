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

	internal class LinkControl
	{

		protected Logger log = LogManager.GetCurrentClassLogger();

		public LinkControl Create(Tuple<NodeControl, NodeControl> linkedNodes)
		{
			theLine = CreateLine();
			this.linkedNodes = linkedNodes;
			if (linkedNodes.Item1 != null)
				BindPoint1(linkedNodes.Item1);
			if (linkedNodes.Item2 != null)
				BindPoint2(linkedNodes.Item2);
			return this;
		}

		protected Line CreateLine()
		{
			var result = new Line();
			result.Stroke = DefaultLineBrush;
			return result;
		}

		protected Brush DefaultLineBrush
		{
			get
			{
				return Brushes.Black;
			}
		}

		protected Tuple<NodeControl, NodeControl> linkedNodes;

		public Tuple<NodeControl, NodeControl> LinkedNodes
		{
			get
			{
				return linkedNodes;
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
			log.Debug("" + property + "," + path + "," + control);
			log.Debug(PropertyPathHelper.GetValue(control, path));
			var binding = new Binding(path);
			binding.Source = control;
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
			log.Debug(MethodBase.GetCurrentMethod() + "\n" + TheLine.X1 + "," + TheLine.Y1);
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

	}

}
