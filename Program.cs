using System;
using NSUci;

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
						Console.WriteLine("option name Mate pruning type check default true");
						Console.WriteLine("option name Null pruning type check default true");
						Console.WriteLine($"option name MultiPV type spin default {CEngine.optMultiPv} min 1 max 100");
						Console.WriteLine($"option name UCI_Elo type spin default {Constants.elo} min 0 max {Constants.elo}");
						Console.WriteLine("option name Skill level type spin default 100 min 0 max 100");
						Console.WriteLine($"option name Hash type spin default {CTranspositionTable.DEFAULT_SIZE_MB} min 0 max 200");
						Console.WriteLine("uciok");
						break;
					case "isready":
						Console.WriteLine("readyok");
						break;
					case "setoption":
						switch (uci.GetValue("name", "value").ToLower())
						{
							case "mate pruning":
								CEngine.optMatePruning = uci.GetValue("value") == "true";
								Console.WriteLine("info string Mate pruning" + (CEngine.optMatePruning ? "on" : "off"));
								break;
							case "null pruning":
								CEngine.optNullPruning = uci.GetValue("value") == "true";
								Console.WriteLine("info string Null pruning" + (CEngine.optNullPruning ? "on" : "off"));
								break;
							case "multipv":
								CEngine.optMultiPv = uci.GetInt("value", CEngine.optMultiPv);
								break;
							case "skill level":
								int level = uci.GetInt("value", 100);
								CEvaluate.SetLevel(level);
								break;
							case "uci_elo":
								int elo = uci.GetInt("value", Constants.elo);
								CEvaluate.SetLevel(elo * 100 / Constants.elo);
								break;
							case "hash":
								int hash = uci.GetInt("value", CTranspositionTable.DEFAULT_SIZE_MB);
								CTranspositionTable.Resize(hash);
								break;
						}
						break;
					case "ucinewgame":
						CTranspositionTable.Clear();
						break;
					case "position":
						engine.SetFen(uci.GetValue("fen", "moves"));
						engine.MakeMoves(uci.GetValue("moves"));
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
					case "eval":
						Console.WriteLine($"evaluation {CEvaluate.Score()}");
						break;
					case "bench":
						bench.Start();
						break;

				}

			}
		}
	}
}
