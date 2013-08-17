using System;
using System.Net;
using System.IO;

namespace DragonSpire
{
	public class Authenticate
	{
		static string AuthURL = "http://session.minecraft.net/game/checkserver.jsp?";

		internal static bool Authenticated(string Username, string Hash)
		{
			string FinalURL = AuthURL + "user=" + Username + "&serverId=" + Hash;
			WebRequest Request = WebRequest.Create(FinalURL);
			Request.Credentials = CredentialCache.DefaultCredentials;
			WebResponse Response = Request.GetResponse();
			Stream ResponseStream = Response.GetResponseStream();
			StreamReader Reader = new StreamReader(ResponseStream);
			string HTTPResponse = Reader.ReadToEnd();
			Console.WriteLine(HTTPResponse);
			Reader.Close();
			Response.Close();
			if (HTTPResponse.Equals("OK"))
				return true;
			return false;
		}
	}
}