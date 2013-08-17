using System.Text.RegularExpressions;

namespace DragonSpire
{
	#region Color
	internal class Color
	{
		/// <summary>
		/// The Escape Character is used to signal that a color, or style code.
		/// </summary>
		internal const string Escape = "\u00A7";

		/// <summary>
		/// Color Characters
		/// </summary>
		internal const string Black = Escape + "0";
		internal const string DarkBlue = Escape + "1";
		internal const string DarkGreen = Escape + "2";
		internal const string DarkCyan = Escape + "3";
		internal const string DarkRed = Escape + "4";
		internal const string Purple = Escape + "5";
		internal const string Gold = Escape + "6";
		internal const string Gray = Escape + "7";
		internal const string DarkGray = Escape + "8";
		internal const string Blue = Escape + "9";
		internal const string BrightGreen = Escape + "a";
		internal const string Cyan = Escape + "b";
		internal const string Red = Escape + "c";
		internal const string Pink = Escape + "d";
		internal const string Yellow = Escape + "e";
		internal const string White = Escape + "f";

		/// <summary>
		/// Style Characters
		/// </summary>
		internal const string Random = Escape + "k";
		internal const string Bold = Escape + "l";
		internal const string Strike = Escape + "m";
		internal const string Underline = Escape + "n";
		internal const string Italic = Escape + "o";
		internal const string Plain = Escape + "r";

		internal static string ToString(char Character)
		{
			switch (Character)
			{
				case '0':
					return "black";
				case '1':
					return "darkblue";
				case '2':
					return "darkgreen";
				case '3':
					return "darkcyan";
				case '4':
					return "darkred";
				case '5':
					return "purple";
				case '6':
					return "gold";
				case '7':
					return "gray";
				case '8':
					return "darkgray";
				case '9':
					return "blue";
				case 'a':
					return "brightgreen";
				case 'b':
					return "cyan";
				case 'c':
					return "red";
				case 'd':
					return "pink";
				case 'e':
					return "yellow";
				default:
					return "white";
			}
		}

		internal static bool IsValidCharacter(char Character)
		{
			Match M = Regex.Match(Character.ToString(), @"[0-9a-fA-FRr]");
			return M.Success;
		}

		internal static string ParseColors(string Text)
		{
			if (Text.Equals("%%")) { Text = "§f"; }
			if (Text.EndsWith("%")) { Text += "f"; }

			string NewText = "";
			char[] CharArray = Text.ToCharArray();

			if (Text.Contains("%"))
			{
				foreach (Match M in Regex.Matches(Text, "%"))
				{
					if (CharArray[M.Index + 1].Equals('%'))
					{
						CharArray[M.Index + 1] = 'f';
					}
					if (IsValidCharacter(CharArray[M.Index + 1]))
					{
						CharArray[M.Index] = '§';
					}
				}
				foreach (char C in CharArray)
				{
					NewText += C;
				}
				return NewText;
			}
			return Text; //return the exactly same string if the identifier is not found
		}
	}
	#endregion

	internal class Messager
	{
		private static string ReplaceChars(string Message)
		{
			string Replaced = Message;
			Replaced = Replaced.Replace("\\", "\\" + "\\");
			Replaced = Replaced.Replace("\"", "\\" + "\"");
			return Replaced;
		}

		internal static string ChatMessage(string Sender, string Message)
		{
			Message = ReplaceChars(Message);
			Message = Color.ParseColors(Message);
			return "{\"translate\":\"chat.type.text\",\"using\":[\"" + Sender + "\",\"" + Message + "\"]}";
		}

		internal static string Announcement(string Sender, string Message)
		{
			Message = ReplaceChars(Message);
			Message = Color.ParseColors(Message);
			return "{\"translate\":\"chat.type.announcement\",\"using\":[\"" + Sender + "\",\"" + Message + "\"]}";
		}
	}
}