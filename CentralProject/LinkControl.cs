using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;

using NLog;

using MyCSharp;

namespace Coursework_2
{

	internal class LinkControl : FrameworkElement
	{

		protected Logger log = LogManager.GetCurrentClassLogger();

		public LinkControl Create(Tuple<NodeControl, NodeControl> linkedNodes)
		{
			var result = new LinkControl();
			result.linkedNodes = linkedNodes;
			return result;
		}

		protected Tuple<NodeControl, NodeControl> linkedNodes;

		public Tuple<NodeControl, NodeControl> LinkedNodes
		{
			get
			{
				return linkedNodes;
			}
		}

		protected Line linkLine;

		public Line LinkLine
		{
			get
			{
				return AutoCreateField.Get( ref linkLine, () => new Line() );
			}
		}

		public void BindFirstPoint(NodeControl control)
		{
			var binding = new Binding();
			binding.Source = control;
			binding.Path = new PropertyPath("NodeShape.Left");
			LinkLine.SetBinding(Line.X1Property, binding);
		}

	}

}
