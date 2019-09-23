using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RapChessCs
{
	class CReader
	{
		private static Thread inputThread;
		private static AutoResetEvent getInput;
		private static AutoResetEvent gotInput;
		public static string input;
		public static bool inputReady;

		static CReader()
		{
			inputReady = false;
			getInput = new AutoResetEvent(false);
			gotInput = new AutoResetEvent(false);
			inputThread = new Thread(reader);
			inputThread.IsBackground = true;
			inputThread.Start();
		}

		private static void reader()
		{
			while (true)
			{
				getInput.WaitOne();
				if (!inputReady)
				{
					input = Console.ReadLine();
					inputReady = true;
					gotInput.Set();
				}
			}
		}

		public static string ReadLine(bool wait, bool ready)
		{
			if (inputReady)
			{
				inputReady = ready;
				return input;
			}
			getInput.Set();
			if (wait)
			{
				gotInput.WaitOne();
				inputReady = ready;
				return input;
			}
			else
				return "";
		}
	}
}