using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire.Commands
{
	public class ExampleCommand : Command
	{
		public override string Name
		{
			get { return "test"; }
		}
		public override string[] Accessors
		{
			get { return new string[1] { "test" }; }
		}
		public override void UseByPlayer(Player p, string FullCommand)
		{
			p.SendMessage("Hello " + p.name);
		}
		public override void HelpPlayer(Player p, string FullCommand)
		{
			p.SendMessage("Since this is just an example command, stfu and start coding!");
		}
	}
}
