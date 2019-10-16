using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RapChessGui
{
	class CReader
	{

		private Thread inputThread;
		private AutoResetEvent getInput;
		private AutoResetEvent gotInput;
		private AutoResetEvent semInput;
		private string input;
		private StreamReader stream;

		private string Input
		{
			get
			{
				semInput.WaitOne(10, false)
					;
				string s = input;
				input = "";
				semInput.Set();
				return s;
			}
			set
			{
				semInput.WaitOne(10, false);
				input = value;
				semInput.Set();
			}
		}

		private void Reader()
		{
			while (true)
			{
				getInput.WaitOne();
				string s = stream.ReadLine();
				Input = s;
				gotInput.Set();
				getInput.Reset();
			}
		}

		public string ReadLine(bool wait)
		{
			getInput.Set();
			if (wait)
				gotInput.WaitOne();
			return Input;
		}

		public void SetStream(StreamReader sr)
		{
			stream = sr;
			getInput = new AutoResetEvent(false);
			gotInput = new AutoResetEvent(false);
			semInput = new AutoResetEvent(true);
			inputThread = new Thread(Reader);
			inputThread.IsBackground = true;
			inputThread.Start();
		}
	}
}

