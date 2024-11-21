using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using RapLog;
using NSUci;

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
			Console.WriteLine("finish");
			Console.Beep();
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
							CEvaluate.PrintEval();
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
					CEvaluate.PrintEval();
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

		void Bench()
		{
			Program.dataIn.post = false;
			Console.WriteLine("Benchmark test");
			CSearch engine = Program.engine;
			MList mo = new MList();
			ulong totalMs = 0;
			ulong totalNodes = 0;
			for (int n = 1; n < 11; n++)
			{
				engine.Start(mo, n, 0, 0);
				ulong ms = (ulong)engine.stopwatch.ElapsedMilliseconds;
				ulong nodes = engine.nodeCur;
				totalMs += ms;
				totalNodes += nodes;
				string sMs = $"{ms:N0}";
				string sNodes = $"{nodes:N0}";
				Console.WriteLine($"{n}.     {new string(' ', 20 - (n < 10 ? 1 : 2) - sMs.Length)} {sMs} {new string(' ', 20 - sNodes.Length)} {sNodes}");
			}
			ulong nps = totalMs > 0 ? (totalNodes * 1000) / totalMs : 0;
			Console.WriteLine();
			Console.WriteLine(new string('=', 32));
			Console.WriteLine($"Time           {totalMs:N0}");
			Console.WriteLine($"Nodes          {totalNodes:N0}");
			Console.WriteLine($"Nps            {nps:N0}");
			Console.WriteLine(new string('=', 32));
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
				Bench();
		}

	}
}
