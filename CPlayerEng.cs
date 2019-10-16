using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapChessGui
{
	class CPlayerEng
	{
		public CReader Reader = new CReader();
		public Process process = new Process();
		public StreamWriter streamWriter;

		public void Kill()
		{
			process.StartInfo.FileName = "";
			try
			{
				process.Kill();
			}
			catch
			{
			}
		}

		public void SetEngine(string eng, string arg)
		{
			Kill();
			process.StartInfo.FileName = "Engines/" + eng;
			process.StartInfo.Arguments = arg;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.Start();
			streamWriter = process.StandardInput;
			Reader.SetStream(process.StandardOutput);
		}
	}
}
