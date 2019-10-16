using System;
using System.Collections.Generic;

namespace RapChessGui
{
	enum CGameState { normal, mate, drawn, repetition, move50, material }

	class CUndo
	{
		public int captured;
		public int hash;
		public int passing;
		public int castle;
		public int move50;
		public int lastCastle;
	}

	class CUci
	{
		public string command;
		public string[] tokens;

		public int GetIndex(string key, int def)
		{
			for (int n = 0; n < tokens.Length; n++)
			{
				if (tokens[n] == key)
				{
					return n + 1;
				}
			}
			return def;
		}

		public int GetInt(string key, int def)
		{
			for (int n = 0; n < tokens.Length - 1; n++)
			{
				if (tokens[n] == key)
				{
					return Int32.Parse(tokens[n + 1]);
				}
			}
			return def;
		}

		public string GetValue(string name)
		{
			int i = GetIndex(name, tokens.Length);
			if (i < tokens.Length)
				return tokens[i];
			return "";
		}

		public void SetMsg(string msg)
		{
			if ((msg == null) || (msg == ""))
			{
				tokens = new string[0];
				command = "";
			}
			else
			{
				tokens = msg.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				command = tokens[0];
			}
		}
	}

	class CEngine
	{
		public const string defFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 0";
		private static Random random = new Random();
		const int piecePawn = 0x01;
		const int pieceKnight = 0x02;
		const int pieceBishop = 0x03;
		const int pieceRook = 0x04;
		const int pieceQueen = 0x05;
		const int pieceKing = 0x06;
		public const int colorBlack = 0x08;
		public const int colorWhite = 0x10;
		public const int colorEmpty = 0x20;
		const int moveflagPassing = 0x02 << 16;
		public const int moveflagCastleKing = 0x04 << 16;
		public const int moveflagCastleQueen = 0x08 << 16;
		const int moveflagPromotion = 0xf0 << 16;
		const int moveflagPromoteQueen = 0x10 << 16;
		const int moveflagPromoteRook = 0x20 << 16;
		const int moveflagPromoteBishop = 0x40 << 16;
		const int moveflagPromoteKnight = 0x80 << 16;
		const int maskCastle = moveflagCastleKing | moveflagCastleQueen;
		const int maskColor = colorBlack | colorWhite;
		int g_captured = 0;
		int g_castleRights = 0xf;
		int g_depth = 0;
		int g_hash = 0;
		int g_passing = 0;
		public int g_move50 = 0;
		public int g_moveNumber = 0;
		int g_phase = 32;
		int g_totalNodes = 0;
		bool g_inCheck = false;
		int g_nodeout = 0;
		int g_timeout = 0;
		bool g_stop = false;
		string g_pv = "";
		string g_scoreFm = "";
		int g_lastCastle = 0;
		bool adjInsufficient = false;
		int adjMobility = 0;
		int g_countMove = 0;
		int undoIndex = 0;
		int[,] g_hashBoard = new int[256, 16];
		int[] boardCheck = new int[256];
		int[] boardCastle = new int[256];
		public bool whiteTurn = true;
		int usColor = 0;
		int enColor = 0;
		int bsIn = -1;
		int bsDepth = 0;
		string bsFm = "";
		string bsPv = "";
		public static int[] arrField = new int[64];
		public static int[] g_board = new int[256];
		int[,] tmpMaterial = new int[7, 2] { { 0, 0 }, { 171, 240 }, { 764, 848 }, { 826, 891 }, { 1282, 1373 }, { 2526, 2646 }, { 0xffff, 0xffff } };
		int[,] arrMaterial = new int[33, 7];
		int[,,] tmpMobility = new int[7, 28, 2] {
			{ { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{ { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{ {-75,-76 }, {-57,-54 }, {-9,-28 }, {-2,-10 }, {6,5 }, {14,12 }, {22, 26 }, { 29,29 }, { 36, 29 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{ { -48,-59 }, {-20,-23 }, {16, -3 }, {26, 13 }, {38, 24 }, {51, 42 }, {55, 54 }, {63, 57 }, {63, 65 }, {68, 73 }, {81, 78 }, {81, 86 }, {91, 88 }, {98, 97 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{ {-58,-76 }, {-27,-18 }, {-15, 28 }, { -10, 55 }, {-5, 69 }, { -2, 82 }, { 9,112 }, {16,118 }, { 30,132 }, {29,142 }, {32,155 }, {38,165 }, { 46,166 }, { 48,169 }, {58,171 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{{ -39, -36 },{-21, -15 },{3, 8 },{3, 18 },{14, 34 },{22, 54 },{28, 61 },{41, 73 },{43, 79 },{48, 92 },{56, 94 },{60, 104 },{60, 113 },{66, 120 },{67, 123 },{70, 126 },{71, 133 },{73, 136 },{79, 140 },{88, 143 },{88, 148 },{99, 166 },{102, 170 },{102, 175 },{106, 184 },{109, 191 },{113, 206 },{116, 212} },
			{ { 90,9 }, { 80,8 }, { 70, 7 }, { 60, 6 }, { 50, 5 }, { 40, 4 }, { 30, 3 }, { 20, 2 }, { 10, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } }
			};
		int[,,] arrMobility = new int[33, 7, 28];
		int[] arrDirKinght = { 14, -14, 18, -18, 31, -31, 33, -33 };
		int[] arrDirBishop = { 15, -15, 17, -17 };
		int[] arrDirRock = { 1, -1, 16, -16 };
		int[] arrDirQueen = { 1, -1, 15, -15, 16, -16, 17, -17 };
		DateTime g_startTime = new DateTime();
		CUci Uci = new CUci();
		CUndo[] undoStack = new CUndo[0xfff];

		public CEngine()
		{
			Initialize();
		}

		public int GetGameState()
		{
			List<int> mu = GenerateAllMoves(whiteTurn, false);
			bool myInsufficient = adjInsufficient;
			if (g_move50 >= 100)
				return (int)CGameState.move50;
			if (IsRepetition())
				return (int)CGameState.repetition;
			GenerateAllMoves(!whiteTurn, false);
			if (adjInsufficient && myInsufficient)
				return (int)CGameState.material;
			List<int> moves = GenerateValidMoves();
			if (moves.Count > 0)
				return (int)CGameState.normal;
			return g_inCheck ? (int)CGameState.mate : (int)CGameState.drawn;
		}

		int MakeSquare(int row, int column)
		{
			return ((row + 4) << 4) | (column + 4);
		}

		public int IsValidMove(int s, int d, string q)
		{
			int max = s & 7;
			int mbx = d & 7;
			int may = s >> 3;
			int mby = d >> 3;
			int sa = MakeSquare(may, max);
			int sb = MakeSquare(mby, mbx);
			string fa = FormatSquare(sa);
			string fb = FormatSquare(sb);
			string move = fa + fb;
			int piece = g_board[sa] & 0xf;
			if (((piece & 7) == piecePawn) && ((mby == 0) || (mby == 7)))
				move += q;
			return IsValidMove(move);
		}

		public int IsValidMove(int m)
		{
			List<int> moves = GenerateValidMoves();
			for (int i = 0; i < moves.Count; i++)
				if (moves[i] == m)
					return m;
			return 0;
		}

		public int IsValidMove(string m)
		{
			return IsValidMove(GetMoveFromString(m));
		}

		int RAND_32()
		{
			return random.Next();
		}

		public static int EmoToIndex(string emo)
		{
			if (emo == "")
				return -1;
			string fl = "abcdefgh";
			int x = fl.IndexOf(emo[2]);
			int y = 8 - Int32.Parse(emo[3].ToString());
			return y * 8 + x;
		}

		public string FormatMove(int move)
		{
			string result = FormatSquare(move & 0xFF) + FormatSquare((move >> 8) & 0xFF);
			if ((move & moveflagPromotion) > 0)
			{
				if ((move & moveflagPromoteQueen) > 0) result += 'q';
				else if ((move & moveflagPromoteRook) > 0) result += 'r';
				else if ((move & moveflagPromoteBishop) > 0) result += 'b';
				else result += 'n';
			}
			return result;
		}

		string FormatSquare(int square)
		{
			char[] arr = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
			return arr[(square & 0xf) - 4] + (12 - (square >> 4)).ToString();
		}

		public string GetFen()
		{
			string result = "";
			string[] arr = { " ", "p", "n", "b", "r", "q", "k", " " };
			for (int row = 0; row < 8; row++)
			{
				if (row != 0)
					result += '/';
				int empty = 0;
				for (int col = 0; col < 8; col++)
				{
					int piece = g_board[((row + 4) << 4) + col + 4];
					if (piece == colorEmpty)
						empty++;
					else
					{
						if (empty != 0)
							result += empty;
						empty = 0;
						string pieceChar = arr[(piece & 0x7)];
						result += ((piece & colorWhite) != 0) ? pieceChar.ToUpper() : pieceChar;
					}
				}
				if (empty != 0)
				{
					result += empty;
				}
			}
			result += whiteTurn ? " w " : " b ";
			if (g_castleRights == 0)
				result += "-";
			else
			{
				if ((g_castleRights & 1) != 0)
					result += 'K';
				if ((g_castleRights & 2) != 0)
					result += 'Q';
				if ((g_castleRights & 4) != 0)
					result += 'k';
				if ((g_castleRights & 8) != 0)
					result += 'q';
			}
			result += ' ';
			if (g_passing == 0)
				result += '-';
			else
				result += FormatSquare(g_passing);
			return result + ' ' + g_move50 + ' ' + g_moveNumber;
		}

		int StrToSquare(string s)
		{
			if (s.Length < 2)
				return 0;
			string fl = "abcdefgh";
			int x = fl.IndexOf(s[0]);
			int y = 12 - Int32.Parse(s[1].ToString());
			return (x + 4) | (y << 4);
		}

		bool IsRepetition()
		{
			for (int n = undoIndex - 6; n >= undoIndex - g_move50; n -= 2)
				if (undoStack[n].hash == g_hash)
				{
					return true;
				}
			return false;
		}

		void GenerateMove(List<int> moves, int fr, int to, bool add, int flag)
		{
			g_countMove++;
			if (((g_board[to] & 7) == pieceKing) || (((boardCheck[to] & g_lastCastle) == g_lastCastle) && ((g_lastCastle & maskCastle) > 0)))
				g_inCheck = true;
			else if (add)
				moves.Add(fr | (to << 8) | flag);
		}

		public List<int> GenerateValidMoves()
		{
			List<int> moves = new List<int>();
			List<int> am = GenerateAllMoves(whiteTurn, false);
			if (!g_inCheck)
				for (int i = am.Count - 1; i >= 0; i--)
				{
					int m = am[i];
					MakeMove(m);
					GenerateAllMoves(whiteTurn, true);
					if (!g_inCheck)
						moves.Add(m);
					UnmakeMove(m);
				}
			return moves;
		}

		public List<int> GenerateAllMoves(bool wt, bool attack)
		{
			adjMobility = 0;
			g_inCheck = false;
			usColor = wt ? colorWhite : colorBlack;
			enColor = wt ? colorBlack : colorWhite;
			int pieceM = 0;
			int pieceN = 0;
			int pieceB = 0;
			List<int> moves = new List<int>();
			for (int n = 0; n < 64; n++)
			{
				int fr = arrField[n];
				int f = g_board[fr];
				if ((f & usColor) > 0) f &= 7;
				else continue;
				g_countMove = 0;
				adjMobility += arrMaterial[g_phase, f];
				switch (f)
				{
					case 1:
						pieceM++;
						int del = wt ? -16 : 16;
						int to = fr + del;
						if (((g_board[to] & colorEmpty) > 0) && !attack)
						{
							GeneratePwnMoves(moves, fr, to, !attack, 0);
							if ((g_board[fr - del - del] == 0) && (g_board[to + del] & colorEmpty) > 0)
								GeneratePwnMoves(moves, fr, to + del, !attack, 0);
						}
						if ((g_board[to - 1] & enColor) > 0)
							GeneratePwnMoves(moves, fr, to - 1, true, 0);
						else if ((to - 1) == g_passing)
							GeneratePwnMoves(moves, fr, g_passing, true, moveflagPassing);
						else if ((g_board[to - 1] & colorEmpty) > 0)
							GeneratePwnMoves(moves, fr, to - 1, false, 0);
						if ((g_board[to + 1] & enColor) > 0)
							GeneratePwnMoves(moves, fr, to + 1, true, 0);
						else if ((to + 1) == g_passing)
							GeneratePwnMoves(moves, fr, g_passing, true, moveflagPassing);
						else if ((g_board[to + 1] & colorEmpty) > 0)
							GeneratePwnMoves(moves, fr, to + 1, false, 0);
						break;
					case 2:
						pieceN++;
						GenerateUniMoves(moves, attack, fr, arrDirKinght, 1);
						adjMobility += arrMobility[g_phase, f, g_countMove];
						break;
					case 3:
						pieceB++;
						GenerateUniMoves(moves, attack, fr, arrDirBishop, 7);
						adjMobility += arrMobility[g_phase, f, g_countMove];
						break;
					case 4:
						pieceM++;
						GenerateUniMoves(moves, attack, fr, arrDirRock, 7);
						adjMobility += arrMobility[g_phase, f, g_countMove];
						break;
					case 5:
						pieceM++;
						GenerateUniMoves(moves, attack, fr, arrDirQueen, 7);
						adjMobility += arrMobility[g_phase, f, g_countMove];
						break;
					case 6:
						GenerateUniMoves(moves, attack, fr, arrDirQueen, 1);
						int cr = wt ? g_castleRights : g_castleRights >> 2;
						if ((cr & 1) > 0)
							if (((g_board[fr + 1] & colorEmpty) > 0) && ((g_board[fr + 2] & colorEmpty) > 0))
								GenerateMove(moves, fr, fr + 2, true, moveflagCastleKing);
						if ((cr & 2) > 0)
							if (((g_board[fr - 1] & colorEmpty) > 0) && ((g_board[fr - 2] & colorEmpty) > 0) && ((g_board[fr - 3] & colorEmpty) > 0))
								GenerateMove(moves, fr, fr - 2, true, moveflagCastleQueen);
						adjMobility += arrMobility[g_phase, f, g_countMove];
						break;
				}
			}
			adjInsufficient = (pieceM == 0) && (pieceN + (pieceB << 1) < 3);
			if ((pieceN | pieceB | pieceM) == 0)
				adjMobility -= 64;
			if (pieceB > 1)
				adjMobility += 64;
			return moves;
		}

		void GeneratePwnMoves(List<int> moves, int fr, int to, bool add, int flag)
		{
			int y = to >> 4;
			if (((y == 4) || (y == 11)) && add)
			{
				GenerateMove(moves, fr, to, add, moveflagPromoteQueen);
				GenerateMove(moves, fr, to, add, moveflagPromoteRook);
				GenerateMove(moves, fr, to, add, moveflagPromoteBishop);
				GenerateMove(moves, fr, to, add, moveflagPromoteKnight);
			}
			else
				GenerateMove(moves, fr, to, add, flag);
		}

		void GenerateUniMoves(List<int> moves, bool attack, int fr, int[] dir, int count)
		{
			for (int n = 0; n < dir.Length; n++)
			{
				int to = fr;
				int c = count;
				while (c-- > 0)
				{
					to += dir[n];
					if ((g_board[to] & colorEmpty) > 0)
						GenerateMove(moves, fr, to, !attack, 0);
					else if ((g_board[to] & enColor) > 0)
					{
						GenerateMove(moves, fr, to, true, 0);
						break;
					}
					else
						break;
				}
			}
		}

		public int GetMoveFromString(string moveString)
		{
			List<int> moves = GenerateAllMoves(whiteTurn, false);
			for (int i = 0; i < moves.Count; i++)
			{
				string m = FormatMove(moves[i]);
				if (m == moveString)
					return moves[i];
			}
			return 0;
		}

		public void Initialize()
		{
			g_hash = RAND_32();
			for (int n = 0; n < undoStack.Length; n++)
				undoStack[n] = new CUndo();
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
					arrField[y * 8 + x] = (y + 4) * 16 + x + 4;
			for (int n = 0; n < 256; n++)
			{
				boardCheck[n] = 0;
				boardCastle[n] = 15;
				g_board[n] = 0;
				for (int p = 0; p < 16; p++)
					g_hashBoard[n, p] = RAND_32();
			}
			int[] arrCastleI = { 68, 72, 75, 180, 184, 187 };
			int[] arrCasteleV = { 7, 3, 11, 13, 12, 14 };
			int[] arrCheckI = { 71, 72, 73, 183, 184, 185 };
			int[] arrCheckV = { colorBlack | moveflagCastleQueen, colorBlack | maskCastle, colorBlack | moveflagCastleKing, colorWhite | moveflagCastleQueen, colorWhite | maskCastle, colorWhite | moveflagCastleKing };
			for (int n = 0; n < 6; n++)
			{
				boardCastle[arrCastleI[n]] = arrCasteleV[n];
				boardCheck[arrCheckI[n]] = arrCheckV[n];
			}
			for (int ph = 2; ph < 33; ph++)
			{
				double f = ph / 32.0;
				for (int p = 1; p < 7; p++)
				{
					arrMaterial[ph, p] = Convert.ToInt32(tmpMaterial[p, 0] * f + tmpMaterial[p, 1] * (1 - f));
					for (int n = 0; n < 28; n++)
						arrMobility[ph, p, n] = Convert.ToInt32(tmpMobility[p, n, 0] * f + tmpMobility[p, n, 1] * (1 - f));
				}
			}
		}

		public bool InitializeFromFen(string fen = defFen)
		{
			string[] chunks = fen.Split(' ');
			if (chunks.Length != 6)
				return false;
			g_phase = 0;
			for (int n = 0; n < 64; n++)
				g_board[arrField[n]] = colorEmpty;
			int row = 0;
			int col = 0;
			string pieces = chunks[0];
			for (int i = 0; i < pieces.Length; i++)
			{
				char c = pieces[i];
				if (c == '/')
				{
					row++;
					col = 0;
				}
				else if (c >= '0' && c <= '9')
				{
					for (int j = 0; j < Int32.Parse(c.ToString()); j++)
						col++;
				}
				else
				{
					g_phase++;
					char b = Char.ToLower(c);
					bool isWhite = b != c;
					int piece = isWhite ? colorWhite : colorBlack;
					int index = (row + 4) * 16 + col + 4;
					switch (b)
					{
						case 'p':
							piece |= piecePawn;
							break;
						case 'n':
							piece |= pieceKnight;
							break;
						case 'b':
							piece |= pieceBishop;
							break;
						case 'r':
							piece |= pieceRook;
							break;
						case 'q':
							piece |= pieceQueen;
							break;
						case 'k':
							piece |= pieceKing;
							break;
					}
					g_board[index] = piece;
					col++;
				}
			}
			whiteTurn = chunks[1] == "w";
			g_castleRights = 0;
			if (chunks[2].IndexOf('K') != -1)
				g_castleRights |= 1;
			if (chunks[2].IndexOf('Q') != -1)
				g_castleRights |= 2;
			if (chunks[2].IndexOf('k') != -1)
				g_castleRights |= 4;
			if (chunks[2].IndexOf('q') != -1)
				g_castleRights |= 8;
			g_passing = StrToSquare(chunks[3]);
			g_move50 = Int32.Parse(chunks[4]);
			g_moveNumber = Int32.Parse(chunks[5]);
			undoIndex = 0;
			return true;
		}

		public void MakeMove(int move)
		{
			int fr = move & 0xFF;
			int to = (move >> 8) & 0xFF;
			int flags = move & 0xFF0000;
			int piecefr = g_board[fr];
			int piece = piecefr & 0xf;
			int capi = to;
			g_captured = g_board[to];
			g_lastCastle = (move & maskCastle) | (piecefr & maskColor);
			if ((flags & moveflagCastleKing) > 0)
			{
				g_board[to - 1] = g_board[to + 1];
				g_board[to + 1] = colorEmpty;
			}
			else if ((flags & moveflagCastleQueen) > 0)
			{
				g_board[to + 1] = g_board[to - 2];
				g_board[to - 2] = colorEmpty;
			}
			else if ((flags & moveflagPassing) > 0)
			{
				capi = whiteTurn ? to + 16 : to - 16;
				g_captured = g_board[capi];
				g_board[capi] = colorEmpty;
			}
			CUndo undo = undoStack[undoIndex++];
			undo.captured = g_captured;
			undo.hash = g_hash;
			undo.passing = g_passing;
			undo.castle = g_castleRights;
			undo.move50 = g_move50;
			undo.lastCastle = g_lastCastle;
			g_hash ^= g_hashBoard[fr, piece];
			g_passing = 0;
			if ((g_captured & 0xF) > 0)
			{
				g_move50 = 0;
				g_phase--;
			}
			else if ((piece & 7) == piecePawn)
			{
				if (to == (fr + 32)) g_passing = (fr + 16);
				if (to == (fr - 32)) g_passing = (fr - 16);
				g_move50 = 0;
			}
			else
				g_move50++;
			if ((flags & moveflagPromotion) > 0)
			{
				int newPiece = piecefr & (~0x7);
				if ((flags & moveflagPromoteKnight) > 0)
					newPiece |= pieceKnight;
				else if ((flags & moveflagPromoteQueen) > 0)
					newPiece |= pieceQueen;
				else if ((flags & moveflagPromoteBishop) > 0)
					newPiece |= pieceBishop;
				else
					newPiece |= pieceRook;
				g_board[to] = newPiece;
				g_hash ^= g_hashBoard[to, newPiece & 7];
			}
			else
			{
				g_board[to] = g_board[fr];
				g_hash ^= g_hashBoard[to, piece];
			}
			g_board[fr] = colorEmpty;
			g_castleRights &= boardCastle[fr] & boardCastle[to];
			whiteTurn ^= true;
			g_moveNumber++;
		}

		public void UnmakeMove(int move)
		{
			int fr = move & 0xFF;
			int to = (move >> 8) & 0xFF;
			int flags = move & 0xFF0000;
			int piece = g_board[to];
			int capi = to;
			CUndo undo = undoStack[--undoIndex];
			g_passing = undo.passing;
			g_castleRights = undo.castle;
			g_move50 = undo.move50;
			g_lastCastle = undo.lastCastle;
			g_hash = undo.hash;
			int captured = undo.captured;
			if ((flags & moveflagCastleKing) > 0)
			{
				g_board[to + 1] = g_board[to - 1];
				g_board[to - 1] = colorEmpty;
			}
			else if ((flags & moveflagCastleQueen) > 0)
			{
				g_board[to - 2] = g_board[to + 1];
				g_board[to + 1] = colorEmpty;
			}
			if ((flags & moveflagPromotion) > 0)
			{
				piece = (g_board[to] & (~0x7)) | piecePawn;
				g_board[fr] = piece;
			}
			else g_board[fr] = g_board[to];
			if ((flags & moveflagPassing) > 0)
			{
				capi = whiteTurn ? to - 16 : to + 16;
				g_board[to] = colorEmpty;
			}
			g_board[capi] = captured;
			if ((captured & 7) > 0)
				g_phase++;
			whiteTurn ^= true;
			g_moveNumber--;
		}

		int Quiesce(List<int> mu, int depth, int depthL, int alpha, int beta, int score)
		{
			int myMobility = adjMobility;
			int alphaDe = 0;
			int index = mu.Count;
			string alphaFm = "";
			string alphaPv = "";
			if (alpha < score)
				alpha = score;
			if (alpha >= beta)
				alpha = score;
			else while (index-- > 0)
				{
					if ((++g_totalNodes & 0x1fff) == 0)
						g_stop = (((g_timeout > 0) && ((DateTime.Now - g_startTime).TotalMilliseconds > g_timeout)) || ((g_nodeout > 0) && (g_totalNodes > g_nodeout)));
					int cm = mu[index];
					MakeMove(cm);
					List<int> me = GenerateAllMoves(whiteTurn, true);
					int osScore = myMobility - adjMobility;
					g_depth = 0;
					g_pv = "";
					if (g_inCheck)
						osScore = -0xffff;
					else if (depth < depthL)
						osScore = -Quiesce(me, depth + 1, depthL, -beta, -alpha, -osScore);
					UnmakeMove(cm);
					if (g_stop) return -0xffff;
					if (alpha < osScore)
					{
						alpha = osScore;
						alphaDe = g_depth + 1;
						alphaFm = FormatMove(cm);
						alphaPv = alphaFm + ' ' + g_pv;
					}
					if (alpha >= beta) break;
				}
			g_depth = alphaDe;
			g_pv = alphaPv;
			return alpha;
		}

		int GetScore(List<int> mu, int depth, int depthL, int alpha, int beta)
		{
			bool myInsufficient = adjInsufficient;
			int myMobility = adjMobility;
			int n = mu.Count;
			int myMoves = n;
			int alphaDe = 0;
			string alphaFm = "";
			string alphaPv = "";
			while (n-- > 0)
			{
				if ((++g_totalNodes & 0x1fff) == 0)
					g_stop = ((depthL > 1) && (((g_timeout > 0) && ((DateTime.Now - g_startTime).TotalMilliseconds > g_timeout)) || ((g_nodeout > 0) && (g_totalNodes > g_nodeout))));
				int cm = mu[n];
				MakeMove(cm);
				List<int> me = GenerateAllMoves(whiteTurn, depth == depthL);
				g_depth = 0;
				g_pv = "";
				int osScore = myMobility - adjMobility;
				if (g_inCheck)
				{
					myMoves--;
					osScore = -0xffff;
				}
				else if ((g_move50 > 99) || IsRepetition() || ((myInsufficient || osScore < 0) && adjInsufficient))
					osScore = 0;
				else if (depth < depthL)
					osScore = -GetScore(me, depth + 1, depthL, -beta, -alpha);
				else
					osScore = -Quiesce(me, 1, depthL, -beta, -alpha, -osScore);
				UnmakeMove(cm);
				if (g_stop) return -0xffff;
				if (alpha < osScore)
				{
					alpha = osScore;
					alphaFm = FormatMove(cm);
					alphaPv = alphaFm + ' ' + g_pv;
					alphaDe = g_depth + 1;
					if (depth == 1)
					{
						if (osScore > 0xf000)
							g_scoreFm = "mate " + ((0xffff - osScore) >> 1);
						else if (osScore < -0xf000)
							g_scoreFm = "mate " + ((-0xfffe - osScore) >> 1);
						else
							g_scoreFm = "cp " + (osScore >> 2);
						bsIn = n;
						bsFm = alphaFm;
						bsPv = alphaPv;
						bsDepth = alphaDe;
						double t = (DateTime.Now - g_startTime).TotalMilliseconds;
						int nps = 0;
						if (t > 0)
							nps = Convert.ToInt32((g_totalNodes / t) * 1000);
						Console.WriteLine("info currmove " + bsFm + " currmovenumber " + n + " nodes " + g_totalNodes + " time " + t + " nps " + nps + " depth " + depthL + " seldepth " + alphaDe + " score " + g_scoreFm + " pv " + bsPv);
					}
				}
				if (alpha >= beta) break;
			}
			if (myMoves == 0)
			{
				GenerateAllMoves(whiteTurn ^ true, true);
				if (!g_inCheck)
				{
					alpha = 0;
				}
				else alpha = -0xffff + depth;
			}
			g_depth = alphaDe;
			g_pv = alphaPv;
			return alpha;
		}

		void Search(int depth, int time, int nodes)
		{
			List<int> mu = GenerateAllMoves(whiteTurn, false);
			bool myInsufficient = adjInsufficient;
			int myMobility = adjMobility;
			int m1 = mu.Count - 1;
			int depthL = depth > 0 ? depth : 1;
			g_stop = false;
			g_totalNodes = 0;
			g_timeout = time;
			g_nodeout = nodes;
			do
			{
				bsIn = m1;
				adjInsufficient = myInsufficient;
				adjMobility = myMobility;
				GetScore(mu, 1, depthL++, -0xffff, 0xffff);
				if (bsIn != m1)
				{
					int m = mu[m1];
					mu[m1] = mu[bsIn];
					mu[bsIn] = m;
				}
			} while (((depth == 0) || (depth > depthL)) && (g_depth <= bsDepth) && !g_stop && (m1 > 0));
			double t = (DateTime.Now - g_startTime).TotalMilliseconds;
			int nps = 0;
			if (t > 0)
				nps = Convert.ToInt32((g_totalNodes / t) * 1000);
			string[] ponder = bsPv.Split(' ');
			string pm = ponder.Length > 1 ? " ponder " + ponder[1] : "";
			Console.WriteLine("info nodes " + g_totalNodes + " time " + t + " nps " + nps);
			Console.WriteLine("bestmove " + bsFm + pm);
		}
	}
}

