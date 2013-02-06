using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

using NLog;

namespace Coursework_2
{

	internal class CanvasNodeLink : FrameworkElement
	{

		protected Logger log = LogManager.GetCurrentClassLogger();

		protected CanvasNodeLink Create(Tuple<NodeControl, NodeControl> linkedNodes)
		{
			var result = new CanvasNodeLink();
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
				return linkLine;
			}
		}

	}

}
