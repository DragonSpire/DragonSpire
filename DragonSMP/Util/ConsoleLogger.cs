using System;
using System.Collections.Generic;

//TODO Setup the TYPES of logging to be static, so that I can use them in other subsystems.

namespace DragonSpire
{
	public static class ConsoleLogger
	{
		public static Dictionary<LogTypesEnum, LogTypeClass> LogTypeList = new Dictionary<LogTypesEnum, LogTypeClass>();

		public static void Log(string message, ConsoleColor textColor, ConsoleColor backgroundColor)
		{
			Console.ForegroundColor = textColor;
			Console.BackgroundColor = backgroundColor;
			Console.WriteLine(message.PadRight(Console.WindowWidth - 1));
			Console.ResetColor();
		}
		public static void Log(string message, LogTypesEnum logTypes)
		{
			LogTypeClass logType = LogTypeList[logTypes];
			Log(logType.Prefix + message, logType.TextColor, logType.BackgroundColor);
		}

		public static void InitLogTypes()
		{
			LogTypeList.Add(LogTypesEnum.Normal, new LogTypeClass(ConsoleColor.Gray, ConsoleColor.Black, ""));
			LogTypeList.Add(LogTypesEnum.Info, new LogTypeClass(ConsoleColor.White, ConsoleColor.Black, "[INFO]:"));
			LogTypeList.Add(LogTypesEnum.System, new LogTypeClass(ConsoleColor.Green, ConsoleColor.Black, "[SYSTEM]:"));

			LogTypeList.Add(LogTypesEnum.Warning, new LogTypeClass(ConsoleColor.Red, ConsoleColor.Black, "[WARNING]:"));
			LogTypeList.Add(LogTypesEnum.Error, new LogTypeClass(ConsoleColor.Yellow, ConsoleColor.Black, "[ERROR]:"));
			LogTypeList.Add(LogTypesEnum.Critical, new LogTypeClass(ConsoleColor.White, ConsoleColor.Red, "[CRITICAL]:"));

			LogTypeList.Add(LogTypesEnum.Chat, new LogTypeClass(ConsoleColor.Magenta, ConsoleColor.Black, ""));
			LogTypeList.Add(LogTypesEnum.Debug, new LogTypeClass(ConsoleColor.DarkGreen, ConsoleColor.Black, "[DBG]:"));
		}
		public static void LogSamples()
		{
			Log("Normal Logging Message", LogTypesEnum.Normal);
			Log("Informational Logging Message", LogTypesEnum.Info);
			Log("System Information Logging", LogTypesEnum.System);
			Log("WARNING Logging message", LogTypesEnum.Warning);
			Log("OMFG AN ERROR =(", LogTypesEnum.Error);
			Log("WE ARE ALL GONNA DIE O_o", LogTypesEnum.Critical);
			Log("Hi, i'm bob, what's your name?", LogTypesEnum.Chat);
			Log("Debug-g-g-g-g-g-g-g-g-g.... Hello? anyone there? :*(", LogTypesEnum.Debug);
		}
	}
	public enum LogTypesEnum
	{
		Normal, //normal is for just run of the mill stuff (player connections for example)
		Info, //info is more in depth but not required 
		System, //Server info (startup shutdown loaded etc)
		Warning, //Server warnings (high traffic etc)
		Error, //Server errors
		Critical, //Errors that cause sessions to crash

		Chat, //Chat messages
		Debug, //Debug messages
	}
	public class LogTypeClass
	{
		internal ConsoleColor BackgroundColor;
		internal string Prefix;
		internal ConsoleColor TextColor;

		internal LogTypeClass(ConsoleColor textColor, ConsoleColor backgroundColor, string prefix)
		{
			TextColor = textColor;
			BackgroundColor = backgroundColor;
			Prefix = prefix;
		}
	}
}