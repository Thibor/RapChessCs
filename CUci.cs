using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSRapchess
{
	class CUci
	{
		public string command;
		public string[] tokens;

		public int GetIndex(string key, int def)
		{
			for (int n = 0; n < tokens.Length; n++)
			{
				if (tokens[n] == key)
				{
					return n + 1;
				}
			}
			return def;
		}

		public int GetInt(string key, int def)
		{
			for (int n = 0; n < tokens.Length - 1; n++)
				if (tokens[n] == key)
					return Int32.Parse(tokens[n + 1]);
			return def;
		}

		public string GetStr(string key, string def="")
		{
			for (int n = 0; n < tokens.Length - 1; n++)
				if (tokens[n] == key)
					return tokens[n + 1];
			return def;
		}

		public void SetMsg(string msg)
		{
			if ((msg == null) || (msg == ""))
			{
				tokens = new string[0];
				command = "";
			}
			else
			{
				tokens = msg.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				command = tokens[0];
			}
		}
	}
}
