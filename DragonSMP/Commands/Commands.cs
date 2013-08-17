using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DragonSpire
{
	public static class CommandManager
	{
		public static Dictionary<string, Command> Commands = new Dictionary<string, Command>();

		internal static bool ExecuteCommand(Player p, string Accessor, string FullCommand)
		{
			Accessor = Accessor.Trim().ToLower();

			if (Commands.ContainsKey(Accessor))
			{
				Commands[Accessor].UseByPlayer(p, FullCommand);
				return true;
			}
			else
			{
				return false;
			}
		}

		internal static void LoadCommands()
		{
			Server.Log("Loading Commands...", LogTypesEnum.System);

			foreach (string fileOn in Directory.GetFiles(Directory.GetCurrentDirectory()))
			{
				FileInfo file = new FileInfo(fileOn);

				//Preliminary check, must be .dll
				if (file.Extension.Equals(".dll") || file.Extension.Equals(".exe"))
				{
					//Create a new assembly from the plugin file we're adding..
					Assembly pluginAssembly = Assembly.LoadFrom(file.Name);

					//Next we'll loop through all the Types found in the assembly
					foreach (Type pluginType in pluginAssembly.GetTypes())
					{
						if (pluginType.IsSubclassOf(typeof(Command)) && pluginType.IsPublic && !pluginType.IsAbstract)
						{
							var command = (Command)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
							//Server.Log("COMMAND: '" + command.Name + "' in " + file.Name, LogTypesEnum.Info);
						}
					}
				}
			}

			Server.Log(CommandManager.Commands.Count + " Commands loaded!", LogTypesEnum.System);
		}
	}

	public abstract class Command
	{
		public abstract string Name { get; }
		public abstract string[] Accessors { get; }

		public virtual void UseByPlayer(Player p, string FullCommand) { HelpPlayer(p, FullCommand); }
		public virtual void UseByConsole(string FullCommand) { HelpConsole(FullCommand); }

		public virtual void HelpPlayer(Player p, string FullCommand) { /*TODO*/ }
		public virtual void HelpConsole(string FullCommand) { Server.Log("Command Not Functional from CONSOLE", LogTypesEnum.Error); }

		public virtual void BlockChangeEvent(Player p, BlockLocation BL, dynamic OtherData) { }

		public Command()
		{
			foreach (string s in Accessors)
			{
				string ss = s.Trim().ToLower();
				if (CommandManager.Commands.ContainsKey(ss))
					{ CommandManager.Commands.Remove(ss); }
				CommandManager.Commands.Add(ss, this);
			}

			
		}
	}
}
