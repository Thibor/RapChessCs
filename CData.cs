using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapChessGui
{
	enum CMode { normal, training }
	public static class CData
	{
		public static bool rotateBoard = false;
		public static int gameState = 0;
		public static int gameMode = 0;
		public static string[] arrModeNames = new string[] { "depth","movetime" };
		public static List<string> engineNames = new List<string>();
		public static List<string> playerNames = new List<string>();

		public static int ModeStoi(string s)
		{
			for (int i = 0; i < arrModeNames.Length; i++)
				if (s == arrModeNames[i])
					return i;
			return 0;
		}

		public static string ModeItos(int i)
		{
			return arrModeNames[i];
		}

	}
}
