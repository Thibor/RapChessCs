using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapChessGui
{
	public static class CHistory
	{
		public static string fen = CEngine.defFen;
		public static List<string> moves = new List<string>();

		public static void Back()
		{
			if (moves.Count > 1)
				moves.RemoveRange(moves.Count - 2,2);
		}

		public static string LastMove()
		{
			if (moves.Count == 0)
				return "";
			return moves[moves.Count - 1];
		}

		public static void NewGame(string f = CEngine.defFen)
		{
			fen = f;
			moves.Clear();
		}

		public static string GetPosition()
		{
			string result = "position ";
			result += fen == CEngine.defFen ? "startpos" : "fen " + fen;
			return result + " moves " + String.Join(" ", moves);
		}
	}
}
