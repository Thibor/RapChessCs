using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;


namespace RapChessGui
{
	public partial class Form1 : Form
	{
		[DllImport("user32.dll")]
		private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern int SetForegroundWindow(IntPtr hWnd);

		bool boardRotate;
		int lastDes = -1;
		CEngine Engine = new CEngine();
		CPlayerList PlayerList = new CPlayerList();
		CPieceList PieceList = new CPieceList();
		CTrainer Trainer = new CTrainer();
		CUci Uci = new CUci();
		FormOptions FOptions = new FormOptions();
		FormPlayer FPlayer = new FormPlayer();
		PrivateFontCollection pfc = new PrivateFontCollection();

		public Form1()
		{
			KillApplication();
			InitializeComponent();
			richTextBox1.AddContextMenu();
			Reset();
			IniRead();
			CPieceBoard.Prepare();
			pictureBox1.Size = new Size(CPieceBoard.size, CPieceBoard.size);
		}

		private void FormChess_Load(object sender, EventArgs e)
		{

			//Select your font from the resources.
			//My font here is "Digireu.ttf"
			int fontLength = Properties.Resources.ChessPiece.Length;

			// create a buffer to read in to
			byte[] fontdata = Properties.Resources.ChessPiece;

			// create an unsafe memory block for the font data
			System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

			// copy the bytes to the unsafe memory block
			Marshal.Copy(fontdata, 0, data, fontLength);

			// pass the font to the font collection
			pfc.AddMemoryFont(data, fontLength);
			ShowTraining();
			Engine.Initialize();
			NewGame();
		}

		void KillApplication()
		{
			Process cp = Process.GetCurrentProcess();
			string fn = cp.MainModule.FileName;
			Process[] processlist = Process.GetProcesses();
			foreach (Process process in processlist)
			{
				try
				{
					String fileName = process.MainModule.FileName;
					if (fileName == fn)
						if (process.MainWindowHandle != cp.MainWindowHandle)
						{
							ShowWindow(process.MainWindowHandle, 9);
							SetForegroundWindow(process.MainWindowHandle);

							cp.Kill();
						}
				}
				catch
				{
				}

			}
		}

		void KillProcess()
		{
			string eDir = AppDomain.CurrentDomain.BaseDirectory + "Engines";
			Process[] processlist = Process.GetProcesses();
			foreach (Process process in processlist)
			{
				try
				{
					String fileName = process.MainModule.FileName;
					if (fileName.IndexOf(eDir) == 0)
					{
						process.Kill();
					}

				}
				catch
				{
				}

			}
		}

		void ShowTraining()
		{
			labGames.Text = $"Games {Trainer.games}";
			nudTime.Value = Trainer.time;
			label12.Text = Trainer.win.ToString();
			label13.Text = Trainer.draw.ToString();
			label14.Text = Trainer.loose.ToString();
			label15.Text = $"{Trainer.Result()}%";
			label7.Text = Trainer.loose.ToString();
			label8.Text = Trainer.draw.ToString();
			label9.Text = Trainer.win.ToString();
			label10.Text = $"{100 - Trainer.Result()}%";
		}

		void IniRead()
		{
			comboBoxW.SelectedIndex = comboBoxW.FindStringExact(CIniFile.Read("white", "Human"));
			comboBoxB.SelectedIndex = comboBoxB.FindStringExact(CIniFile.Read("black", "Computer"));
			comboBoxTrained.SelectedIndex = comboBoxTrained.FindStringExact(CIniFile.Read("trained", "Computer"));
			comboBoxTeacher.SelectedIndex = comboBoxTeacher.FindStringExact(CIniFile.Read("teacher", "RapChessCs.exe"));
			Trainer.time = Convert.ToInt32(CIniFile.Read("time", "1000"));
			CPieceBoard.LoadFromIni();

		}

		void IniSave()
		{
			CIniFile.Write("white", comboBoxW.Text);
			CIniFile.Write("black", comboBoxB.Text);
			CIniFile.Write("trained", comboBoxTrained.Text);
			CIniFile.Write("teacher", comboBoxTeacher.Text);
			CIniFile.Write("time", Trainer.time.ToString());
			CPieceBoard.SaveToIni();
		}

		void Reset()
		{
			comboBoxW.Items.Clear();
			comboBoxB.Items.Clear();
			comboBoxTrained.Items.Clear();
			comboBoxTeacher.Items.Clear();
			for (int n = 0; n < CUserList.list.Count; n++)
			{
				CUser u = CUserList.list[n];
				comboBoxW.Items.Add(u.name);
				comboBoxB.Items.Add(u.name);
				comboBoxTrained.Items.Add(u.name);
			}
			foreach (string en in CData.engineNames)
				comboBoxTeacher.Items.Add(en);
		}

		bool MakeMove(string emo)
		{
			string cpName = PlayerList.CurPlayer().user.name;
			if (Engine.IsValidMove(emo) == 0)
			{
				labLast.Text = "Move error " + emo;
				labLast.ForeColor = Color.Red;
				richTextBox1.AppendText($" error {emo}", Color.Red);
				richTextBox1.SaveFile("last error.rtf");
				MessageBox.Show($"{cpName} do wrong move {emo}");
				return false;
			}
			double t = (DateTime.Now - PlayerList.CurPlayer().timeStart).TotalMilliseconds;
			PlayerList.CurPlayer().timeTotal += t;
			int gm = Engine.GetMoveFromString(emo);
			CPieceBoard.MakeMove(gm);
			Engine.MakeMove(gm);
			int moveNumber = (Engine.g_moveNumber >> 1) + 1;
			labMove.Text = "Move " + moveNumber.ToString() + " " + Engine.g_move50.ToString();
			CHistory.moves.Add(emo);
			ShowHistory();
			CData.gameState = Engine.GetGameState();
			if (CData.gameState == 0)
				PlayerList.MakeMove();
			else
			{
				labLast.ForeColor = Color.Yellow;
				switch (CData.gameState)
				{
					case (int)CGameState.mate:
						labLast.Text = cpName + " win";
						break;
					case (int)CGameState.drawn:
						labLast.Text = "Stalemate";
						break;
					case (int)CGameState.repetition:
						labLast.Text = "Threefold repetition";
						break;
					case (int)CGameState.move50:
						labLast.Text = "Fifty-move rule";
						break;
					case (int)CGameState.material:
						labLast.Text = "Insufficient material";
						break;
				}
				if (CData.gameMode == (int)CMode.training)
				{
					Trainer.games++;
					if (CData.gameState == (int)CGameState.mate)
					{
						if (cpName == "Teacher")
						{
							Trainer.win++;
							Trainer.time -= 100;
							if (Trainer.time < 100)
								Trainer.time = 100;
						}
						else
						{
							Trainer.loose++;
							Trainer.time += 100;
						}
					}
					else
					{
						Trainer.draw++;
						Trainer.time += 100;
					}
					ShowTraining();
					richTextBox1.SaveFile("last game.rtf");
					timerStart.Start();
				}
			}
			return true;
		}

		void Clear()
		{
			labTimeT.Text = "00:00:00";
			labTimeB.Text = "00:00:00";
			labScoreT.Text = "Score 0";
			labScoreB.Text = "Score 0";
			labDepthT.Text = "Depth 0";
			labDepthB.Text = "Depth 0";
			labNpsT.Text = "Nps 0";
			labNpsB.Text = "Nps 0";
			richTextBox1.Clear();
			labMove.Text = "Move 1 0";
			labLast.ForeColor = Color.Gainsboro;
			labLast.Text = "Good luck";
			lastDes = -1;
			Engine.InitializeFromFen();
			CData.gameState = 0;
			CHistory.NewGame(Engine.GetFen());
			PieceList.Fill();
			PlayerList.NewGame();
			timer1.Enabled = true;
		}

		void NewGame()
		{
			KillProcess();
			CData.gameMode = (int)CMode.normal;
			PlayerList.player[0].SetUser(comboBoxW.Text);
			PlayerList.player[1].SetUser(comboBoxB.Text);
			Clear();
		}

		public void RenderBoard()
		{
			Color curColor = Engine.whiteTurn ? Color.White : Color.Black;
			string abc = "ABCDEFGH";
			boardRotate = (PlayerList.player[1].IsHuman() && !PlayerList.player[0].IsHuman()) ^ CData.rotateBoard;
			if (PlayerList.player[1].IsHuman() && PlayerList.player[0].IsHuman())
				boardRotate = !PlayerList.CurPlayer().IsWhite();
			if (boardRotate)
			{
				labNameT.Text = PlayerList.player[0].user.name;
				labNameB.Text = PlayerList.player[1].user.name;
			}
			else
			{
				labNameT.Text = PlayerList.player[1].user.name;
				labNameB.Text = PlayerList.player[0].user.name;
			}
			if (boardRotate ^ Engine.whiteTurn)
			{
				panBottom.BackColor = curColor;
				panTop.BackColor = Color.Silver;
			}
			else
			{
				panTop.BackColor = curColor;
				panBottom.BackColor = Color.Silver;
			}
			pictureBox1.Image = new Bitmap(CPieceBoard.bitmap);
			Graphics g = Graphics.FromImage(pictureBox1.Image);
			Brush brush3 = new SolidBrush(Color.FromArgb(0x80, 0xff, 0xff, 0x00));
			Font font = new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold);
			Font fontPiece = new Font(pfc.Families[0], CPieceBoard.field);
			Brush foreBrush = new SolidBrush(Color.White);
			Brush brushW = new SolidBrush(Color.White);
			Brush brushB = new SolidBrush(Color.Black);
			Pen outline = new Pen(Color.Black, 4);
			Pen penW = new Pen(Color.Black, 4);
			Pen penB = new Pen(Color.White, 4);
			GraphicsPath gp = new GraphicsPath();
			GraphicsPath gpW = new GraphicsPath();
			GraphicsPath gpB = new GraphicsPath();
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			g.SmoothingMode = SmoothingMode.HighQuality;
			Rectangle rec = new Rectangle();
			for(int n= 0; n < 8; n++)
			{
				int x = boardRotate ? 7 - n : n;
				int y = boardRotate ? 7 - n : n;
				int x2 = CPieceBoard.margin + x * CPieceBoard.field;
				int y2 = CPieceBoard.margin + y * CPieceBoard.field;
				rec.X = 0;
				rec.Y = y2;
				rec.Width = CPieceBoard.margin;
				rec.Height = CPieceBoard.field;
				string letter = (8 - y).ToString();
				gp.AddString(letter, font.FontFamily, (int)font.Style, font.Size, rec, sf);
				rec.X = pictureBox1.Width - CPieceBoard.margin;
				gp.AddString(letter, font.FontFamily, (int)font.Style, font.Size, rec, sf);
				rec.X = x2;
				rec.Y = 0;
				rec.Width = CPieceBoard.field;
				rec.Height = CPieceBoard.margin;
				letter = abc[x].ToString();
				gp.AddString(letter, font.FontFamily, (int)font.Style, font.Size, rec, sf);
				rec.Y = pictureBox1.Height - CPieceBoard.margin;
				gp.AddString(letter, font.FontFamily, (int)font.Style, font.Size, rec, sf);
			}
			for (int y = 0; y < 8; y++)
			{
				int yr = boardRotate ? 7 - y : y;
				int y2 = CPieceBoard.margin + yr * CPieceBoard.field;
				for (int x = 0; x < 8; x++)
				{
					int i = y * 8 + x;
					int xr = boardRotate ? 7 - x : x;
					int x2 = CPieceBoard.margin + xr * CPieceBoard.field;
					rec.X = x2;
					rec.Y = y2;
					rec.Width = CPieceBoard.field;
					rec.Height = CPieceBoard.field;
					if (i == lastDes)
						g.FillRectangle(brush3,rec);
					CPiece piece = CPieceBoard.list[i];
					if (piece == null)
						continue;
					GraphicsPath gp1 = null;
					int image = 0;
					if (piece.desImage >= 0)
					{
						gp1 = piece.desImage > 5 ? gpB : gpW;
						image = piece.desImage % 6;
						gp1.AddString("pnbrqk"[image].ToString(), fontPiece.FontFamily, (int)fontPiece.Style, fontPiece.Size, rec, sf);
					}
					piece.SetDes(i, x2, y2);
					rec.X = piece.curXY.X;
					rec.Y = piece.curXY.Y;
					gp1 = piece.image > 5 ? gpB : gpW;
					image = piece.image % 6;
					gp1.AddString("pnbrqk"[image].ToString(), fontPiece.FontFamily, (int)fontPiece.Style, fontPiece.Size, rec, sf);
				}
			}
			g.DrawPath(outline, gp);
			g.FillPath(foreBrush, gp);
			g.DrawPath(penW, gpW);
			g.FillPath(brushW, gpW);
			g.DrawPath(penB, gpB);
			g.FillPath(brushB, gpB);
			outline.Dispose();
			penW.Dispose();
			penB.Dispose();
			brush3.Dispose();
			foreBrush.Dispose();
			brushW.Dispose();
			brushB.Dispose();
			font.Dispose();
			fontPiece.Dispose();
			g.Dispose();
			pictureBox1.Refresh();
			if ((CData.gameState > 0) && (!CPieceBoard.animated))
			{
				timer1.Enabled = false;
				PieceList.Fill();
			}
			CPieceBoard.Render();
		}

		void RenderHistory()
		{
			Engine.InitializeFromFen();
			for (int n = 0; n < CHistory.moves.Count; n++)
			{
				string emo = CHistory.moves[n];
				int gmo = Engine.GetMoveFromString(emo);
				Engine.MakeMove(gmo);
			}
			PieceList.Fill();
			ShowHistory();
		}

		void ShowHistory()
		{
			richTextBox1.Clear();
			for (int n = 0; n < CHistory.moves.Count; n++)
			{
				if ((n & 1) == 0)
				{
					int moveNumber = (n >> 1) + 1;
					richTextBox1.AppendText($"{moveNumber} ", Color.Red);
				}
				string emo = CHistory.moves[n];
				richTextBox1.AppendText($"{emo} ");
			}
			string lm = CHistory.LastMove();
			lastDes = CEngine.EmoToIndex(lm);
		}

		void TrainerStart()
		{
			KillProcess();
			CData.gameMode = (int)CMode.training;
			Trainer.user.engine = comboBoxTeacher.Text;
			Trainer.user.mode = "movetime";
			Trainer.user.value = Trainer.time.ToString();
			var nw = comboBoxTrained.Text;
			PlayerList.player[0].SetUser(nw);
			PlayerList.player[1].SetUser(Trainer.user);
			if (Trainer.rotate == 1)
				PlayerList.Rotate();
			Trainer.rotate ^= 1;
			Clear();
		}

		bool TryMove(int s, int d)
		{
			int m = Engine.IsValidMove(s, d, "q");
			if (m == 0)
				return false;
			string em = Engine.FormatMove(m);
			return MakeMove(em);
		}

		private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
		{
			if (!PlayerList.CurPlayer().IsHuman())
				return;
			CPieceBoard.animated = true;
			int sou = lastDes;
			int x = (e.Location.X - CPieceBoard.margin) / CPieceBoard.field;
			int y = (e.Location.Y - CPieceBoard.margin) / CPieceBoard.field;
			if (boardRotate)
			{
				x = 7 - x;
				y = 7 - y;
			}
			lastDes = y * 8 + x;
			TryMove(sou, lastDes);
		}

		private void FormChess_FormClosed(object sender, FormClosedEventArgs e)
		{
			IniSave();
			KillProcess();
		}

		private void Timer1_Tick_1(object sender, EventArgs e)
		{
			if (CPieceBoard.animated)
			{
				RenderBoard();
				return;
			}
			var cp = PlayerList.CurPlayer();
			double dif = CData.gameState > 0 ? 0 : (DateTime.Now - cp.timeStart).TotalMilliseconds;
			if (!cp.IsHuman())
			{
				string msg = cp.PlayerEng.Reader.ReadLine(false);
				Uci.SetMsg(msg);
				if (Uci.command != "")
				{
					string s = Uci.GetValue("cp");
					if (s != "")
						cp.score = s;
					s = Uci.GetValue("mate");
					if (s != "")
					{
						if (Int32.Parse(s) > 0)
							cp.score = $"+{s}M";
						else
							cp.score = $"{s}M";
					}
					s = Uci.GetValue("depth");
					if (s != "")
						cp.depth = s;
					s = Uci.GetValue("seldepth");
					if (s != "")
						cp.seldepth = s;
					s = Uci.GetValue("nps");
					if (s != "")
						cp.nps = s;
					s = Uci.GetValue("ponder");
					if (s != "")
						cp.ponder = s;
					if (Uci.tokens[0] == "bestmove")
					{
						string em = Uci.tokens[1];
						MakeMove(em);
						dif = 0;
					}
					else
					{
						int i = Uci.GetIndex("pv", 0);
						if (i > 0)
						{
							string pv = "";
							for (int n = i; n < Uci.tokens.Length; n++)
								pv += Uci.tokens[n] + " ";
							labLast.Text = pv;
						}
					}
				}
			}
			DateTime tot = new DateTime().AddMilliseconds(cp.timeTotal + dif);
			if (boardRotate ^ cp.IsWhite())
			{
				labTimeB.Text = tot.ToString("HH:mm:ss");
				labScoreB.Text = $"Score {cp.score}";
				labNpsB.Text = $"Nps {cp.nps}";
				labPonderB.Text = $"Ponder {cp.ponder}";
				if (cp.seldepth != "0")
					if (cp.seldepth != "0")
						labDepthB.Text = $"Depth {cp.depth} / {cp.seldepth}";
					else
						labDepthB.Text = $"Depth {cp.depth}";
			}
			else
			{
				labTimeT.Text = tot.ToString("HH:mm:ss");
				labScoreT.Text = $"Score {cp.score}";
				labNpsT.Text = $"Nps {cp.nps}";
				labPonderT.Text = $"Ponder {cp.ponder}";
				if (cp.seldepth != "0")
					labDepthT.Text = $"Depth {cp.depth} / {cp.seldepth}";
				else
					labDepthT.Text = $"Depth {cp.depth}";
			}
		}

		private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewGame();
		}

		private void ButStart_Click(object sender, EventArgs e)
		{
			NewGame();
		}

		private void PlayersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			IniSave();
			FPlayer.ShowDialog(this);
			Reset();
			IniRead();
		}

		private void ButStop_Click(object sender, EventArgs e)
		{
			PlayerList.CurPlayer().SendMessage("stop");
		}

		private void BackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CHistory.Back();
			RenderHistory();
		}

		private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FOptions.ShowDialog(this);
			CPieceBoard.Prepare();
			RenderBoard();
		}

		private void ButTraining_Click(object sender, EventArgs e)
		{
			TrainerStart();
		}

		private void TimerStart_Tick(object sender, EventArgs e)
		{
			timerStart.Stop();
			if (CData.gameMode == (int)CMode.training)
			{
				TrainerStart();
			}
		}

		private void saveToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(Engine.GetFen());
		}

		private void loadFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string fen = Clipboard.GetText();
			if (!Engine.InitializeFromFen(fen))
			{
				MessageBox.Show("Wrong fen");
				return;
			}
			CHistory.NewGame(fen);
			lastDes = -1;
			PieceList.Fill();
			RenderBoard();
		}
	}

	public static class RichTextBoxExtensions
	{
		public static void AppendText(this RichTextBox box, string text, Color color)
		{
			box.SelectionStart = box.TextLength;
			box.SelectionLength = 0;

			box.SelectionColor = color;
			box.AppendText(text);
			box.SelectionColor = box.ForeColor;
		}

		public static void AddContextMenu(this RichTextBox rtb)
		{
			if (rtb.ContextMenuStrip == null)
			{
				ContextMenuStrip cms = new ContextMenuStrip()
				{
					ShowImageMargin = false
				};

				ToolStripMenuItem tsmiUndo = new ToolStripMenuItem("Undo");
				tsmiUndo.Click += (sender, e) => rtb.Undo();
				cms.Items.Add(tsmiUndo);

				ToolStripMenuItem tsmiRedo = new ToolStripMenuItem("Redo");
				tsmiRedo.Click += (sender, e) => rtb.Redo();
				cms.Items.Add(tsmiRedo);

				cms.Items.Add(new ToolStripSeparator());

				ToolStripMenuItem tsmiCut = new ToolStripMenuItem("Cut");
				tsmiCut.Click += (sender, e) => rtb.Cut();
				cms.Items.Add(tsmiCut);

				ToolStripMenuItem tsmiCopy = new ToolStripMenuItem("Copy");
				tsmiCopy.Click += (sender, e) => rtb.Copy();
				cms.Items.Add(tsmiCopy);

				ToolStripMenuItem tsmiPaste = new ToolStripMenuItem("Paste");
				tsmiPaste.Click += (sender, e) => rtb.Paste();
				cms.Items.Add(tsmiPaste);

				ToolStripMenuItem tsmiDelete = new ToolStripMenuItem("Delete");
				tsmiDelete.Click += (sender, e) => rtb.SelectedText = "";
				cms.Items.Add(tsmiDelete);

				cms.Items.Add(new ToolStripSeparator());

				ToolStripMenuItem tsmiSelectAll = new ToolStripMenuItem("Select All");
				tsmiSelectAll.Click += (sender, e) => rtb.SelectAll();
				cms.Items.Add(tsmiSelectAll);

				cms.Opening += (sender, e) =>
				{
					tsmiUndo.Enabled = !rtb.ReadOnly && rtb.CanUndo;
					tsmiRedo.Enabled = !rtb.ReadOnly && rtb.CanRedo;
					tsmiCut.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
					tsmiCopy.Enabled = rtb.SelectionLength > 0;
					tsmiPaste.Enabled = !rtb.ReadOnly && Clipboard.ContainsText();
					tsmiDelete.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
					tsmiSelectAll.Enabled = rtb.TextLength > 0 && rtb.SelectionLength < rtb.TextLength;
				};

				rtb.ContextMenuStrip = cms;
			}
		}
	}
}
