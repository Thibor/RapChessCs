using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapChessGui
{

	public class CUser
	{
		public string name = "Human";
		public string engine ="Human";
		public string parameters = "";
		public string mode = "movetime";
		public string value = "1000";

		public CUser(string n)
		{
			name = n;
		}

		public void LoadFromIni()
		{
			engine = CIniFile.Read("engine","Human", "player " + name);
			parameters = CIniFile.Read("parameters", "", "player " + name);
			mode = CIniFile.Read("mode","movetime", "player " + name);
			value = CIniFile.Read("value", "1000", "player " + name);
		}

		public void SaveToIni()
		{
			CIniFile.Write("engine",engine, "player " + name);
			CIniFile.Write("parameters", parameters, "player " + name);
			CIniFile.Write("mode", mode, "player " + name);
			CIniFile.Write("value", value, "player " + name);
		}
	}

	public class CUserList
	{
		public static List<CUser> list = new List<CUser>();

		public static int GetIndex(string name)
		{
			for (int n = 0; n < list.Count; n++)
			{
				var user = list[n];
				if (user.name == name)
					return n;
			}
			return -1;
		}

		public static CUser GetUser(string name)
		{
			for (int n = 0; n < list.Count; n++)
			{
				var user = list[n];
				if (user.name == name)
					return user;
			}
			return null;
		}

		public static void LoadFromIni()
		{
			list.Clear();
			string player = CIniFile.Read("playerNames");
			var apn = player.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			for(int n = 0; n < apn.Length; n++)
			{
				string name = apn[n];
				var u = new CUser(name);
				u.LoadFromIni();
				list.Add(u);
			}
			if(list.Count < 2)
			{
				var uc = new CUser("Computer");
				uc.engine = "RapChessCs.exe";
				uc.mode = "movetime";
				uc.value = "1000";
				list.Add(uc);
				var up = new CUser("Human");
				up.engine = "Human";
				list.Add(up);
				SaveToIni();
			}
		}

		public static void SaveToIni()
		{
			List<string> names = new List<string>();
			for(int n = 0; n < list.Count; n++)
			{
				var user = list[n];
				names.Add(user.name);
				user.SaveToIni();
			}
			names.Sort();
			CIniFile.Write("playerNames", String.Join("|",names.ToArray()));
		}
	}
}
