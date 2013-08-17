using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire.Commands
{
	public class Gamemode : Command
	{
		public override string Name
		{
			get { return "Gamemode"; }
		}
		public override string[] Accessors
		{
			get { return new string[2] { "gamemode", "gm" }; }
		}
		public override void UseByPlayer(Player p, string FullCommand)
		{
            string[] s = FullCommand.Split(' ');
            if (s.Length < 2)
            {
				p.SendMessage("Changing Gamemode for " + p.name + " to " + (p.isInCreative ? "Survival (0)" : "Creative (1)"));
                p.isInCreative = !p.isInCreative;
            }
            else
            {
                Player to = Player.Find(s[1]);
                if (to == null) { p.SendMessage("Player could not be found."); return; }
				p.SendMessage("Changing gamemode for " + to.name + " to " + (to.isInCreative ? "Survival (0)" : "Creative (1)"));
                to.isInCreative = !to.isInCreative;
            }
		}
		public override void HelpPlayer(Player p, string FullCommand)
		{
            p.SendMessage("/gamemode (username)");
            p.SendMessage("Toggles between creative and survival game modes.");
		}
	}
}
