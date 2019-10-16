using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RapChessGui
{
	public partial class FormChess : Form
	{
		bool boardRotate;
		int lastDes = -1;
		int field = 0;
		CIniFile IniFile = new CIniFile();
		CEngine Engine = new CEngine();
		CPlayerList PlayerList = new CPlayerList();
		CPieceList PieceList = new CPieceList();
		CTrainer Trainer = new CTrainer();
		CUci Uci = new CUci();
		FormOptions FOptions = new FormOptions();
		FormPlayer FPlayer = new FormPlayer();

		public FormChess()
		{
			KillApplication();
			InitializeComponent();
			richTextBox1.AddContextMenu();
			CPieceBoard.Prepare();
			pictureBox1.Size = new Size(CPieceBoard.size, CPieceBoard.size);
			Reset();
		}

		private void FormChess_Load(object sender, EventArgs e)
		{
			ShowTraining();
			Engine.Initialize();
			NewGame();
		}

		void KillApplication()
		{
			int count = 0;
			string fn = Process.GetCurrentProcess().MainModule.FileName;
			Process[] processlist = Process.GetProcesses();
			foreach (Process process in processlist)
			{
				try
				{
					String fileName = process.MainModule.FileName;
					if (fileName == fn)
						count++;

				}
				catch
				{
				}

			}
			if (count > 1)
				Process.GetCurrentProcess().Kill();
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
					string cDir = Path.GetDirectoryName(fileName);
					if (cDir == eDir)
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
			labTrainTime.Text = $"Time {Trainer.time / 1000.0} s";
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
			comboBoxW.SelectedIndex = comboBoxW.FindStringExact(IniFile.Read("white", "Human"));
			comboBoxB.SelectedIndex = comboBoxB.FindStringExact(IniFile.Read("black", "Computer"));
			comboBoxTrained.SelectedIndex = comboBoxTrained.FindStringExact(IniFile.Read("trained", "Computer"));
			comboBoxTeacher.SelectedIndex = comboBoxTeacher.FindStringExact(IniFile.Read("teacher", "RapChessCs.exe"));
			Trainer.time = Convert.ToInt32(IniFile.Read("time", "1000"));
		}

		void IniSave()
		{
			IniFile.Write("white", comboBoxW.Text);
			IniFile.Write("black", comboBoxB.Text);
			IniFile.Write("trained", comboBoxTrained.Text);
			IniFile.Write("teacher", comboBoxTeacher.Text);
			IniFile.Write("time", Trainer.time.ToString());
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
			IniRead();
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

		void RenderBoard()
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
			field = (pictureBox1.Width - CPieceBoard.margin * 2) / 8;
			pictureBox1.Image = new Bitmap(CPieceBoard.bitmap);
			Graphics g = Graphics.FromImage(pictureBox1.Image);
			Brush brush3 = new SolidBrush(Color.FromArgb(0x80, 0xff, 0xff, 0x00));
			Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
			for (int y = 0; y < 8; y++)
			{
				int y2 = CPieceBoard.margin + y * field;
				if (boardRotate)
					y2 = y2 = CPieceBoard.margin + (7 - y) * field;
				string letter = (8 - y).ToString();
				SizeF size = g.MeasureString(letter, font);
				g.DrawString(letter, font, new SolidBrush(Color.White), (CPieceBoard.margin - size.Width) / 2, y2 + (field - size.Height) / 2);
				g.DrawString(letter, font, new SolidBrush(Color.White), pictureBox1.Width - size.Width - (CPieceBoard.margin - size.Width) / 2, y2 + (field - size.Height) / 2);
				for (int x = 0; x < 8; x++)
				{
					int i = y * 8 + x;
					int x2 = CPieceBoard.margin + x * field;
					if (boardRotate)
						x2 = CPieceBoard.margin + (7 - x) * field;
					letter = abc[x].ToString();
					size = g.MeasureString(letter, font);
					g.DrawString(letter, font, new SolidBrush(Color.White), x2 + (field - size.Width) / 2, (CPieceBoard.margin - size.Height) / 2);
					g.DrawString(letter, font, new SolidBrush(Color.White), x2 + (field - size.Width) / 2, pictureBox1.Height - size.Height - (CPieceBoard.margin - size.Height) / 2);
					if (i == lastDes)
						g.FillRectangle(brush3, x2, y2, field, field);
					CPiece piece = CPieceBoard.list[i];
					if (piece == null)
						continue;
					piece.SetDes(i, x2, y2);
					if(piece.desImage >= 0)
						imageList1.Draw(g, piece.desXY, piece.desImage);
					imageList1.Draw(g, piece.curXY, piece.image);
				}
			}
			font.Dispose();
			brush3.Dispose();
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
			int x = (e.Location.X - CPieceBoard.margin) / field;
			int y = (e.Location.Y - CPieceBoard.margin) / field;
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
			FPlayer.ShowDialog(this);
			Reset();
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
