using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using RapLog;

namespace NSRapchess
{

	class CBench
	{
		public bool start = false;
		bool modeFen = true;
		bool modeScore = true;
		bool modeSort = false;
		public ulong nodes = 0;
		string modeGo = "go infinite";
		int index = 0;
		readonly Stopwatch timer = new Stopwatch();
		List<string> list;
		readonly CRapLog log = new CRapLog();
		List<string> fenList = new List<string>();

		string GetTimeElapsed()
		{
			DateTime dt = new DateTime();
			dt = dt.AddMilliseconds(timer.Elapsed.TotalMilliseconds);
			return dt.ToString("HH:mm:ss.fff ");
		}

		void WriteLine(string msg)
		{
			Console.WriteLine(msg);
			log.Add(msg);
		}

		void Sort()
		{
			fenList.Sort();
			fenList.Insert(0,modeGo);
			fenList.Insert(0,"sort");
			File.WriteAllLines("bench.txt", fenList);
		}

		void BenchStart()
		{
			index = 0;
			start = true;
			nodes = 0;
			fenList.Clear();
			timer.Restart();
			WriteLine("bench start");
			Next();
		}

		void BenchFinish()
		{
			start = false;
			timer.Stop();
			double ms = timer.Elapsed.TotalMilliseconds;
			double nps = ms > 0 ? (nodes * 1000.0)/ ms : 0;
			WriteLine($"time {GetTimeElapsed()} nodes {nodes:N0} nps {nps:N0}");
			WriteLine("bench end");
			if (modeSort)
				Sort();
		}

		void Read()
		{
			if (index >= list.Count)
			{
				BenchFinish();
				return;
			}
			CUci uci2 = new CUci();
			while (index < list.Count)
			{
				string order = list[index++];
				uci2.SetMsg(order);
				switch (uci2.command)
				{
					case "finish":
						BenchFinish();
						return;
					case "fen":
						modeFen = true;
						continue;
					case "moves":
						modeFen = false;
						continue;
					case "score":
						modeScore = true;
						continue;
					case "go":
						modeScore = false;
						modeGo = order;
						continue;
					case "sort":
						modeSort = true;
						continue;
				}
				if (modeFen)
					Program.chess.SetFen(order);
				else
				{
					Program.chess.SetFen();
					Program.chess.MakeMoves(order);
				}
				if (modeScore)
					CScore.Trace();
				else
				{
					string fen = Program.chess.GetFen();
					fenList.Add(fen);
					log.Add(fen);
					Program.uci.SetMsg(modeGo);
					Program.chess.UciGo();
					break;
				}
			}
		}

		public void Next()
		{
			if (start)
				Read();
		}

		public void Start()
		{
			string fn = "bench.txt";
			if (File.Exists(fn))
			{
				string[] aList = File.ReadAllLines(fn);
				list = new List<string>(aList);
				BenchStart();
			}
			else
				CScore.Trace();
		}

	}
}
