using System;
using System.Windows.Controls;

using MyWPF;

namespace Coursework_2
{

	public class RemoveLinkMenuItem : MenuItem
	{

		protected LinkControl link;

		public RemoveLinkMenuItem Create(LinkControl link, NodeControl toNode)
		{
			Header = "Remove link to: " + toNode.Caption.Text;
			link.RemoveAssociatedElements += Remove;
			this.link = link;
			return this;
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
