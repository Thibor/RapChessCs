using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Namespce
{
	class CChess
	{
		const int piecePawn = 0x01;
		const int pieceKnight = 0x02;
		const int pieceBishop = 0x03;
		const int pieceRook = 0x04;
		const int pieceQueen = 0x05;
		const int pieceKing = 0x06;
		const int colorWhite = 0x08;
		const int colorBlack = 0x10;
		const int colorEmpty = 0x20;
		const int moveflagPassing = 0x02 << 16;
		const int moveflagCastleKing = 0x04 << 16;
		const int moveflagCastleQueen = 0x08 << 16;
		const int moveflagPromoteQueen = pieceQueen << 20;
		const int moveflagPromoteRook = pieceRook << 20;
		const int moveflagPromoteBishop = pieceBishop << 20;
		const int moveflagPromoteKnight = pieceKnight << 20;
		const int maskPromotion = moveflagPromoteQueen | moveflagPromoteRook | moveflagPromoteBishop | moveflagPromoteKnight;
		int inTime = 0;
		int inDepth = 0;
		int inNodes = 0;
		int g_castleRights = 0xf;
		ulong g_hash = 0;
		int g_passing = 0;
		public int g_move50 = 0;
		public int g_moveNumber = 0;
		public int g_phase = 32;
		int g_totalNodes = 0;
		int g_timeout = 0;
		int g_depthout = 0;
		int g_nodeout = 0;
		int g_mainDepth = 1;
		bool g_stop = false;
		public int undoIndex = 0;
		int[] kingPos = new int[2];
		int[] arrFieldL = new int[64];
		int[] arrFieldS = new int[256];
		int[] g_board = new int[256];
		ulong[,] g_hashBoard = new ulong[256, 16];
		int[] boardCastle = new int[256];
		public bool whiteTurn = true;
		string bsFm = "";
		string bsPv = "";
		ulong[] bitBoard = new ulong[16];
		ulong[] bbAttackBishop = new ulong[64];
		ulong[] bbAttackRook = new ulong[64];
		ulong[] bbAttackQueen = new ulong[64];
		ulong[,] bbDistance = new ulong[64, 2];
		ulong[] bbFile = new ulong[64];
		ulong[] bbIsolated = new ulong[8];
		ulong[,] bbOutpost = new ulong[64, 2];
		ulong[,] bbPassed = new ulong[64, 2];
		ulong[,] bbProtector = new ulong[64, 2];
		ulong[,] bbSupported = new ulong[64, 2];
		int[,] bonDistance = new int[2, 8] { { 0, 4, 8, 12, 16, 20, 24, 0 }, { 0, 24, 20, 16, 12, 8, 4, 0 } };
		int[] tmpIsolated = new int[2] { -5, -15 };
		int[] tmpDoubled = new int[2] { -11, -56 };
		int[] tmpBackward = new int[2] { -9, -24 };
		int[] tmpPawnKing = new int[2] { 0, -64 };
		int[,] tmpOutpost = new int[3, 2] { { 32, 10 }, { 30, 21 }, { 60, 42 } };
		int[,] tmpCenter = new int[6, 2] { { 4, 2 }, { 8, 8 }, { 4, 8 }, { -8, 8 }, { -8, 0xf }, { -8, 8 } };
		int[] tmpPassedFile = new int[8] { 0, -8, -16, -24, -24, -16, -8, 0 };
		int[] tmpPassedRank = new int[8] { 0, 5, 5, 30, 70, 150, 250, 0 };
		int[,] tmpMaterial = new int[6, 2] { { 171, 240 }, { 764, 848 }, { 826, 891 }, { 1282, 1373 }, { 2526, 2646 }, { 0xffff, 0xffff } };
		int[,,] tmpMobility = new int[7, 28, 2] {
			{ { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{ { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{ {-75,-76 }, {-57,-54 }, {-9,-28 }, {-2,-10 }, {6,5 }, {14,12 }, {22, 26 }, { 29,29 }, { 36, 29 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{ { -48,-59 }, {-20,-23 }, {16, -3 }, {26, 13 }, {38, 24 }, {51, 42 }, {55, 54 }, {63, 57 }, {63, 65 }, {68, 73 }, {81, 78 }, {81, 86 }, {91, 88 }, {98, 97 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{ {-58,-76 }, {-27,-18 }, {-15, 28 }, { -10, 55 }, {-5, 69 }, { -2, 82 }, { 9,112 }, {16,118 }, { 30,132 }, {29,142 }, {32,155 }, {38,165 }, { 46,166 }, { 48,169 }, {58,171 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
			{{ -39, -36 },{-21, -15 },{3, 8 },{3, 18 },{14, 34 },{22, 54 },{28, 61 },{41, 73 },{43, 79 },{48, 92 },{56, 94 },{60, 104 },{60, 113 },{66, 120 },{67, 123 },{70, 126 },{71, 133 },{73, 136 },{79, 140 },{88, 143 },{88, 148 },{99, 166 },{102, 170 },{102, 175 },{106, 184 },{109, 191 },{113, 206 },{116, 212} },
			{ { 90,9 }, { 80,8 }, { 70, 7 }, { 60, 6 }, { 50, 5 }, { 40, 4 }, { 30, 3 }, { 20, 2 }, { 10, 1 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } }
			};
		int[,,] bonMobility = new int[33, 7, 28];
		int[] arrIsolated = new int[33];
		int[] arrDoubled = new int[33];
		int[] arrBackward = new int[33];
		int[] arrPawnKing = new int[33];
		int[,,] bonConnected = new int[33, 8, 2];
		int[,] bonOutpost = new int[33, 3];
		int[,,] bonPosition = new int[33, 16, 256];
		int[,] bonPassed = new int[64, 2];
		int[] arrDirKinght = { 14, -14, 18, -18, 31, -31, 33, -33 };
		int[] arrDirBishop = { 15, -15, 17, -17 };
		int[] arrDirRook = { 1, -1, 16, -16 };
		int[] arrDirQueen = { 1, -1, 15, -15, 16, -16, 17, -17 };
		ulong[,] arrAttack = new ulong[16, 64];
		CUndo[] undoStack = new CUndo[0xfff];
		Thread startThread;
		public Stopwatch stopwatch = Stopwatch.StartNew();
		private static readonly Random random = new Random();
		public CSynStop synStop = new CSynStop();

		public CChess()
		{
			g_hash = RAND_32();
			for (int n = 0; n < undoStack.Length; n++)
				undoStack[n] = new CUndo();
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
				{
					int fieldS = y * 8 + x;
					int fieldL = (y + 4) * 16 + x + 4;
					arrFieldL[fieldS] = fieldL;
					arrFieldS[fieldL] = fieldS;
				}
			for (int n = 0; n < 256; n++)
			{
				boardCastle[n] = 15;
				g_board[n] = 0;
				for (int p = 0; p < 16; p++)
					g_hashBoard[n, p] = RAND_32();
			}
			int[] arrRankCon = new int[8] { 0, 7, 8, 12, 29, 48, 86, 0 };
			int[] arrCastleI = { 68, 72, 75, 180, 184, 187 };
			int[] arrCasteleV = { 7, 3, 11, 13, 12, 14 };
			for (int n = 0; n < 6; n++)
				boardCastle[arrCastleI[n]] = arrCasteleV[n];
			for (int ph = 2; ph < 33; ph++)
			{
				double f = ph / 32.0;
				for (int p = 1; p < 7; p++)
					for (int n = 0; n < 28; n++)
						bonMobility[ph, p, n] = Convert.ToInt32(tmpMobility[p, n, 0] * f + tmpMobility[p, n, 1] * (1 - f));
			}
			for (int ph = 2; ph < 33; ph++)
			{
				arrIsolated[ph] = GetPhaseValue(ph, tmpIsolated[0], tmpIsolated[1]);
				arrDoubled[ph] = GetPhaseValue(ph, tmpDoubled[0], tmpDoubled[1]);
				arrBackward[ph] = GetPhaseValue(ph, tmpBackward[0], tmpBackward[1]);
				arrPawnKing[ph] = GetPhaseValue(ph, tmpPawnKing[0], tmpPawnKing[1]);
				for (int n = 0; n < 3; n++)
					bonOutpost[ph, n] = GetPhaseValue(ph, tmpOutpost[n, 0], tmpOutpost[n, 1]);
				for (int rank = 1; rank < 7; rank++)
				{
					int a = arrRankCon[rank];
					int b = (arrRankCon[rank] * (rank - 2)) / 4;
					int v = GetPhaseValue(ph,a,b);
					bonConnected[ph, rank, 0] = v;
					bonConnected[ph, 7 - rank, 1] = v;
				}

			}
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++)
				{
					int i = (y << 3) | x;
					bonPassed[i, 0] = tmpPassedRank[y] + tmpPassedFile[x];
					bonPassed[i, 1] = tmpPassedRank[7 - y] + tmpPassedFile[x];
				}
			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++)
				{
					int iw = (y << 3) | x;
					for (int n = 0; n < 8; n++)
						if (n != y)
							CBitBoard.Add(ref bbFile[iw], x, n);
					for (int n = 1; n < 3; n++)
						for (int y2 = y - n; y2 <= y + n; y2++)
							for (int x2 = x - n; x2 <= x + n; x2++)
								CBitBoard.Add(ref bbDistance[iw, n - 1], x2, y2);
					CBitBoard.Add(ref bbProtector[iw, 1], x, y + 1);
					CBitBoard.Add(ref bbProtector[iw, 1], x, y + 2);
					CBitBoard.Add(ref bbProtector[iw, 1], x + 1, y + 1);
					CBitBoard.Add(ref bbProtector[iw, 1], x + 1, y + 2);
					CBitBoard.Add(ref bbProtector[iw, 1], x - 1, y + 1);
					CBitBoard.Add(ref bbProtector[iw, 1], x - 1, y + 2);
					CBitBoard.Add(ref bbProtector[iw, 0], x, y - 1);
					CBitBoard.Add(ref bbProtector[iw, 0], x, y - 2);
					CBitBoard.Add(ref bbProtector[iw, 0], x + 1, y - 1);
					CBitBoard.Add(ref bbProtector[iw, 0], x + 1, y - 2);
					CBitBoard.Add(ref bbProtector[iw, 0], x - 1, y - 1);
					CBitBoard.Add(ref bbProtector[iw, 0], x - 1, y - 2);
					for (int n = y - 1; n >= 0; n--)
					{
						CBitBoard.Add(ref bbOutpost[iw, 1], x - 1, n);
						CBitBoard.Add(ref bbOutpost[iw, 1], x + 1, n);
					}
					for (int n = y + 1; n < 8; n++)
					{
						CBitBoard.Add(ref bbOutpost[iw, 0], x - 1, n);
						CBitBoard.Add(ref bbOutpost[iw, 0], x + 1, n);
					}
					for (int n = 1; n < 8; n++)
					{
						CBitBoard.Add(ref bbAttackBishop[iw], x - n, y - n);
						CBitBoard.Add(ref bbAttackBishop[iw], x - n, y + n);
						CBitBoard.Add(ref bbAttackBishop[iw], x + n, y - n);
						CBitBoard.Add(ref bbAttackBishop[iw], x + n, y + n);
						CBitBoard.Add(ref bbAttackRook[iw], x - n, y);
						CBitBoard.Add(ref bbAttackRook[iw], x + n, y);
						CBitBoard.Add(ref bbAttackRook[iw], x, y - n);
						CBitBoard.Add(ref bbAttackRook[iw], x, y + n);
					}
					bbAttackQueen[iw] = bbAttackBishop[iw] | bbAttackRook[iw];
					for (int n = y - 1; n >= 0; n--)
						CBitBoard.Add(ref bbPassed[iw, 1], x, n);
					for (int n = y - 2; n >= 0; n--)
					{
						CBitBoard.Add(ref bbPassed[iw, 1], x - 1, n);
						CBitBoard.Add(ref bbPassed[iw, 1], x + 1, n);
					}
					for (int n = y + 1; n < 8; n++)
						CBitBoard.Add(ref bbPassed[iw, 0], x, n);
					for (int n = y + 2; n < 8; n++)
					{
						CBitBoard.Add(ref bbPassed[iw, 0], x - 1, n);
						CBitBoard.Add(ref bbPassed[iw, 0], x + 1, n);
					}
					CBitBoard.Add(ref bbIsolated[x], x - 1, y);
					CBitBoard.Add(ref bbIsolated[x], x + 1, y);
					CBitBoard.Add(ref bbSupported[iw, 1], x - 1, y + 1);
					CBitBoard.Add(ref bbSupported[iw, 1], x + 1, y + 1);
					CBitBoard.Add(ref bbSupported[iw, 0], x - 1, y - 1);
					CBitBoard.Add(ref bbSupported[iw, 0], x + 1, y - 1);

					CBitBoard.Add(ref arrAttack[pieceKnight, iw], x - 1, y - 2);
					CBitBoard.Add(ref arrAttack[pieceKnight, iw], x - 1, y + 2);
					CBitBoard.Add(ref arrAttack[pieceKnight, iw], x + 1, y - 2);
					CBitBoard.Add(ref arrAttack[pieceKnight, iw], x + 1, y + 2);
					CBitBoard.Add(ref arrAttack[pieceKnight, iw], x - 2, y - 1);
					CBitBoard.Add(ref arrAttack[pieceKnight, iw], x - 2, y + 1);
					CBitBoard.Add(ref arrAttack[pieceKnight, iw], x + 2, y - 1);
					CBitBoard.Add(ref arrAttack[pieceKnight, iw], x + 2, y + 1);

					CBitBoard.Add(ref arrAttack[piecePawn | colorWhite, iw], x - 1, y - 1);
					CBitBoard.Add(ref arrAttack[piecePawn | colorWhite, iw], x + 1, y - 1);
					CBitBoard.Add(ref arrAttack[piecePawn, iw], x - 1, y + 1);
					CBitBoard.Add(ref arrAttack[piecePawn, iw], x + 1, y + 1);

					CBitBoard.Add(ref arrAttack[pieceKing, iw], x - 1, y - 1);
					CBitBoard.Add(ref arrAttack[pieceKing, iw], x - 1, y + 1);
					CBitBoard.Add(ref arrAttack[pieceKing, iw], x - 1, y);
					CBitBoard.Add(ref arrAttack[pieceKing, iw], x + 1, y - 1);
					CBitBoard.Add(ref arrAttack[pieceKing, iw], x + 1, y + 1);
					CBitBoard.Add(ref arrAttack[pieceKing, iw], x + 1, y);
					CBitBoard.Add(ref arrAttack[pieceKing, iw], x, y - 1);
					CBitBoard.Add(ref arrAttack[pieceKing, iw], x, y + 1);

				}
			FillBonPosition();
		}

		public void FillBonPosition(int level = 100)
		{
			for (int ph = 2; ph < 33; ph++)
				for (int rank = 1; rank < 7; rank++)
					for (int y = 0; y < 8; y++)
						for (int x = 0; x < 8; x++)
						{
							int nw = (y + 4) * 16 + (x + 4);
							int nb = (11 - y) * 16 + (x + 4);
							int cx = Math.Min(x, 7 - x) + 1;
							int cy = Math.Min(y, 7 - y) + 1;
							int center = (cx * cy) - 1;
							int a = tmpCenter[rank - 1, 0] * center;
							int b = tmpCenter[rank - 1, 1] * center;
							a += tmpMaterial[rank - 1, 0];
							b += tmpMaterial[rank - 1, 1];
							if ((rank == 3) && ((x == y) || (x == (7 - y))))
							{
								a += 16;
								b += 16;
							}
							if (rank < 6)
								a = (a * level - a * (100 - level)) / 100;
							int v = GetPhaseValue(ph, a, b);
							bonPosition[ph, 8 | rank, nw] = v;
							bonPosition[ph, rank, nb] = v;
						}
		}

		bool GetStop()
		{
			return ((g_timeout > 0) && (stopwatch.Elapsed.TotalMilliseconds > g_timeout)) || ((g_depthout > 0) && (g_mainDepth > g_depthout)) || ((g_nodeout > 0) && (g_totalNodes > g_nodeout));
		}

		int GetPhaseValue(double phase, int a, int b)
		{
			double f = phase / 32.0;
			double v = a * f + b * (1 - f);
			return Convert.ToInt32(v);
		}

		public void SetFen(string fen)
		{
			synStop.SetStop(false);
			g_phase = 0;
			for (int n = 0; n < 64; n++)
				g_board[arrFieldL[n]] = colorEmpty;
			for (int n = 0; n < 16; n++)
				bitBoard[n] = 0;
			if (fen == "") fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
			string[] chunks = fen.Split(' ');
			int y = 0;
			int x = 0;
			string pieces = chunks[0];
			for (int i = 0; i < pieces.Length; i++)
			{
				char c = pieces[i];
				if (c == '/')
				{
					y++;
					x = 0;
				}
				else if (c >= '0' && c <= '9')
					x += Int32.Parse(c.ToString());
				else
				{
					g_phase++;
					bool isWhite = Char.IsUpper(c);
					int piece = isWhite ? colorWhite : colorBlack;
					int index = (y + 4) * 16 + x + 4;
					switch (Char.ToLower(c))
					{
						case 'p':
							piece |= piecePawn;
							break;
						case 'b':
							piece |= pieceBishop;
							break;
						case 'n':
							piece |= pieceKnight;
							break;
						case 'r':
							piece |= pieceRook;
							break;
						case 'q':
							piece |= pieceQueen;
							break;
						case 'k':
							kingPos[isWhite ? 1 : 0] = index;
							piece |= pieceKing;
							break;
					}
					g_board[index] = piece;
					int bi = (y << 3) | x;
					CBitBoard.Add(ref bitBoard[piece & 0xf], bi);
					x++;
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
			g_passing = 0;
			if (chunks[3].IndexOf('-') == -1)
				g_passing = StrToSquare(chunks[3]);
			g_move50 = 0;
			g_moveNumber = Int32.Parse(chunks[5]);
			if (g_moveNumber > 0) g_moveNumber--;
			g_moveNumber *= 2;
			if (!whiteTurn) g_moveNumber++;
			undoIndex = 0;
		}

		ulong RAND_32()
		{
			return ((ulong)random.Next() << 32) | ((ulong)random.Next() << 0);
		}

		string EmoToUmo(int emo)
		{
			string result = SquareToStr(emo & 0xFF) + SquareToStr((emo >> 8) & 0xFF);
			int promotion = emo & maskPromotion;
			if (promotion > 0)
			{
				if (promotion == moveflagPromoteQueen) result += 'q';
				else if (promotion == moveflagPromoteRook) result += 'r';
				else if (promotion == moveflagPromoteBishop) result += 'b';
				else result += 'n';
			}
			return result;
		}

		public int UmoToEmo(string umo)
		{
			List<int> moves = GenerateAllMoves(whiteTurn);
			for (int i = 0; i < moves.Count; i++)
				if (EmoToUmo(moves[i]) == umo)
					return moves[i];
			return 0;
		}

		string SquareToStr(int square)
		{
			int x = (square & 0xf) - 4;
			int y = (square >> 4) - 4;
			string xs = "abcdefgh";
			string ys = "87654321";
			return $"{xs[x]}{ys[y]}";
		}

		int StrToSquare(string s)
		{
			string xs = "abcdefgh";
			string ys = "87654321";
			int x = xs.IndexOf(s[0]);
			int y = ys.IndexOf(s[1]);
			return ((y + 4) << 4) | (x + 4);
		}

		bool IsRepetition()
		{
			for (int n = undoIndex - 2; n >= undoIndex - g_move50; n -= 2)
				if (undoStack[n].hash == g_hash)
					return true;
			return false;
		}

		bool IsAttacked(bool wt, int to)
		{
			int uc = wt ? colorWhite : colorBlack;
			int ec = wt ? colorBlack : colorWhite;
			if ((bitBoard[(ec | piecePawn) & 0xf] & arrAttack[(uc | piecePawn) & 0xf, arrFieldS[to]]) > 0)
				return true;
			if ((bitBoard[(ec | pieceKnight) & 0xf] & arrAttack[pieceKnight, arrFieldS[to]]) > 0)
				return true;
			if ((bitBoard[(ec | pieceKing) & 0xf] & arrAttack[pieceKing, arrFieldS[to]]) > 0)
				return true;
			foreach (int d in arrDirBishop)
			{
				int fr = to + d;
				while (g_board[fr] > 0)
				{
					if ((g_board[fr] & 0x1f) == (ec | pieceBishop) || (g_board[fr] & 0x1f) == (ec | pieceQueen))
						return true;
					if ((g_board[fr] & colorEmpty) == 0)
						break;
					fr += d;
				}
			}
			foreach (int d in arrDirRook)
			{
				int fr = to + d;
				while (g_board[fr] > 0)
				{
					if ((g_board[fr] & 0x1f) == (ec | pieceRook) || (g_board[fr] & 0x1f) == (ec | pieceQueen))
						return true;
					if ((g_board[fr] & colorEmpty) == 0)
						break;
					fr += d;
				}
			}
			return false;
		}

		void AddMove(List<int> moves, int fr, int to, int flag, ref int bestRank)
		{
			int rank = g_board[to] & 7;
			int promotion = flag & maskPromotion;
			if (promotion > 0)
				rank += (promotion >> 20) - 1;
			int m = fr | (to << 8) | flag;
			if ((rank > 0) || ((flag & moveflagPassing) > 0))
			{
				if (bestRank <= rank)
				{
					bestRank = rank;
					moves.Insert(0, m);
				}
				else
					moves.Insert(1, m);
			}
			else
				moves.Add(m);
		}

		List<int> GenerateAllMoves(bool wt)
		{
			int bestRank = 0;
			int usColor = wt ? colorWhite : colorBlack;
			int enColor = wt ? colorBlack : colorWhite;
			List<int> moves = new List<int>(64);
			for (int n = 0; n < 64; n++)
			{
				int fr = arrFieldL[n];
				int f = g_board[fr];
				if ((f & usColor) > 0)
					f &= 7;
				else
					continue;
				switch (f)
				{
					case 1:
						int del = wt ? -16 : 16;
						int to = fr + del;
						if ((g_board[to] & colorEmpty) > 0)
						{
							GeneratePwnMoves(moves, fr, to, 0, ref bestRank);
							if ((g_board[fr - del - del] == 0) && (g_board[to + del] & colorEmpty) > 0)
								GeneratePwnMoves(moves, fr, to + del, 0, ref bestRank);
						}
						if ((g_board[to - 1] & enColor) > 0)
							GeneratePwnMoves(moves, fr, to - 1, 0, ref bestRank);
						else if ((to - 1) == g_passing)
							GeneratePwnMoves(moves, fr, g_passing, moveflagPassing, ref bestRank);
						if ((g_board[to + 1] & enColor) > 0)
							GeneratePwnMoves(moves, fr, to + 1, 0, ref bestRank);
						else if ((to + 1) == g_passing)
							GeneratePwnMoves(moves, fr, g_passing, moveflagPassing, ref bestRank);
						break;
					case 2:
						GenerateUniMoves(enColor, moves, false, fr, arrDirKinght, 1, ref bestRank);
						break;
					case 3:
						GenerateUniMoves(enColor, moves, false, fr, arrDirBishop, 7, ref bestRank);
						break;
					case 4:
						GenerateUniMoves(enColor, moves, false, fr, arrDirRook, 7, ref bestRank);
						break;
					case 5:
						GenerateUniMoves(enColor, moves, false, fr, arrDirQueen, 7, ref bestRank);
						break;
					case 6:
						GenerateUniMoves(enColor, moves, false, fr, arrDirQueen, 1, ref bestRank);
						int cr = wt ? g_castleRights : g_castleRights >> 2;
						if ((cr & 1) > 0)
							if (((g_board[fr + 1] & colorEmpty) > 0) && ((g_board[fr + 2] & colorEmpty) > 0) && !IsAttacked(wt, fr) && !IsAttacked(wt, fr + 1) && !IsAttacked(wt, fr + 2))
								AddMove(moves, fr, fr + 2, moveflagCastleKing, ref bestRank);
						if ((cr & 2) > 0)
							if (((g_board[fr - 1] & colorEmpty) > 0) && ((g_board[fr - 2] & colorEmpty) > 0) && ((g_board[fr - 3] & colorEmpty) > 0) && !IsAttacked(wt, fr) && !IsAttacked(wt, fr - 1) && !IsAttacked(wt, fr - 2))
								AddMove(moves, fr, fr - 2, moveflagCastleQueen, ref bestRank);
						break;
				}
			}
			return moves;
		}

		List<int> GenerateLegalMoves(bool wt)
		{
			List<int> moves = GenerateAllMoves(wt);
			for (int n = moves.Count - 1; n >= 0; n--)
			{
				int cm = moves[n];
				string umo = EmoToUmo(cm);
				MakeMove(cm);
				int kp = kingPos[wt ? 1 : 0];
				if (IsAttacked(wt, kp))
					moves.RemoveAt(n);
				UnmakeMove(cm);
			}
			return moves;
		}


		void GeneratePwnMoves(List<int> moves, int fr, int to, int flag, ref int bestRank)
		{
			int y = to >> 4;
			if ((y == 4) || (y == 11))
			{
				AddMove(moves, fr, to, moveflagPromoteQueen, ref bestRank);
				AddMove(moves, fr, to, moveflagPromoteRook, ref bestRank);
				AddMove(moves, fr, to, moveflagPromoteBishop, ref bestRank);
				AddMove(moves, fr, to, moveflagPromoteKnight, ref bestRank);
			}
			else
				AddMove(moves, fr, to, flag, ref bestRank);
		}

		int CountUniMoves(int enColor, int fr, int[] dir, int count)
		{
			int result = 0;
			for (int n = 0; n < dir.Length; n++)
			{
				int to = fr;
				int c = count;
				while (c-- > 0)
				{
					to += dir[n];
					if ((g_board[to] & colorEmpty) > 0)
						result++;
					else if ((g_board[to] & enColor) > 0)
					{
						result++;
						break;
					}
					else
						break;
				}
			}
			return result;
		}

		int GenerateUniMoves(int enColor, List<int> moves, bool attack, int fr, int[] dir, int count, ref int bestRank)
		{
			int result = 0;
			for (int n = 0; n < dir.Length; n++)
			{
				int to = fr;
				int c = count;
				while (c-- > 0)
				{
					to += dir[n];
					if ((g_board[to] & colorEmpty) > 0)
					{
						result++;
						if (!attack)
							AddMove(moves, fr, to, 0, ref bestRank);
					}
					else if ((g_board[to] & enColor) > 0)
					{
						result++;
						AddMove(moves, fr, to, 0, ref bestRank);
						break;
					}
					else
						break;
				}
			}
			return result;
		}

		public void MakeMove(int move)
		{
			int fr = move & 0xFF;
			int to = (move >> 8) & 0xFF;
			int flags = move & 0xFF0000;
			int piecefr = g_board[fr];
			int piece = piecefr & 0xf;
			int rank = piecefr & 7;
			int capi = to;
			int captured = g_board[to];
			CBitBoard.Del(ref bitBoard[piece], arrFieldS[fr]);
			if ((flags & moveflagCastleKing) > 0)
			{
				g_board[to - 1] = g_board[to + 1];
				g_board[to + 1] = colorEmpty;
				int secPiece = (piece & colorWhite) | pieceRook;
				CBitBoard.Del(ref bitBoard[secPiece], arrFieldS[to + 1]);
				CBitBoard.Add(ref bitBoard[secPiece], arrFieldS[to - 1]);
			}
			else if ((flags & moveflagCastleQueen) > 0)
			{
				g_board[to + 1] = g_board[to - 2];
				g_board[to - 2] = colorEmpty;
				int secPiece = (piece & colorWhite) | pieceRook;
				CBitBoard.Del(ref bitBoard[secPiece], arrFieldS[to - 2]);
				CBitBoard.Add(ref bitBoard[secPiece], arrFieldS[to + 1]);
			}
			else if ((flags & moveflagPassing) > 0)
			{
				capi = whiteTurn ? to + 16 : to - 16;
				captured = g_board[capi];
				g_board[capi] = colorEmpty;
			}
			ref CUndo undo = ref undoStack[undoIndex++];
			undo.captured = captured;
			undo.hash = g_hash;
			undo.passing = g_passing;
			undo.castle = g_castleRights;
			undo.move50 = g_move50;
			g_hash ^= g_hashBoard[fr, piece];
			g_passing = 0;
			if (captured != colorEmpty)
			{
				g_move50 = 0;
				g_phase--;
				CBitBoard.Del(ref bitBoard[captured & 0xf], arrFieldS[capi]);
			}
			else if (rank == piecePawn)
			{
				if (to == (fr + 32)) g_passing = (fr + 16);
				if (to == (fr - 32)) g_passing = (fr - 16);
				g_move50 = 0;
			}
			else
				g_move50++;
			if ((flags & maskPromotion) > 0)
			{
				int newPiece = ((piecefr & (~0x7)) | (flags >> 20));
				g_board[to] = newPiece;
				CBitBoard.Add(ref bitBoard[newPiece & 0xf], arrFieldS[to]);
				g_hash ^= g_hashBoard[to, newPiece & 0xf];
			}
			else
			{
				g_board[to] = g_board[fr];
				CBitBoard.Add(ref bitBoard[piece], arrFieldS[to]);
				g_hash ^= g_hashBoard[to, piece];
			}
			if (rank == pieceKing)
				kingPos[whiteTurn ? 1 : 0] = to;
			g_board[fr] = colorEmpty;
			g_castleRights &= boardCastle[fr] & boardCastle[to];
			whiteTurn ^= true;
			g_moveNumber++;
		}

		void UnmakeMove(int move)
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
			g_hash = undo.hash;
			int captured = undo.captured;
			CBitBoard.Del(ref bitBoard[piece & 0xf], arrFieldS[to]);
			if ((flags & moveflagCastleKing) > 0)
			{
				g_board[to + 1] = g_board[to - 1];
				g_board[to - 1] = colorEmpty;
				int secPiece = (piece & colorWhite) | pieceRook;
				CBitBoard.Del(ref bitBoard[secPiece], arrFieldS[to - 1]);
				CBitBoard.Add(ref bitBoard[secPiece], arrFieldS[to + 1]);
			}
			else if ((flags & moveflagCastleQueen) > 0)
			{
				g_board[to - 2] = g_board[to + 1];
				g_board[to + 1] = colorEmpty;
				int secPiece = (piece & colorWhite) | pieceRook;
				CBitBoard.Del(ref bitBoard[secPiece], arrFieldS[to + 1]);
				CBitBoard.Add(ref bitBoard[secPiece], arrFieldS[to - 2]);
			}
			if ((flags & maskPromotion) > 0)
			{
				int secPiece = (piece & (~0x7)) | piecePawn;
				g_board[fr] = secPiece;
				CBitBoard.Add(ref bitBoard[secPiece & 0xf], arrFieldS[fr]);
			}
			else
			{
				g_board[fr] = g_board[to];
				CBitBoard.Add(ref bitBoard[piece & 0xf], arrFieldS[fr]);
				if ((g_board[fr] & 0x7) == pieceKing)
					kingPos[whiteTurn ? 0 : 1] = fr;
			}
			if ((flags & moveflagPassing) > 0)
			{
				capi = whiteTurn ? to - 16 : to + 16;
				g_board[to] = colorEmpty;
				CBitBoard.Del(ref bitBoard[piece & 0xf], arrFieldS[to]);
			}
			g_board[capi] = captured;
			if (captured != colorEmpty)
			{
				g_phase++;
				CBitBoard.Add(ref bitBoard[captured & 0xf], arrFieldS[capi]);
			}
			whiteTurn ^= true;
			g_moveNumber--;
		}

		int GetColorScore(bool wt, out bool insufficient, List<int> moves = null)
		{
			int score = 0;
			int bestRank = 0;
			int pieceM = 0;
			int pieceN = 0;
			int pieceB = 0;
			int w = wt ? 1 : 0;
			int usColor = wt ? colorWhite : colorBlack;
			int enColor = wt ? colorBlack : colorWhite;
			int colorShUs = usColor & 0xf;
			int colorShEn = enColor & 0xf;
			int pawnDistance = 8;
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
				{
					int n = (y << 3) | x;
					int fr = arrFieldL[n];
					int f = g_board[fr];
					if ((f & usColor) == 0)
						continue;
					int piece = f & 0xf;
					int rank = f & 7;
					score += bonPosition[g_phase, piece, fr];
					int c;
					switch (rank)
					{
						case 1:
							pieceM++;
							if ((bitBoard[piece] & bbIsolated[x]) == 0)
								score -= 16;
							if ((bitBoard[piece] & bbFile[n]) > 0)
								score -= 8;
							if ((bitBoard[piece ^ 8] & bbPassed[n, w]) == 0)
							{
								score += bonPassed[n, w];
								if ((bitBoard[colorShUs | pieceKing] & bbDistance[n, 0]) > 0)
									score += (bonDistance[w, y] << 1);
								else if ((bitBoard[colorShUs | pieceKing] & bbDistance[n, 1]) > 0)
									score += bonDistance[w, y];
								if ((bitBoard[colorShEn | pieceKing] & bbDistance[n, 0]) > 0)
									score -= bonDistance[w ^ 1, y];
								else if ((bitBoard[colorShEn | pieceKing] & bbDistance[n, 1]) > 0)
									score -= (bonDistance[w ^ 1, y]) >> 1;
							}
							if (moves != null)
							{
								int del = wt ? -16 : 16;
								int to = fr + del;
								if ((g_board[to - 1] & enColor) > 0)
									GeneratePwnMoves(moves, fr, to - 1, 0, ref bestRank);
								else if ((to - 1) == g_passing)
									GeneratePwnMoves(moves, fr, g_passing, moveflagPassing, ref bestRank);
								if ((g_board[to + 1] & enColor) > 0)
									GeneratePwnMoves(moves, fr, to + 1, 0, ref bestRank);
								else if ((to + 1) == g_passing)
									GeneratePwnMoves(moves, fr, g_passing, moveflagPassing, ref bestRank);
							}
							if (pawnDistance > 1)
							{
								int kp = kingPos[wt ? 1 : 0];
								int dx = Math.Abs((kp & 0xf) - (fr & 0xf));
								int dy = Math.Abs((kp >> 4) - (fr >> 4));
								int di = Math.Max(dx, dy);
								if (pawnDistance > di)
									pawnDistance = di;
							}
							break;
						case 2:
							pieceN++;
							if (((bitBoard[colorShEn | piecePawn] & bbOutpost[n, w]) == 0) && ((bitBoard[colorShUs | piecePawn] & bbSupported[n, w]) > 0))
								score += 16;
							c = moves == null ? CountUniMoves(enColor, fr, arrDirKinght, 1) : GenerateUniMoves(enColor, moves, true, fr, arrDirKinght, 1, ref bestRank);
							score += bonMobility[g_phase, rank, c];
							break;
						case 3:
							pieceB++;
							if (((bitBoard[colorShEn | piecePawn] & bbOutpost[n, w]) == 0) && ((bitBoard[colorShUs | piecePawn] & bbSupported[n, w]) > 0))
								score += 16;
							c = moves == null ? CountUniMoves(enColor, fr, arrDirBishop, 7) : GenerateUniMoves(enColor, moves, true, fr, arrDirBishop, 7, ref bestRank);
							score += bonMobility[g_phase, rank, c];
							break;
						case 4:
							pieceM++;
							if (((bitBoard[colorShEn | pieceKing]) & bbFile[n]) > 0)
								score += 8;
							if (((bitBoard[colorShEn | pieceQueen]) & bbFile[n]) > 0)
								score += 8;
							if ((bitBoard[colorShUs | piecePawn] & bbFile[n]) == 0)
								if ((bitBoard[colorShEn | piecePawn] & bbFile[n]) > 0)
									score += 8;
								else
									score += 16;
							c = moves == null ? CountUniMoves(enColor, fr, arrDirRook, 7) : GenerateUniMoves(enColor, moves, true, fr, arrDirRook, 7, ref bestRank);
							score += bonMobility[g_phase, rank, c];
							break;
						case 5:
							pieceM++;
							if ((bitBoard[colorShEn | pieceBishop] & bbAttackBishop[n]) > 0)
								score -= 8;
							if ((bitBoard[colorShEn | pieceRook] & bbAttackRook[n]) > 0)
								score -= 8;
							c = moves == null ? CountUniMoves(enColor, fr, arrDirQueen, 7) : GenerateUniMoves(enColor, moves, true, fr, arrDirQueen, 7, ref bestRank);
							score += bonMobility[g_phase, rank, c];
							break;
						case 6:
							if (((bitBoard[colorShEn | pieceBishop] | bitBoard[colorShEn | pieceQueen]) & bbAttackBishop[n]) > 0)
								score -= 12;
							if (((bitBoard[colorShEn | pieceRook] | bitBoard[colorShEn | pieceQueen]) & bbAttackBishop[n]) > 0)
								score -= 12;
							if (moves != null)
								GenerateUniMoves(enColor, moves, true, fr, arrDirQueen, 1, ref bestRank);
							break;
					}
				}
			if (pieceB > 1)
				score += 64;
			if (pawnDistance < 8)
				score += pawnDistance * arrPawnKing[g_phase];
			insufficient = ((pieceM == 0) && (pieceN + (pieceB << 1) < 3));
			return score;
		}

		int Quiesce(int ply, int depth, int alpha, int beta, int enScore, bool enInsufficient, ref int alDe, ref string alPv)
		{
			int neDe = 0;
			string nePv = "";
			List<int> mu = new List<int>(64);
			int usScore = GetColorScore(whiteTurn, out bool usInsufficient, mu);
			if (enInsufficient && usInsufficient)
				return 0;
			int score = usScore - enScore;
			if (usInsufficient != enInsufficient)
				score += usInsufficient ? -400 : 400;
			if (depth < 1)
				return score;
			if (score >= beta)
				return beta;
			if (score > alpha)
				alpha = score;
			int index = mu.Count;
			while (index-- > 0)
			{
				alDe = 0;
				alPv = "";
				if ((++g_totalNodes & 0xfff) == 0)
					if (GetStop() || synStop.GetStop())
						g_stop = true;
				int cm = mu[index];
				MakeMove(cm);
				if (IsAttacked(!whiteTurn, kingPos[whiteTurn ? 0 : 1]))
					score = -0xffff;
				else
					score = -Quiesce(ply + 1, depth - 1, -beta, -alpha, usScore, usInsufficient, ref alDe, ref alPv);
				UnmakeMove(cm);
				if (g_stop) return -0xffff;
				if (score >= beta)
					return beta;
				if (score > alpha)
				{
					nePv = $"{EmoToUmo(cm)} {alPv}";
					neDe = alDe + 1;
					alpha = score;
					if (alpha >= beta)
						return beta;
				}
			}
			alDe = neDe;
			alPv = nePv;
			return alpha;
		}

		int Search(List<int> usm, int ply, int depth, int alpha, int beta, ref int alDe, ref string alPv, bool nullMove)
		{
			int neDe = 0;
			string nePv = "";
			int countCheck = 0;
			bool usCheck = IsAttacked(whiteTurn, kingPos[whiteTurn ? 1 : 0]);

			if (usCheck)
				depth++;

			alpha = Math.Max(alpha, -0xffff + ply);
			beta = Math.Min(beta, 0xffff - ply);
			if (alpha >= beta)
				return alpha;

			if (depth <= 0)
			{
				int enScore = GetColorScore(!whiteTurn, out bool enInsufficient);
				return Quiesce(1, g_mainDepth, alpha, beta, enScore, enInsufficient, ref alDe, ref alPv);
			}

			if (nullMove && !usCheck && (depth > 0) && (ply > 1) && (g_phase > 10))
			{
				whiteTurn = !whiteTurn;
				int d = 0;
				string pv = "";
				List<int> me = GenerateAllMoves(whiteTurn);
				int score = -Search(me, ply + 1, depth >> 2, -beta, -beta + 1, ref d, ref pv, false);
				whiteTurn = !whiteTurn;
				if (score >= beta)
					return beta;
			}

			for (int n = 0; n < usm.Count; n++)
			{
				alDe = 0;
				alPv = "";
				int cm = usm[n];
				if ((++g_totalNodes & 0xfff) == 0)
					if (GetStop() || synStop.GetStop())
						g_stop = depth > 1;
				MakeMove(cm);
				int score = -0xffff;
				if (IsAttacked(!whiteTurn, kingPos[whiteTurn ? 0 : 1]))
					countCheck++;
				else if ((g_move50 > 99) || IsRepetition())
					score = 0;
				else
				{
					List<int> me = GenerateAllMoves(whiteTurn);
					score = -Search(me, ply + 1, depth - 1, -beta, -alpha, ref alDe, ref alPv, nullMove);
				}
				UnmakeMove(cm);
				if (g_stop) return 0;
				if (score >= beta)
					return beta;
				if (score > alpha)
				{
					string alphaFm = EmoToUmo(cm);
					nePv = $"{alphaFm} {alPv}";
					neDe = alDe + 1;
					alpha = score;
					if (ply == 1)
					{
						string scFm = score > 0xf000 ? $"mate {(0xffff - score) >> 1}" : ((score < -0xf000) ? $"mate {(-0xfffe - score) >> 1}" : $"cp {score}");
						bsFm = alphaFm;
						bsPv = nePv;
						double t = stopwatch.Elapsed.TotalMilliseconds;
						double nps = t > 0 ? (g_totalNodes / t) * 1000 : 0;
						Console.WriteLine($"info currmove {bsFm} currmovenumber {n + 1} nodes {g_totalNodes} time {Convert.ToInt64(t)} nps {Convert.ToInt64(nps)} depth {g_mainDepth} seldepth {neDe} score {scFm} pv {nePv}");
						usm.RemoveAt(n);
						usm.Insert(0, cm);
					}
				}
			}
			if (countCheck == usm.Count)
				if (usCheck)
					alpha = -0xffff + ply;
				else
					alpha = 0;
			alDe = neDe;
			alPv = nePv;
			return alpha;
		}

		public void Start(int depth, int time, int nodes)
		{
			List<int> mu = GenerateLegalMoves(whiteTurn);
			if (mu.Count == 0)
			{
				Console.WriteLine($"info string no moves");
				return;
			}
			int depthLimit = mu.Count == 1 ? 3 : 100;
			g_stop = false;
			g_totalNodes = 0;
			g_timeout = time;
			g_depthout = depth;
			g_nodeout = nodes;
			g_mainDepth = 1;
			do
			{
				int alDe = 0;
				string alPv = "";
				int score = Search(mu, 1, g_mainDepth, -0xffff, 0xffff, ref alDe, ref alPv, true);
				double t = stopwatch.Elapsed.TotalMilliseconds;
				double nps = t > 0 ? (g_totalNodes / t) * 1000 : 0;
				Console.WriteLine($"info depth {g_mainDepth} nodes {g_totalNodes} time {Convert.ToInt64(t)} nps {Convert.ToInt64(nps)}");
				if (++g_mainDepth > depthLimit)
					break;
				if ((score < -0xf000) || (score > 0xf000))
					break;
			} while (!GetStop() && !synStop.GetStop());
			string[] ponder = bsPv.Split(' ');
			string pm = ponder.Length > 1 ? $" ponder {ponder[1]}" : "";
			Console.WriteLine($"bestmove {bsFm}{pm}");
		}

		public void Thread()
		{
			Start(inDepth, inTime, inNodes);
		}

		public void StartThread(int depth, int time, int nodes)
		{
			inDepth = depth;
			inTime = time;
			inNodes = nodes;
			startThread = new Thread(Thread);
			startThread.Start();
		}

	}
}