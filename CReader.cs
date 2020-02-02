using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace RapChessCs
{
	class CReader
	{
		private static Thread inputThread;
		private static AutoResetEvent getInput;
		private static AutoResetEvent gotInput;
		public static string input = "";

		static CReader()
		{
			getInput = new AutoResetEvent(false);
			gotInput = new AutoResetEvent(false);
			inputThread = new Thread(Reader);
			inputThread.IsBackground = true;
			inputThread.Start();
		}

		private static void Reader()
		{
			while (true)
			{
				getInput.WaitOne();
				input = "";
				input = Console.ReadLine();
				getInput.Reset();
				gotInput.Set();
			}
		}

		public static string ReadLine(bool wait)
		{
			getInput.Set();
			if (wait)
				gotInput.WaitOne();
			return input;
		}

	}
}
