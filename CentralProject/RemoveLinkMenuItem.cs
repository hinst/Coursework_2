using System;
using System.Windows.Data;
using System.Windows.Controls;

using MyWPF;

namespace Coursework_2
{

	public class RemoveLinkMenuItem : MenuItem
	{

		protected LinkControl link;

		public RemoveLinkMenuItem Create(LinkControl link, NodeControl toNode)
		{
			BindOppositeNodeCaptionTextChangeNotifier(toNode);
			SetHeader(toNode.Caption.Text);
			link.RemoveAssociatedElements += Remove;
			this.link = link;
			return this;
		}

		protected PropertyChangeNotifier oppositeNodeCaptionTextChangeNotifier;

		protected void BindOppositeNodeCaptionTextChangeNotifier(NodeControl toNode)
		{
			oppositeNodeCaptionTextChangeNotifier = new PropertyChangeNotifier(toNode.Caption, "Text");
			oppositeNodeCaptionTextChangeNotifier.ValueChanged += 
				delegate(object sender, EventArgs e)
				{
					oppositeNodeCaptionTextChangeHandler();
				};
		}

		protected void oppositeNodeCaptionTextChangeHandler()
		{
			SetHeader(oppositeNodeCaptionTextChangeNotifier.Value as string);
		}

		protected void SetHeader(string oppositeNodeCaptionText)
		{
			Header = "Remove link to: " + oppositeNodeCaptionText;
		}

		protected void Remove()
		{
			var menu = this.NavigateUp<ContextMenu>();
			menu.Items.Remove(this);
		}

		protected override void OnClick()
		{
			link.Remove();
		}

	}

}
