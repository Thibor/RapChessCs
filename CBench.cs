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
		string modeGo = "go infinite";
		int index = 0;
		readonly Stopwatch timer = new Stopwatch();
		List<string> list;
		readonly CRapLog log = new CRapLog();

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

		void BenchStart()
		{
			index = 0;
			start = true;
			timer.Restart();
			WriteLine("bench start");
			Next();
		}

		void BenchFinish()
		{
			start = false;
			timer.Stop();
			WriteLine($"time {GetTimeElapsed()}");
			WriteLine("bench end");
		}

		void Read()
		{
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
					log.Add(Program.chess.GetFen());
					Program.uci.SetMsg(modeGo);
					Program.chess.UciGo();
					break;
				}
			}
			if (index >= list.Count)
				BenchFinish();
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
