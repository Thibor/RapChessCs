﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Int32;
using Move = System.Int32;
using Bitboard = System.UInt64;
using Square = System.Int32;

namespace NSRapchess
{
	public struct RScore
	{
		public bool insufficient;
		public int w;
		public int usCol;
		public int enCol;
		public ulong usBitboard;
		public ulong enBitboard;
		public ulong poBitboard;
		public int kingPosition;

		public RScore(Color color)
		{
			w = color >> 3;
			usCol = color;
			enCol = color ^ Constants.maskColor;
			usBitboard = CPosition.bitBoard[usCol];
			enBitboard = CPosition.bitBoard[enCol];
			poBitboard = ~usBitboard;
			kingPosition = CBitboard.Read(CPosition.bitBoard[Constants.pieceKing | usCol]);
			insufficient = CScore.GetInsufficient(usCol);
		}

	}

	internal class CScore
	{
		readonly public static Bitboard[,] bbRank = new ulong[8, 2];
		readonly static Bitboard[,] bbOutpost = new ulong[64, 2];
		readonly static Bitboard[,] bbSupported = new ulong[64, 2];
		readonly static Bitboard[] bbIsolated = new ulong[64];
		readonly static Bitboard[] bbDoubled = new ulong[64];
		readonly static Bitboard[] bbConnected = new ulong[64];
		readonly static Bitboard[,] bbPassed = new ulong[64, 2];
		readonly static Bitboard[,] bbKingSafety = new ulong[64, 2];
		readonly static Bitboard[] bbAttackBishop = new ulong[64];
		readonly static Bitboard[] bbAttackRook = new ulong[64];
		readonly static Bitboard[] bbAttackKing = new ulong[64];
		public static double[] arrCenter = new double[64];
		public static int[,] arrDistance = new int[64, 64];
		readonly static int[] bonusPassedRankMg = new int[8] { 0, 10, 17, 15, 62, 168, 276, 0 };
		readonly static int[] bonusPassedRankEg = new int[8] { 0, 28, 33, 41, 72, 177, 260, 0 };
		readonly static int[] bonusPassedFileMg = new int[8] { -33, -22, -11, 0, 0, -11, -22, -33 };
		readonly static int[] bonusPassedFileEg = new int[8] { -24, -16, -8, 0, 0, -8, -16, -24 };
		readonly static int[] bonusPawnRank = new int[8] { 0, 7, 8, 12, 29, 48, 86, 0 };
		readonly static int[,] bonusCenter = new int[8, 2] { { 0, 0 }, { 8, 0 }, { 8, 8 }, { 1, 2 }, {0,4}, { -8, 8 }, { -32, 32 }, { 0, 0 } };
		readonly static int[] bonusMaterial = new int[7] { 0, 100, 320, 330, 500, 900, 0 };
		public static int[] bonusMaterialDel = new int[7] { 0, 16, 16, 16, 20, 24, 0 };
		readonly static int[,] bonPawnConnected = new int[64, 2];
		public static int[,] bonMaterialMg = new int[16, 64];
		public static int[,] bonMaterialEg = new int[16, 64];
		public static int[,,] bonMaterialPhase = new int[16, 64, 33];
		public static int[][] bonusMobilityMg = new int[8][]{
	new int[]{},
	new int[]{},
	new int[9],
	new int[14],
	new int[15],
	new int[28],
	new int[]{},
	new int[]{}};
		public static int[][] bonusMobilityEg = new int[8][] {
	new int[]{},
	new int[]{},
	new int[9],
	new int[14],
	new int[15],
	new int[28],
	new int[]{},
	new int[]{}};

		public static void Init()
		{
			for (int a = 0; a < 64; a++)
				for (int b = 0; b < 64; b++)
				{
					int dx = Math.Abs((a & 7) - (b & 7));
					int dy = Math.Abs((a >> 3) - (b >> 3));
					arrDistance[a, b] = Math.Max(dx, dy);
				}
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
				{
					int iw = (y << 3) | x;
					CBitboard.Add(ref bbSupported[iw, 1], x + 1, y + 1);
					CBitboard.Add(ref bbSupported[iw, 1], x - 1, y + 1);
					CBitboard.Add(ref bbSupported[iw, 0], x + 1, y - 1);
					CBitboard.Add(ref bbSupported[iw, 0], x - 1, y - 1);
					CBitboard.Add(ref bbKingSafety[iw, 1], x, y - 1);
					CBitboard.Add(ref bbKingSafety[iw, 0], x, y + 1);
					CBitboard.Add(ref bbConnected[iw], x + 1, y);
					CBitboard.Add(ref bbConnected[iw], x - 1, y);
					CBitboard.Add(ref bbConnected[iw], x + 1, y + 1);
					CBitboard.Add(ref bbConnected[iw], x - 1, y + 1);
					CBitboard.Add(ref bbConnected[iw], x + 1, y - 1);
					CBitboard.Add(ref bbConnected[iw], x - 1, y - 1);
					CBitboard.Add(ref bbRank[y, 0], x, y);
					CBitboard.Add(ref bbRank[y, 1], x, 7 - y);
					for (int n = y - 1; n >= 0; n--)
					{
						CBitboard.Add(ref bbOutpost[iw, 1], x - 1, n);
						CBitboard.Add(ref bbOutpost[iw, 1], x + 1, n);
					}
					for (int n = y + 1; n < 8; n++)
					{
						CBitboard.Add(ref bbOutpost[iw, 0], x - 1, n);
						CBitboard.Add(ref bbOutpost[iw, 0], x + 1, n);
					}
					for (int n = 0; n < 8; n++)
					{
						if (n != y)
							CBitboard.Add(ref bbDoubled[iw], x, n);
						CBitboard.Add(ref bbIsolated[iw], x + 1, n);
						CBitboard.Add(ref bbIsolated[iw], x - 1, n);
						CBitboard.Add(ref bbAttackBishop[iw], x - n, y - n);
						CBitboard.Add(ref bbAttackBishop[iw], x - n, y + n);
						CBitboard.Add(ref bbAttackBishop[iw], x + n, y - n);
						CBitboard.Add(ref bbAttackBishop[iw], x + n, y + n);
						CBitboard.Add(ref bbAttackRook[iw], x - n, y);
						CBitboard.Add(ref bbAttackRook[iw], x + n, y);
						CBitboard.Add(ref bbAttackRook[iw], x, y - n);
						CBitboard.Add(ref bbAttackRook[iw], x, y + n);
					}
					CBitboard.Add(ref bbAttackKing[iw], x, y + 1);
					CBitboard.Add(ref bbAttackKing[iw], x, y - 1);
					CBitboard.Add(ref bbAttackKing[iw], x - 1, y + 1);
					CBitboard.Add(ref bbAttackKing[iw], x - 1, y - 1);
					CBitboard.Add(ref bbAttackKing[iw], x + 1, y + 1);
					CBitboard.Add(ref bbAttackKing[iw], x + 1, y - 1);
					CBitboard.Add(ref bbAttackKing[iw], x + 1, y);
					CBitboard.Add(ref bbAttackKing[iw], x - 1, y);
					for (int n = y + 1; n < 8; n++)
						CBitboard.Add(ref bbPassed[iw, 0], x, n);
					for (int n = y + 2; n < 8; n++)
					{
						CBitboard.Add(ref bbPassed[iw, 0], x - 1, n);
						CBitboard.Add(ref bbPassed[iw, 0], x + 1, n);
					}
					for (int n = y - 1; n >= 0; n--)
						CBitboard.Add(ref bbPassed[iw, 1], x, n);
					for (int n = y - 2; n >= 0; n--)
					{
						CBitboard.Add(ref bbPassed[iw, 1], x - 1, n);
						CBitboard.Add(ref bbPassed[iw, 1], x + 1, n);
					}
				}
			FillArray(bonusMobilityMg[2], -62, 33, 4, 3);
			FillArray(bonusMobilityEg[2], -81, 25, 4, 5);
			FillArray(bonusMobilityMg[3], -48, 98, 2, 16);
			FillArray(bonusMobilityEg[3], -59, 97, 3, 13);
			FillArray(bonusMobilityMg[4], -60, 62, 2, 2);
			FillArray(bonusMobilityEg[4], -78, 172, 2, 23);
			FillArray(bonusMobilityMg[5], -30, 116, 4, 20);
			FillArray(bonusMobilityEg[5], -48, 219, 3, 19);
			SetLevel(100);
		}

		static void FillArray(int[] arr, int start, int end, int pos, int val)
		{
			double GetDelY(int x1, int x2, int y1, int y2, int i)
			{
				double lenx = x2 - x1;
				double leny = y2 - y1;
				double delx = (i - x1) / lenx;
				return leny * delx;
			}
			int GetVal1(int x1, int x2, int y1, int y2, int i)
			{
				double lenx = x2 - x1;
				double leny = y1 - y2;
				double delx = (i - x1) / lenx;
				double dely1 = leny * (1.0 - delx);
				double dely2 = y2;
				double p = (i - x1) / lenx;
				double r = dely1 * (1.0 - p) + dely2 * p;
				return Convert.ToInt32(r);
			}
			int GetVal2(int x1, int x2, int y1, int y2, int i)
			{
				double lenx = x2 - x1;
				double dely1 = y1;
				double dely2 = GetDelY(x1, x2, y1, y2, i);
				double p = (i - x1) / lenx;
				double r = dely1 * (1.0 - p) + dely2 * p;
				return Convert.ToInt32(r);
			}
			for (int n = 0; n < arr.Length; n++)
			{
				if (n < pos)
					arr[n] = GetVal1(0, pos, start, val, n);
				if (n == pos)
					arr[n] = val;
				if (n > pos)
					arr[n] = GetVal2(pos, arr.Length - 1, val, end, n);
			}
		}

		public static bool GetInsufficient(int col)
		{
			if (CBitboard.Count(CPosition.bitBoard[col]) > 3)
				return false;
			if ((CPosition.bitBoard[col | Constants.piecePawn] | CPosition.bitBoard[col | Constants.pieceRook] | CPosition.bitBoard[col | Constants.pieceQueen]) > 0)
				return false;
			if (((CPosition.bitBoard[col | Constants.pieceBishop] & Constants.bbLight) > 0) && ((CPosition.bitBoard[col | Constants.pieceBishop] & Constants.bbDark) > 0))
				return false;
			if (((CPosition.bitBoard[col | Constants.pieceBishop]) > 0) && ((CPosition.bitBoard[col | Constants.pieceKnight]) > 0))
				return false;
			return true;
		}

		static int CountMovesBishop(RScore rs, int fr)
		{
			int count = 0;
			ulong bbFr = 1ul << fr;
			ulong bb = bbFr;
			int to = fr;
			while ((bb & CMovesGenerator.bbLastSE) == 0)
			{
				bb >>= 9;
				to -= 9;
				if ((bb & rs.enBitboard) > 0)
				{
					count++;
					break;
				}
				if ((bb & rs.usBitboard) > 0)
					break;
				count++;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastSW) == 0)
			{
				bb >>= 7;
				to -= 7;
				if ((bb & rs.enBitboard) > 0)
				{
					count++;
					break;
				}
				if ((bb & rs.usBitboard) > 0)
					break;
				count++;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastNW) == 0)
			{
				bb <<= 9;
				to += 9;
				if ((bb & rs.enBitboard) > 0)
				{
					count++;
					break;
				}
				if ((bb & rs.usBitboard) > 0)
					break;
				count++;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastNE) == 0)
			{
				bb <<= 7;
				to += 7;
				if ((bb & rs.enBitboard) > 0)
				{
					count++;
					break;
				}
				if ((bb & rs.usBitboard) > 0)
					break;
				count++;
			}
			return count;
		}

		static int CountMovesRook(RScore rs, int fr)
		{
			int count = 0;
			ulong bbFr = 1ul << fr;
			ulong bb = bbFr;
			int to = fr;
			while ((bb & CMovesGenerator.bbLastE) == 0)
			{
				bb >>= 1;
				to--;
				if ((bb & rs.enBitboard) > 0)
				{
					count++;
					break;
				}
				if ((bb & rs.usBitboard) > 0)
					break;
				count++;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastW) == 0)
			{
				bb <<= 1;
				to++;
				if ((bb & rs.enBitboard) > 0)
				{
					count++;
					break;
				}
				if ((bb & rs.usBitboard) > 0)
					break;
				count++;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastS) == 0)
			{
				bb >>= 8;
				to -= 8;
				if ((bb & rs.enBitboard) > 0)
				{
					count++;
					break;
				}
				if ((bb & rs.usBitboard) > 0)
					break;
				count++;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastN) == 0)
			{
				bb <<= 8;
				to += 8;
				if ((bb & rs.enBitboard) > 0)
				{
					count++;
					break;
				}
				if ((bb & rs.usBitboard) > 0)
					break;
				count++;
			}
			return count;
		}

		public static int MgEgToScore(int mg, int eg, int phase)
		{
			return (mg * phase + eg * (32 - phase)) / 32;
		}

		public static bool IsPassed(int piece, int fr)
		{
			return (CPosition.bitBoard[piece ^ Constants.maskColor] & bbPassed[fr, piece >> 3]) == 0;
		}

		public static void ScorePawn(RScore rs, ref int mg, ref int eg)
		{
			int piece = Constants.piecePawn | rs.usCol;
			Bitboard usbb = CPosition.bitBoard[Constants.piecePawn | rs.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				if ((CPosition.bitBoard[piece] & bbConnected[fr]) == 0)
				{
					mg -= bonPawnConnected[fr, rs.w];
					eg -= bonPawnConnected[fr, rs.w];
				}
				if ((CPosition.bitBoard[piece] & bbDoubled[fr]) > 0)
				{
					mg -= 16;
					eg -= 32;
				}
				if ((CPosition.bitBoard[piece] & bbIsolated[fr]) == 0)
				{
					mg -= 4;
					eg -= 8;
				}
				if (IsPassed(piece, fr))
				{
					mg += bonMaterialMg[rs.usCol, fr];
					eg += bonMaterialEg[rs.usCol, fr] - (arrDistance[rs.kingPosition, fr] << 3);
				}
				else
				{
					mg += bonMaterialMg[piece, fr];
					eg += bonMaterialEg[piece, fr] - (arrDistance[rs.kingPosition, fr] << 2);
				}
			}
		}


		static void ScoreKnight(RScore rs, ref int mg, ref int eg)
		{
			int piece = Constants.pieceKnight | rs.usCol;
			Bitboard usbb = CPosition.bitBoard[piece];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				mg += bonMaterialMg[piece, fr];
				eg += bonMaterialEg[piece, fr];
				Bitboard bb = CMovesGenerator.bbAttack[Constants.pieceKnight, fr] & rs.poBitboard;
				int count = CBitboard.Count(bb);
				mg += bonusMobilityMg[Constants.pieceKnight][count];
				eg += bonusMobilityEg[Constants.pieceKnight][count];
				if (((CPosition.bitBoard[rs.enCol | Constants.piecePawn] & bbOutpost[fr, rs.w]) == 0) && ((CPosition.bitBoard[rs.usCol | Constants.piecePawn] & bbSupported[fr, rs.w]) > 0))
				{
					mg += 33;
					eg += 3;
				}
			}
		}

		public static void ScoreBishop(RScore rs, ref int mg, ref int eg)
		{
			int score = 0;
			int piece = Constants.pieceBishop | rs.usCol;
			Bitboard usbb = CPosition.bitBoard[piece];
			if (((usbb & Constants.bbLight) > 0) && ((usbb & Constants.bbDark) > 0))
				score += 64;
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				mg += bonMaterialMg[piece, fr];
				eg += bonMaterialEg[piece, fr];
				int count = CountMovesBishop(rs, fr);
				mg += bonusMobilityMg[Constants.pieceBishop][count];
				eg += bonusMobilityEg[Constants.pieceBishop][count];
				if (((CPosition.bitBoard[rs.enCol | Constants.pieceQueen] | CPosition.bitBoard[rs.enCol | Constants.pieceKing]) & bbAttackBishop[fr]) > 0) score += 8;
				if (((CPosition.bitBoard[rs.enCol | Constants.piecePawn] & bbOutpost[fr, rs.w]) == 0) && ((CPosition.bitBoard[rs.usCol | Constants.piecePawn] & bbSupported[fr, rs.w]) > 0))
				{
					mg += 33;
					eg += 3;
				}
			}
			mg += score;
			eg += score;
		}

		public static void ScoreRook(RScore rs, ref int mg, ref int eg)
		{
			int score = 0;
			int piece = Constants.pieceRook | rs.usCol;
			Bitboard usbb = CPosition.bitBoard[piece];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				mg += bonMaterialMg[piece, fr];
				eg += bonMaterialEg[piece, fr];
				int count = CountMovesRook(rs, fr);
				mg += bonusMobilityMg[Constants.pieceRook][count];
				eg += bonusMobilityEg[Constants.pieceRook][count];
				if (((CPosition.bitBoard[rs.enCol | Constants.pieceQueen] | CPosition.bitBoard[rs.enCol | Constants.pieceKing]) & bbAttackRook[fr]) > 0) score += 8;
				if ((CPosition.bitBoard[rs.usCol | Constants.piecePawn] & bbDoubled[fr]) == 0)
					if ((CPosition.bitBoard[rs.enCol | Constants.piecePawn] & bbDoubled[fr]) == 0)
						score += 16;
					else
						score += 8;
			}
			mg += score;
			eg += score;
		}

		public static void ScoreQueen(RScore rs, ref int mg, ref int eg)
		{
			int score = 0;
			int piece = Constants.pieceQueen | rs.usCol;
			Bitboard usbb = CPosition.bitBoard[piece];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				mg += bonMaterialMg[piece, fr];
				eg += bonMaterialEg[piece, fr];
				int count = CountMovesBishop(rs, fr) + CountMovesRook(rs, fr);
				mg += bonusMobilityMg[Constants.pieceQueen][count];
				eg += bonusMobilityEg[Constants.pieceQueen][count];
			}
			mg += score;
			eg += score;
		}

		public static void ScoreKing(RScore rs, ref int mg, ref int eg)
		{
			int piece = Constants.pieceKing | rs.usCol;
			Bitboard usPawns = CPosition.bitBoard[rs.usCol | Constants.piecePawn];
			Bitboard enPawns = CPosition.bitBoard[rs.enCol | Constants.piecePawn];
			if ((bbAttackKing[rs.kingPosition] & (usPawns | enPawns)) > 0)
			{
				mg += 8;
				eg += 16;
			}
			if ((CPosition.bitBoard[rs.usCol | Constants.piecePawn] & bbKingSafety[rs.kingPosition, rs.w]) > 0)
			{
				mg += 16;
				eg += 8;
			}
			mg += bonMaterialMg[piece, rs.kingPosition];
			eg += bonMaterialEg[piece, rs.kingPosition];
		}

		public static int Score(RScore rs)
		{
			int mg = 0;
			int eg = 0;
			ScorePawn(rs, ref mg, ref eg);
			ScoreKnight(rs, ref mg, ref eg);
			ScoreBishop(rs, ref mg, ref eg);
			ScoreRook(rs, ref mg, ref eg);
			ScoreQueen(rs, ref mg, ref eg);
			ScoreKing(rs, ref mg, ref eg);
			int score = MgEgToScore(mg, eg, CPosition.phase);
			if (rs.insufficient)
				score >>= 2;
			return score;
		}

		public static int Score()
		{
			if (CPosition.usRS.insufficient && CPosition.enRS.insufficient)
				return 0;
			return Score(CPosition.usRS) - Score(CPosition.enRS);
		}

		public static void SetLevel(int level)
		{
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
				{
					int ib = (y << 3) | x;
					double center = 1.0 - (Math.Abs(x - 3.5) + Math.Abs(y - 3.5)) / 7.0;
					arrCenter[ib] = center;
					for (int r = 1; r < 7; r++)
					{
						bonMaterialMg[r, ib] = bonusMaterial[r] - bonusMaterialDel[r] + Convert.ToInt32(bonusCenter[r, 0] * center);
						bonMaterialEg[r, ib] = bonusMaterial[r] + bonusMaterialDel[r] + Convert.ToInt32(bonusCenter[r, 1] * center);
						if (r < 6)
							bonMaterialMg[r, ib] = (-bonMaterialMg[r, ib] * (50 - level)) / 50;
						if (r == Constants.piecePawn)
						{
							bonMaterialMg[r, ib] += bonusPawnRank[y];
							bonMaterialEg[r, ib] += bonusPawnRank[y];
							bonPawnConnected[ib, 0] = bonusPawnRank[y];
						}
						if (r == Constants.pieceKnight)
							if (y == 0)
								bonMaterialMg[r, ib] -= 8;
						if (r == Constants.pieceBishop)
						{
							if ((x == y) || (x == 7 - y))
								bonMaterialEg[r, ib] += 4;
							if (y == 0)
								bonMaterialMg[r, ib] -= 8;
						}
					}
					bonMaterialMg[0, ib] = bonusMaterial[1] - bonusMaterialDel[1] + bonusPassedRankMg[y] + bonusPassedFileMg[x];
					bonMaterialEg[0, ib] = bonusMaterial[1] + bonusMaterialDel[1] + bonusPassedRankEg[y] + bonusPassedFileEg[x];
				}
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
				{
					int iw = (y << 3) | x;
					int ib = ((7 - y) << 3) | x;
					for (int r = 0; r < 7; r++)
					{
						bonMaterialMg[8 + r, iw] = bonMaterialMg[r, ib];
						bonMaterialEg[8 + r, iw] = bonMaterialEg[r, ib];
					}
					bonPawnConnected[iw, 1] = bonPawnConnected[ib, 0];
				}
			for (int r = 0; r < 16; r++)
				for (int f = 0; f < 64; f++)
					for (int p = 0; p < 33; p++)
						bonMaterialPhase[r, f, p] = MgEgToScore(bonMaterialMg[r, f], bonMaterialEg[r, f], p);
		}

		static void ShowResult(string title, int wMg, int bMg, int wEg, int bEg)
		{
			Console.WriteLine($"{title} white ({wMg} {wEg}) black ({bMg} {bEg}) total ({wMg - bMg} {wEg - bEg})");
		}

		public static void Trace()
		{
			RScore wRS = new RScore(Constants.colWhite);
			RScore bRS = new RScore(Constants.colBlack);
			int wMg = 0;
			int bMg = 0;
			int wEg = 0;
			int bEg = 0;
			int scoreMg = 0;
			int scoreEg = 0;
			ScorePawn(wRS, ref wMg, ref wEg);
			ScorePawn(bRS, ref bMg, ref bEg);
			ShowResult("Pawns", wMg, bMg, wEg, bEg);
			scoreMg += wMg - bMg;
			scoreEg += wEg - bEg;
			wMg = bMg = wEg = bEg = 0;
			ScoreKnight(wRS, ref wMg, ref wEg);
			ScoreKnight(bRS, ref bMg, ref bEg);
			ShowResult("Knights", wMg, bMg, wEg, bEg);
			scoreMg += wMg - bMg;
			scoreEg += wEg - bEg;
			wMg = bMg = wEg = bEg = 0;
			ScoreBishop(wRS, ref wMg, ref wEg);
			ScoreBishop(bRS, ref bMg, ref bEg);
			ShowResult("Bishops", wMg, bMg, wEg, bEg);
			scoreMg += wMg - bMg;
			scoreEg += wEg - bEg;
			wMg = bMg = wEg = bEg = 0;
			ScoreRook(wRS, ref wMg, ref wEg);
			ScoreRook(bRS, ref bMg, ref bEg);
			ShowResult("Rooks", wMg, bMg, wEg, bEg);
			scoreMg += wMg - bMg;
			scoreEg += wEg - bEg;
			wMg = bMg = wEg = bEg = 0;
			ScoreQueen(wRS, ref wMg, ref wEg);
			ScoreQueen(bRS, ref bMg, ref bEg);
			ShowResult("Queens", wMg, bMg, wEg, bEg);
			scoreMg += wMg - bMg;
			scoreEg += wEg - bEg;
			wMg = bMg = wEg = bEg = 0;
			ScoreKing(wRS, ref wMg, ref wEg);
			ScoreKing(bRS, ref bMg, ref bEg);
			ShowResult("Kings", wMg, bMg, wEg, bEg);
			scoreMg += wMg - bMg;
			scoreEg += wEg - bEg;
			Console.WriteLine($"Score ({scoreMg} {scoreEg})");
		}

	}
}
