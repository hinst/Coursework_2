using System;
using System.Windows.Controls;

namespace Coursework_2
{

	public class RemoveLinkMenuItem : MenuItem
	{

		protected LinkControl link;

		public RemoveLinkMenuItem Create(LinkControl link, NodeControl toNode)
		{
			Header = "Remove link to: " + toNode.Caption.Text;
			this.link = link;
			return this;
		}

		protected override void OnClick()
		{
			
		}

	}

}
