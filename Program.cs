using System;
using System.Xml.Linq;
using NSUci;

namespace NSRapchess
{
	class Program
	{
		public static CSearch engine = new CSearch();
		public static CUci uci = new CUci();
		public static CBench bench = new CBench();
		public static COptions options = new COptions();
		public static CDataIn dataIn = new CDataIn();
		public static CDataOut dataOut = new CDataOut();
		readonly static string version = "2023-04-30";
		readonly static string name = $"RapChessCs ver. {version}";

		static void UciCommand(string msg)
		{
			uci.SetMsg(msg);
			switch (uci.command)
			{
				case "uci":
					Console.WriteLine($"id name {name}");
					Console.WriteLine("id author Thibor Raven");
					Console.WriteLine("id link https://github.com/Thibor/RapChessCs");
					Console.WriteLine("option name Mate pruning type check default true");
					Console.WriteLine("option name Null pruning type check default true");
					Console.WriteLine("option name Ponder type check default true");
					Console.WriteLine($"option name MultiPV type spin default {CSearch.optMultiPv} min 1 max 100");
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
							CSearch.optMatePruning = uci.GetValue("value") == "true";
							Console.WriteLine("info string Mate pruning" + (CSearch.optMatePruning ? "on" : "off"));
							break;
						case "null pruning":
							CSearch.optNullPruning = uci.GetValue("value") == "true";
							Console.WriteLine("info string Null pruning" + (CSearch.optNullPruning ? "on" : "off"));
							break;
						case "ponder":
							options.ponder = uci.GetValue("value") == "true";
							break;
						case "multipv":
							CSearch.optMultiPv = uci.GetInt("value", CSearch.optMultiPv);
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
					if (uci.GetIndex("eval") > 0)
						Console.WriteLine($"eval {CEvaluate.Eval()}");
					break;
				case "go":
                    engine.Searchmoves(uci.GetValue("searchmoves"));
                    engine.UciGo();
                    break;
				case "stop":
					engine.synStop.SetStop(true);
					break;
				case "quit":
					engine.synStop.SetStop(true);
					return;
				case "ponderhit":
					if (dataIn.ponder)
					{
						dataIn.ponder = false;
						engine.timeOut = dataIn.time;
						engine.depthOut = dataIn.depth;
						engine.nodeOut = dataIn.nodes;
						engine.stopwatch.Restart();
					}
					else
						engine.BestMove();
					break;
				case "eval":
					CEvaluate.PrintEval();
					break;
				case "bench":
					dataIn.Reset();
					bench.Start();
					break;
			}
		}

		static void Main()
		{
			Console.WriteLine(name);
			//UciCommand("setoption name MultiPV value 3");
            while (true)
			{
				string msg = Console.ReadLine();
				UciCommand(msg);
			}
		}
	}
}
