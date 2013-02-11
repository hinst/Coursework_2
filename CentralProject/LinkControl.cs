using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Reflection;
using System.Xml.Linq;

using NLog;

using MyCSharp;
using MyWPF;

namespace Coursework_2
{

	public class LinkControl : DependencyObject
	{

		protected static DependencyProperty reverseProperty;

		public static DependencyProperty LinkControlProperty 
		{
			get
			{
				return AutoCreateField.Get(ref reverseProperty, () => DependencyProperty.RegisterAttached( "LinkControl", typeof(LinkControl), typeof(LinkControl) ));
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
			result.SetValue(LinkControlProperty, this);
			result.IsHitTestVisible = false;
			return result;
		}

		public void DetachLineProperty()
		{
			TheLine.SetValue(LinkControlProperty, null);
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
				return AutoCreateField.Get(ref linkedNodes, () => new Tuple<NodeControl, NodeControl>(null, null));
			}
			set
			{
				linkedNodes = value;
			}
		}

		public event Action RemoveAssociatedElements;

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

		protected void BindNode(DependencyProperty xProperty, DependencyProperty yProperty, NodeControl control)
		{
			control.UpdateLinkPointProperty();
			BindCoordinate(xProperty, "LinkPointX", control);
			BindCoordinate(yProperty, "LinkPointY", control);
		}

		public void BindNode1(NodeControl control)
		{
			BindNode(Line.X1Property, Line.Y1Property, control);
			LinkedNodes = new Tuple<NodeControl, NodeControl>(control, LinkedNodes.Item2);
			EnsureNodesHasRemoveLinkContextMenu();
		}

		public void BindNode2(NodeControl control)
		{
			BindNode(Line.X2Property, Line.Y2Property, control);
			LinkedNodes = new Tuple<NodeControl, NodeControl>(LinkedNodes.Item1, control);
			EnsureNodesHasRemoveLinkContextMenu();
		}

		public void EnsureNodesHasRemoveLinkContextMenu()
		{
			if (LinkedNodes.Item1 != null && LinkedNodes.Item2 != null)
			{
				LinkedNodes.Item1.AddRemoveLinkContextMenuItem(this);
				LinkedNodes.Item2.AddRemoveLinkContextMenuItem(this);
			}
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
			RemoveAssociatedElements();
			DetachLineProperty();
			var parentCanvas = ParentCanvas;
			if (parentCanvas != null)
				ParentCanvas.Children.Remove(TheLine);
		}

		#region serialization

		protected const string XmlNode1AttributeName = "Node1";
		protected const string XmlNode2AttributeName = "Node2";

		public XElement SaveToElement()
		{
			var result = new XElement(GetType().Name);
			result.SetAttributeValue(XmlNode1AttributeName, LinkedNodes.Item1.GetValue(ContentSerializer.IdProperty));
			result.SetAttributeValue(XmlNode2AttributeName, LinkedNodes.Item2.GetValue(ContentSerializer.IdProperty));
			return result;
		}

		public LinkControl Create(XElement element, Canvas canvas)
		{
			var node1_ID = int.Parse(element.Attribute(XmlNode1AttributeName).Value);
			var node2_ID = int.Parse(element.Attribute(XmlNode2AttributeName).Value);
			Create();
			var node1 = NodeControl.FindBySerialId(node1_ID, canvas);
			var node2 = NodeControl.FindBySerialId(node2_ID, canvas);
			BindNode1(node1);
			BindNode2(node2);
			return this;
		}

		#endregion

	}

}
