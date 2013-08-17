using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DragonSpire
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			foreach (Player p in Player.players.Values.ToArray())
			{
				foreach (Player p2 in Player.players.Values.ToArray())
				{
					if (p2 != p)
					{
						p.client.SpawnNamedEntity(p2);
					}
				}
			}
		}
	}
}
