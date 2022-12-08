using System;

namespace NSRapchess
{
	class Program
	{
		public static CEngine engine = new CEngine();
		public static CUci uci = new CUci();
		public static CBench bench = new CBench();

		static void Main()
		{
			string version = "2022-12-08";

			while (true)
			{
				string msg = Console.ReadLine();
				uci.SetMsg(msg);
				switch (uci.command)
				{
					case "uci":
						Console.WriteLine($"id name Rapcschess {version}");
						Console.WriteLine("id author Thibor Raven");
						Console.WriteLine("id link https://github.com/Thibor/RapChessCs");
						Console.WriteLine("option name MatePruning type check default true");
						Console.WriteLine("option name NullPruning type check default true");
						Console.WriteLine($"option name MultiPV type spin default {CEngine.optMultiPv} min 1 max 100");
						Console.WriteLine("option name SkillLevel type spin default 100 min 0 max 100");
						Console.WriteLine($"option name Hash type spin default {CTranspositionTable.DEFAULT_SIZE_MB} min 0 max 200");
						Console.WriteLine("uciok");
						break;
					case "isready":
						Console.WriteLine("readyok");
						break;
					case "setoption":
						switch (uci.GetStr("name"))
						{
							case "MatePruning":
								CEngine.optMatePruning = uci.GetStr("value", "true") == "true";
								string mp = CEngine.optMatePruning ? "on" : "off";
								Console.WriteLine($"info string MatePruning {mp}");
								break;
							case "MultiPV":
								CEngine.optMultiPv = uci.GetInt("value", CEngine.optMultiPv);
								break;
							case "SkillLevel":
								int level = uci.GetInt("value", 100);
								CEvaluate.SetLevel(level);
								break;
							case "Hash":
								int hash = uci.GetInt("value", CTranspositionTable.DEFAULT_SIZE_MB);
								CTranspositionTable.Resize(hash);
								break;
						}
						break;
					case "ucinewgame":
						CTranspositionTable.Clear();
						break;
					case "position":
						string fen = String.Empty;
						int lo = uci.GetIndex("fen");
						int hi = uci.GetIndex("moves", uci.tokens.Length);
						if (lo > 0)
						{
							if (lo > hi)
								hi = uci.tokens.Length;
							for (int n = lo; n < hi; n++)
							{
								if (n > lo)
									fen += ' ';
								fen += uci.tokens[n];
							}
						}
						engine.SetFen(fen);
						lo = uci.GetIndex("moves");
						hi = uci.GetIndex("fen", uci.tokens.Length);
						if (lo > 0)
						{
							if (lo > hi)
								hi = uci.tokens.Length;
							string moves = String.Empty;
							for (int n = lo; n < hi; n++)
							{
								string m = uci.tokens[n];
								moves += $" {m}";
								int emo = engine.UmoToEmo(m);
								if (emo == 0)
								{
									Console.WriteLine($"info string wrong moves {moves}");
									break;
								}
								engine.MakeMove(emo);
								if (CPosition.move50 == 0)
									engine.undoIndex = 0;
							}
						}
						break;
					case "go":
						engine.UciGo();
						break;
					case "stop":
						engine.synStop.SetStop(true);
						break;
					case "quit":
						engine.synStop.SetStop(true);
						return;
					case "bench":
						bench.Start();
						break;
				}

			}
		}
	}
}
