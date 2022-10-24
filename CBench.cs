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
		readonly List<string> fenList = new List<string>();

		string GetTimeElapsed()
		{
			DateTime dt = new DateTime();
			dt = dt.AddMilliseconds(timer.Elapsed.TotalMilliseconds);
			return dt.ToString("HH:mm:ss.fff ");
		}

		void Sort()
		{
			fenList.Sort();
			string last = String.Empty;
			for (int n = fenList.Count - 1; n >= 0; n--)
			{
				string cur = fenList[n];
				if (cur == last)
					fenList.RemoveAt(n);
				last = cur;
			}
			fenList.Insert(0, modeGo);
			fenList.Insert(0, "sort");
			File.WriteAllLines("bench.txt", fenList);
		}

		void BenchStart()
		{
			index = 0;
			start = true;
			nodes = 0;
			fenList.Clear();
			timer.Restart();
			Console.WriteLine("bench start");
			Next();
		}

		void History()
		{
			List<string> list = log.List();
			for (int n = 2; n >= 0; n--)
				if (n < list.Count)
					Console.WriteLine(list[n]);
		}

		void BenchFinish()
		{
			if (!start)
				return;
			start = false;
			timer.Stop();
			double ms = timer.Elapsed.TotalMilliseconds;
			double nps = ms > 0 ? (nodes * 1000.0) / ms : 0;
			string msg = $"time {GetTimeElapsed()} nodes {nodes:N0} nps {nps:N0}";
			log.Add(msg);
			if (!modeScore)
				History();
			Console.WriteLine("bench end");
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
					case "//":
						continue;
					case "finish":
						if (fenList.Count == 0)
							CScore.Trace();
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
					Program.engine.SetFen(order);
				else
				{
					Program.engine.SetFen();
					Program.engine.MakeMoves(order);
				}
				string fen = Program.engine.GetFen();
				fenList.Add(fen);
				if (modeScore)
					CScore.Trace();
				else
				{
					CTranspositionTable.Clear();
					Program.uci.SetMsg(modeGo);
					Program.engine.UciGo();
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
