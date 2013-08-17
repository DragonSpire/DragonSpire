using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire.Commands
{
	public class Announce : Command
	{
		public override string Name
		{
			get { return "Announce"; }
		}

		public override string[] Accessors
		{
			get { return new string[1] { "announce" }; }
		}

		public override void UseByPlayer(Player p, string FullCommand)
		{
			string[] Args = FullCommand.Split(new char[] { ' ' }, 2);
			if (Args.Length.Equals(2))
			{
				Player.Announcement("Server", Args[1]);
			}
			else
			{
				HelpPlayer(p, FullCommand);
			}
		}

		public override void HelpPlayer(Player p, string FullCommand)
		{
			p.SendMessage("/announce <message>");
			p.SendMessage("Announce a message to the entire server.");
		}
	}
}