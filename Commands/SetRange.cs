using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire.Commands
{
	public class SetRangeCommand : Command
	{
		public override string Name
		{
			get { return "Set Range"; }
		}
		public override string[] Accessors
		{
			get { return new string[1] { "SetRange" }; }
		}
		public override void UseByPlayer(Player p, string FullCommand)
		{
			string[] args = FullCommand.Split(' ');

			if (args.Length == 2)
			{
				try
				{
					float range = Convert.ToSingle(args[1]);
					p.PickupRange = range;
				}
				catch(Exception e)
				{
					p.SendMessage("Error, range must be a number!");
					string a = e.Message;
				}
			}
			else if (args.Length == 3)
			{
				p.SendMessage("Setting another players range is NYI");
			}
			else
			{
				p.SendMessage("Command arguments invalid, can only have 1 or two arguments!");
				HelpPlayer(p, FullCommand);
			}
		}
		public override void HelpPlayer(Player p, string FullCommand)
		{
			p.SendMessage("Set the distance at which a player can pickup blocks");
			p.SendMessage("/SetRange <player> [range]");
		}
	}
}
