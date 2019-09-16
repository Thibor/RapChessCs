using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RapCsChess
{
	class CReader
	{
		private static Thread inputThread;
		private static AutoResetEvent getInput;
		private static string input;
		private static bool newInput;

		static CReader()
		{
			getInput = new AutoResetEvent(false);
			newInput = false;
			inputThread = new Thread(reader);
			inputThread.IsBackground = true;
			inputThread.Start();
		}

		private static void reader()
		{
			while (true)
			{
				getInput.WaitOne();
				input = Console.ReadLine();
				newInput = true;
			}
		}

		public static string ReadLine()
		{
			if (newInput)
			{
				newInput = false;
				return input;
			}
			else
			{
				getInput.Set();
				return "";
			}
		}
	}
}
