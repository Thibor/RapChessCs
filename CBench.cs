using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NSRapchess
{
	
	class CBench
	{
		string fn = "bench.txt";

		public void Start()
		{
			if (File.Exists(fn))
			{
				var aList = File.ReadAllLines(fn);
				if (aList.Length > 0)
				{
					bool modeFen = true;
					bool modeTrace = true;
					bool modeFinish = false;
					string modeGo = "go infinite";
					List<string> sList = new List<string>(aList);
					CUci uci2 = new CUci();
					foreach (string order in sList)
					{
						uci2.SetMsg(order);
						switch (uci2.command)
						{
							case "finish":
								modeFinish = true;
								break;
							case "fen":
								modeFen = true;
								continue;
							case "moves":
								modeFen = false;
								continue;
							case "trace":
								modeTrace = true;
								continue;
							case "go":
								modeTrace = false;
								modeGo = order;
								continue;
						}
						if (modeFinish)
							break;
						if (modeFen)
							Program.chess.SetFen(order);
						else
						{
							Program.chess.SetFen();
							Program.chess.MakeMoves(order);
						}
						if (modeTrace)
							CScore.Trace();
						else
						{
							Program.uci.SetMsg(modeGo);
							Program.chess.UciGo();
							break;
						}

					}
				}
			}
			else
				CScore.Trace();
		}

	}
}
