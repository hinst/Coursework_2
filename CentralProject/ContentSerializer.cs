using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Runtime.Serialization;

using NLog;

using MyCSharp;

namespace Coursework_2
{

	internal class ContentSerializer
	{

		protected Logger log = LogManager.GetCurrentClassLogger();

		protected ObjectIDGenerator objectIdGenerator = new ObjectIDGenerator();

		public ContentSerializer Create(Canvas canvas)
		{
			theCanvas = canvas;
			return this;
		}

		protected Canvas theCanvas;

		public Canvas TheCanvas
		{
			get
			{
				return theCanvas;
			}
		}

		public void SaveToFile(string fileName)
		{
			log.Debug("Now saving '" + fileName + "'...");
			ContentToXml().Save(fileName);
		}

		protected XDocument ContentToXml()
		{
			var result = new XDocument();
			result.Add(ContentToElement());
			return result;
		}

		protected XElement ContentToElement()
		{
			var element = new XElement(ContentElementName);
			WriteNodes(element);
			WriteLinks(element);
			return element;
		}

		protected void GenerateIds()
		{
			ForEach.MatchingType<DependencyObject>(
				TheCanvas.Children,
				(element) => 
					element.SetValue(IdProperty, GetNewId(element))
			);
		}

		#region nodes

		protected void WriteNodes(XElement element)
		{
			Assert.Assigned(element);
			ForEach.MatchingType<NodeControl>(
				TheCanvas.Children,
				(node) => WriteNode(element, node)
			);
		}

		protected long GetNewId(DependencyObject control)
		{
			bool firstTime;
			var id = objectIdGenerator.GetId(control, out firstTime);
			return (long)id;
		}

		protected long GetExistingId(DependencyObject control)
		{
			var id = (long)control.GetValue(IdProperty);
			return id;
		}

		protected void WriteNode(XElement elements, NodeControl node)
		{
			var element = node.SaveToElement();
			node.SetValue(IdProperty, GetNewId(node));
			element.SetIdAttribute(GetExistingId(node));
			elements.Add(element);
		}

		#endregion

		#region links

		protected void WriteLinks(XElement element)
		{
			Assert.Assigned(element);
			ForEach.MatchingType<Line>(
				TheCanvas.Children,
				(line) => WriteLine(element, line)
			);
		}

		protected void WriteLine(XElement elements, Line line)
		{
			var link = line.GetValue(LinkControl.LinkControlProperty) as LinkControl;
			WriteLink(elements, link);
		}

		protected void WriteLink(XElement elements, LinkControl link)
		{
			Assert.Assigned(link);
			var element = link.SaveToElement();
			link.SetValue(IdProperty, GetNewId(link));
			element.SetIdAttribute(GetExistingId(link));
			elements.Add(element);
		}

		#endregion

		public void LoadFromFile(string fileName)
		{
			var document = XDocument.Load(fileName);
			LoadContentFrom(document);
		}

		protected void LoadContentFrom(XDocument document)
		{
			var contentElement = document.Element(ContentElementName);
			Assert.Assigned(contentElement);
			ReadNodes(contentElement);
			ReadLinks(contentElement);
		}

		protected void ContentFromElement(XElement element)
		{
			Assert.Assigned(element);
			ReadNodes(element);
		}

		public const string ContentElementName = "content";

		public const string IdAttributeName = "id";

		public static readonly DependencyProperty IdProperty = DependencyProperty.RegisterAttached("Id", typeof(long), typeof(ContentSerializer));

		protected void ReadNodes(XElement contentElement)
		{
			foreach (XNode node in contentElement.Nodes())
			{
				var currentElement = node as XElement;
				if (currentElement != null)
					if (currentElement.Name == typeof(NodeControl).Name)
						ReadNode(currentElement);
			}
		}

		protected void ReadNode(XElement element)
		{
			var node = NodeControl.Create(element);
			node.SetValue(IdProperty, element.GetIdAttribute());
			TheCanvas.Children.Add(node);
		}

		protected void ReadLinks(XElement links)
		{
			foreach (XNode node in links.Nodes())
			{
				var element = node as XElement;
				if (element != null)
					if (element.Name == typeof(LinkControl).Name)
						ReadLink(element);
			}
		}

		protected void ReadLink(XElement element)
		{
			var link = new LinkControl().Create(element, TheCanvas);
			link.SetValue(IdProperty, element.GetIdAttribute());
			TheCanvas.Children.Add(link.TheLine);
		}

	}


	static class ContentSerializerIdAttributeHelper
	{

		public static long GetIdAttribute(this XElement element)
		{
			return long.Parse(element.Attribute(ContentSerializer.IdAttributeName).Value);
		}

		public static void SetIdAttribute(this XElement element, long id)
		{
			element.SetAttributeValue(ContentSerializer.IdAttributeName, id);
		}

	}

}

