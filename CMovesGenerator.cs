using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Color = System.Int32;
using Move = System.Int32;
using Square = System.Int32;
using Bitboard = System.UInt64;
using Hash = System.UInt64;

namespace NSRapchess
{

	struct TPieceMove
	{
		public int maskPiece;
		public ulong maskMove;
	}

	internal static class CMovesGenerator
	{
		public static Bitboard bbLastN = 0;
		public static Bitboard bbLastS = 0;
		public static Bitboard bbLastW = 0;
		public static Bitboard bbLastE = 0;
		public static Bitboard bbLastNW = 0;
		public static Bitboard bbLastNE = 0;
		public static Bitboard bbLastSW = 0;
		public static Bitboard bbLastSE = 0;
		readonly static TPieceMove[,] arrVector = new TPieceMove[64, 64];
		public static ulong[,] bbAttack = new ulong[16, 64];
		readonly static List<int>[] arrDelta = new List<int>[16];
		readonly static int[] arrDelRook = new int[] { 1, -1, 8, -8 };
		readonly static int[] arrDelBishop = new int[] { 7, -7, 9, -9 };
		readonly static int[] arrDelKnight = new int[] { 15, -15, 17, -17, 6, -6, 10, -10 };

		public static void Init()
		{
			for (int n = 0; n < 8; n++)
			{
				CBitboard.Add(ref bbLastN, n, 7);
				CBitboard.Add(ref bbLastS, n, 0);
				CBitboard.Add(ref bbLastW, 7, n);
				CBitboard.Add(ref bbLastE, 0, n);
			}
			bbLastNW = bbLastN | bbLastW;
			bbLastNE = bbLastN | bbLastE;
			bbLastSW = bbLastS | bbLastW;
			bbLastSE = bbLastS | bbLastE;
			for (int piece = 0; piece < 16; piece++)
			{
				arrDelta[piece] = new List<int>();
				List<int> list = arrDelta[piece];
				switch (piece)
				{
					case Constants.piecePawn | Constants.colWhite:
						list.Add(7);
						list.Add(9);
						break;
					case Constants.piecePawn | Constants.colBlack:
						list.Add(-7);
						list.Add(-9);
						break;
					case Constants.pieceKnight | Constants.colWhite:
					case Constants.pieceKnight | Constants.colBlack:
						foreach (int del in arrDelKnight)
							list.Add(del);
						break;
					case Constants.pieceBishop | Constants.colWhite:
					case Constants.pieceBishop | Constants.colBlack:
						foreach (int del in arrDelBishop)
							list.Add(del);
						break;
					case Constants.pieceRook | Constants.colWhite:
					case Constants.pieceRook | Constants.colBlack:
						foreach (int del in arrDelRook)
							list.Add(del);
						break;
					case Constants.pieceQueen | Constants.colWhite:
					case Constants.pieceQueen | Constants.colBlack:
					case Constants.pieceKing | Constants.colWhite:
					case Constants.pieceKing | Constants.colBlack:
						foreach (int del in arrDelBishop)
							list.Add(del);
						foreach (int del in arrDelRook)
							list.Add(del);
						break;
				}
			}

			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++)
				{
					int iw = (y << 3) | x;
					CBitboard.Add(ref bbAttack[Constants.pieceKnight, iw], x - 1, y - 2);
					CBitboard.Add(ref bbAttack[Constants.pieceKnight, iw], x - 1, y + 2);
					CBitboard.Add(ref bbAttack[Constants.pieceKnight, iw], x + 1, y - 2);
					CBitboard.Add(ref bbAttack[Constants.pieceKnight, iw], x + 1, y + 2);
					CBitboard.Add(ref bbAttack[Constants.pieceKnight, iw], x - 2, y - 1);
					CBitboard.Add(ref bbAttack[Constants.pieceKnight, iw], x - 2, y + 1);
					CBitboard.Add(ref bbAttack[Constants.pieceKnight, iw], x + 2, y - 1);
					CBitboard.Add(ref bbAttack[Constants.pieceKnight, iw], x + 2, y + 1);

					CBitboard.Add(ref bbAttack[Constants.piecePawn | Constants.colWhite, iw], x - 1, y - 1);
					CBitboard.Add(ref bbAttack[Constants.piecePawn | Constants.colWhite, iw], x + 1, y - 1);
					CBitboard.Add(ref bbAttack[Constants.piecePawn, iw], x - 1, y + 1);
					CBitboard.Add(ref bbAttack[Constants.piecePawn, iw], x + 1, y + 1);

					CBitboard.Add(ref bbAttack[Constants.pieceKing, iw], x - 1, y - 1);
					CBitboard.Add(ref bbAttack[Constants.pieceKing, iw], x - 1, y + 1);
					CBitboard.Add(ref bbAttack[Constants.pieceKing, iw], x - 1, y);
					CBitboard.Add(ref bbAttack[Constants.pieceKing, iw], x + 1, y - 1);
					CBitboard.Add(ref bbAttack[Constants.pieceKing, iw], x + 1, y + 1);
					CBitboard.Add(ref bbAttack[Constants.pieceKing, iw], x + 1, y);
					CBitboard.Add(ref bbAttack[Constants.pieceKing, iw], x, y - 1);
					CBitboard.Add(ref bbAttack[Constants.pieceKing, iw], x, y + 1);
					for (int n = 1; n < 8; n++)
					{
						CBitboard.Add(ref bbAttack[Constants.pieceRook, iw], x + n, y);
						CBitboard.Add(ref bbAttack[Constants.pieceRook, iw], x - n, y);
						CBitboard.Add(ref bbAttack[Constants.pieceRook, iw], x, y + n);
						CBitboard.Add(ref bbAttack[Constants.pieceRook, iw], x, y - n);
						CBitboard.Add(ref bbAttack[Constants.pieceBishop, iw], x + n, y + n);
						CBitboard.Add(ref bbAttack[Constants.pieceBishop, iw], x + n, y - n);
						CBitboard.Add(ref bbAttack[Constants.pieceBishop, iw], x - n, y + n);
						CBitboard.Add(ref bbAttack[Constants.pieceBishop, iw], x - n, y - n);
					}
					bbAttack[Constants.pieceQueen, iw] = bbAttack[Constants.pieceBishop, iw] | bbAttack[Constants.pieceRook, iw];
					bbAttack[Constants.pieceKnight | Constants.colWhite, iw] = bbAttack[Constants.pieceKnight, iw];
					bbAttack[Constants.pieceBishop | Constants.colWhite, iw] = bbAttack[Constants.pieceBishop, iw];
					bbAttack[Constants.pieceRook | Constants.colWhite, iw] = bbAttack[Constants.pieceRook, iw];
					bbAttack[Constants.pieceQueen | Constants.colWhite, iw] = bbAttack[Constants.pieceQueen, iw];
					FillVector(Constants.piecePawn | Constants.colWhite, x, y, 1);
					FillVector(Constants.piecePawn | Constants.colBlack, x, y, 1);
					FillVector(Constants.pieceKnight | Constants.colWhite, x, y, 1);
					FillVector(Constants.pieceKnight | Constants.colBlack, x, y, 1);
					FillVector(Constants.pieceBishop | Constants.colWhite, x, y, 7);
					FillVector(Constants.pieceBishop | Constants.colBlack, x, y, 7);
					FillVector(Constants.pieceRook | Constants.colWhite, x, y, 7);
					FillVector(Constants.pieceRook | Constants.colBlack, x, y, 7);
					FillVector(Constants.pieceQueen | Constants.colWhite, x, y, 7);
					FillVector(Constants.pieceQueen | Constants.colBlack, x, y, 7);
					FillVector(Constants.pieceKing | Constants.colWhite, x, y, 1);
					FillVector(Constants.pieceKing | Constants.colBlack, x, y, 1);
				}
		}

		static void DelToXY(int del, out int dx, out int dy)
		{
			int x1 = 3;
			int y1 = 3;
			int p1 = (y1 << 3) | x1;
			int p2 = p1 + del;
			int x2 = p2 % 8;
			int y2 = p2 / 8;
			dx = x1 - x2;
			dy = y1 - y2;
			Debug.Assert((Math.Abs(dx) < 3) && (Math.Abs(dy) < 3));
		}

		static void FillVector(int piece, int fx, int fy, int dx, int dy, int length)
		{
			int x = fx;
			int y = fy;
			ulong maskMove = 0;
			for (int n = 0; n < length; n++)
			{
				ulong lastMask = maskMove;
				x += dx;
				y += dy;
				maskMove |= 1ul << ((y << 3) | x);
				int fr = (fy << 3) | fx;
				int to = (y << 3) | x;
				arrVector[fr, to].maskMove = lastMask;
				arrVector[fr, to].maskPiece |= 1 << piece;
			}
		}

		static void FillVector(int piece, int fx, int fy, int count)
		{
			List<int> list = arrDelta[piece];
			foreach (int del in list)
			{
				DelToXY(del, out int dx, out int dy);
				int x = fx + dx;
				int y = fy + dy;
				int length = 0;
				while ((x >= 0) && (y >= 0) && (x < 8) && (y < 8) && (length < count))
				{
					x += dx;
					y += dy;
					length++;
				}
				if (length > 0)
					FillVector(piece, fx, fy, dx, dy, length);
			}
		}

		public static bool IsSquareAttackedFromTo(Square fr, Square to)
		{
			TPieceMove pm = arrVector[fr, to];
			if ((~CPosition.emBitboard & pm.maskMove) > 0)
				return false;
			return (pm.maskPiece & (1 << CPosition.board[fr])) > 0;
		}

		public static bool IsSquareAttacked(Square target, Color enCol)
		{
			Color usCol = enCol ^ Constants.maskColor;
			if ((CPosition.bitBoard[enCol | Constants.piecePawn] & bbAttack[usCol | Constants.piecePawn, target]) > 0)
				return true;
			if ((CPosition.bitBoard[enCol | Constants.pieceKnight] & bbAttack[Constants.pieceKnight, target]) > 0)
				return true;
			if ((CPosition.bitBoard[enCol | Constants.pieceKing] & bbAttack[Constants.pieceKing, target]) > 0)
				return true;
			ulong bbEn = (CPosition.bitBoard[enCol | Constants.pieceBishop] | CPosition.bitBoard[enCol | Constants.pieceRook] | CPosition.bitBoard[enCol | Constants.pieceQueen]) & bbAttack[Constants.pieceQueen, target];
			while (bbEn > 0)
			{
				int fr = CBitboard.Pop(ref bbEn);
				if (IsSquareAttackedFromTo(fr, target))
					return true;
			}
			return false;
		}

		public static bool IsHashMoveValid(int move)
		{
			if (move == 0)
				return false;
			bool wt = CPosition.IsWhiteTurn();
			int fr = move & Constants.maskMove;
			int to = (move >> 6) & Constants.maskMove;
			int piece = CPosition.board[fr];
			int rank = piece & Constants.maskRank;
			if (!CPosition.IsSquareOur(fr) || CPosition.IsSquareOur(to))
				return false;
			bool isPassing = (move & Constants.moveflagPassing) > 0;
			bool isPromotion = (move & Constants.maskPromotion) > 0;
			if ((rank == Constants.piecePawn)||isPassing|| isPromotion)
			{
				if (rank != Constants.piecePawn)
					return false;
				if (isPassing)
					return (to == CChess.passant) && IsSquareAttackedFromTo(fr, to);
				if (CPosition.IsRank(fr, 7) && !isPromotion)
					return false;
				int del = wt ? -8 : 8;
				if ((to == fr + del) && CPosition.IsSquareEmpty(to))
					return true;
				if ((to == fr + del + 1) && CPosition.IsSquareEnemy(to))
					return true;
				if ((to == fr + del - 1) && CPosition.IsSquareEnemy(to))
					return true;
				if ((to == fr + del + del) && CPosition.IsSquareEmpty(fr + del) && CPosition.IsSquareEmpty(to) && CPosition.IsRank(fr, 2))
					return true;
				return false;
			}
			if (rank == Constants.pieceKing)
			{
				int cr = CChess.castleRights >> (wt ? 0 : 2);
				if ((move & Constants.moveflagCastleKing) > 0)
				{
					if (((cr & 1) > 0) && CPosition.IsSquareEmpty(fr + 1) && CPosition.IsSquareEmpty(fr + 2) && (!IsSquareAttacked(fr, CPosition.enCol)) && (!IsSquareAttacked(fr + 1, CPosition.enCol)) && (!IsSquareAttacked(fr + 2, CPosition.enCol)))
						return true;
					return false;
				}
				if ((move & Constants.moveflagCastleQueen) > 0)
				{
					if (((cr & 2) > 0) && CPosition.IsSquareEmpty(fr - 1) && CPosition.IsSquareEmpty(fr - 2) && (!IsSquareAttacked(fr, CPosition.enCol)) && (!IsSquareAttacked(fr - 1, CPosition.enCol)) && (!IsSquareAttacked(fr - 2, CPosition.enCol)))
						return true;
					return false;
				}
			}
			return IsSquareAttackedFromTo(fr, to);
		}

		public static void AddMove(MList moves, int fr, int to, int flag)
		{
			moves.Add(fr | (to << 6) | flag);
		}

		public static void GenerateMovesPawn(MList moves, int fr, int to, int flag)
		{
			if (((1ul << to) & Constants.bbPromotion) > 0)
			{
				AddMove(moves, fr, to, Constants.moveflagPromoteQueen);
				AddMove(moves, fr, to, Constants.moveflagPromoteRook);
				AddMove(moves, fr, to, Constants.moveflagPromoteBishop);
				AddMove(moves, fr, to, Constants.moveflagPromoteKnight);
			}
			else
				AddMove(moves, fr, to, flag);
		}

		public static void GenerateMovesBishop(MList moves, int fr, bool all)
		{
			ulong bbFr = 1ul << fr;
			ulong bb = bbFr;
			int to = fr;
			while ((bb & bbLastSE) == 0)
			{
				bb >>= 9;
				to -= 9;
				if ((bb & CPosition.enBitboard) > 0)
				{
					AddMove(moves, fr, to, 0);
					break;
				}
				if ((bb & CPosition.usBitboard) > 0)
					break;
				if (all)
					AddMove(moves, fr, to, 0);
			}
			bb = bbFr;
			to = fr;
			while ((bb & bbLastSW) == 0)
			{
				bb >>= 7;
				to -= 7;
				if ((bb & CPosition.enBitboard) > 0)
				{
					AddMove(moves, fr, to, 0);
					break;
				}
				if ((bb & CPosition.usBitboard) > 0)
					break;
				if (all)
					AddMove(moves, fr, to, 0);
			}
			bb = bbFr;
			to = fr;
			while ((bb & bbLastNW) == 0)
			{
				bb <<= 9;
				to += 9;
				if ((bb & CPosition.enBitboard) > 0)
				{
					AddMove(moves, fr, to, 0);
					break;
				}
				if ((bb & CPosition.usBitboard) > 0)
					break;
				if (all)
					AddMove(moves, fr, to, 0);
			}
			bb = bbFr;
			to = fr;
			while ((bb & bbLastNE) == 0)
			{
				bb <<= 7;
				to += 7;
				if ((bb & CPosition.enBitboard) > 0)
				{
					AddMove(moves, fr, to, 0);
					break;
				}
				if ((bb & CPosition.usBitboard) > 0)
					break;
				if (all)
					AddMove(moves, fr, to, 0);
			}
		}

		public static void GenerateMovesRook(MList moves, int fr, bool all)
		{
			ulong bbFr = 1ul << fr;
			ulong bb = bbFr;
			int to = fr;
			while ((bb & bbLastE) == 0)
			{
				bb >>= 1;
				to--;
				if ((bb & CPosition.enBitboard) > 0)
				{
					AddMove(moves, fr, to, 0);
					break;
				}
				if ((bb & CPosition.usBitboard) > 0)
					break;
				if (all)
					AddMove(moves, fr, to, 0);
			}
			bb = bbFr;
			to = fr;
			while ((bb & bbLastW) == 0)
			{
				bb <<= 1;
				to++;
				if ((bb & CPosition.enBitboard) > 0)
				{
					AddMove(moves, fr, to, 0);
					break;
				}
				if ((bb & CPosition.usBitboard) > 0)
					break;
				if (all)
					AddMove(moves, fr, to, 0);
			}
			bb = bbFr;
			to = fr;
			while ((bb & bbLastS) == 0)
			{
				bb >>= 8;
				to -= 8;
				if ((bb & CPosition.enBitboard) > 0)
				{
					AddMove(moves, fr, to, 0);
					break;
				}
				if ((bb & CPosition.usBitboard) > 0)
					break;
				if (all)
					AddMove(moves, fr, to, 0);
			}
			bb = bbFr;
			to = fr;
			while ((bb & bbLastN) == 0)
			{
				bb <<= 8;
				to += 8;
				if ((bb & CPosition.enBitboard) > 0)
				{
					AddMove(moves, fr, to, 0);
					break;
				}
				if ((bb & CPosition.usBitboard) > 0)
					break;
				if (all)
					AddMove(moves, fr, to, 0);
			}
		}

		public static MList GenerateMovesAttack()
		{
			MList moves = new MList();
			ulong paBitboard = 0;
			if (CChess.passant > 0)
				paBitboard = 1ul << CChess.passant;


			ulong usbb = CPosition.bitBoard[Constants.piecePawn | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				ulong bb = bbAttack[Constants.piecePawn | CPosition.usCol, fr] & (CPosition.enBitboard | paBitboard);
				while (bb > 0)
				{
					int to = CBitboard.Pop(ref bb);
					int flag = to == CChess.passant ? Constants.moveflagPassing : 0;
					GenerateMovesPawn(moves, fr, to, flag);
				}
			}
			usbb = CPosition.bitBoard[Constants.pieceKnight | CPosition.usCol];
			while (usbb > 0)
			{
				int count = 0;
				int fr = CBitboard.Pop(ref usbb);
				ulong bb = bbAttack[Constants.pieceKnight, fr] & CPosition.enBitboard;
				while (bb > 0)
				{
					int to = CBitboard.Pop(ref bb);
					AddMove(moves, fr, to, 0);
					count++;
				}
			}
			usbb = CPosition.bitBoard[Constants.pieceBishop | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				GenerateMovesBishop(moves, fr, false);
			}
			usbb = CPosition.bitBoard[Constants.pieceRook | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				GenerateMovesRook(moves, fr, false);
			}
			usbb = CPosition.bitBoard[Constants.pieceQueen | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				GenerateMovesBishop(moves, fr, false);
				GenerateMovesRook(moves, fr, false);
			}
			usbb = CPosition.bitBoard[Constants.pieceKing | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				ulong bb = bbAttack[Constants.pieceKing, fr] & CPosition.enBitboard;
				while (bb > 0)
				{
					int to = CBitboard.Pop(ref bb);
					AddMove(moves, fr, to, 0);
				}
			}
			return moves;
		}

		public static bool IsValidMove(Move move)
		{
			MList moves = GenerateMovesAll();
			for (int n = 0; n < moves.count; n++)
				if (moves.table[n].move == move)
					return true;
			return false;
		}

		public static MList GenerateMovesAll()
		{
			MList moves = new MList();
			bool wt = CPosition.IsWhiteTurn();
			ulong paBitboard = 0;
			if (CChess.passant > 0)
				paBitboard = 1ul << CChess.passant;

			ulong usbb = CPosition.bitBoard[Constants.piecePawn | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				ulong bb = bbAttack[Constants.piecePawn | CPosition.usCol, fr] & (CPosition.enBitboard | paBitboard);
				while (bb > 0)
				{
					int to = CBitboard.Pop(ref bb);
					int flag = to == CChess.passant ? Constants.moveflagPassing : 0;
					GenerateMovesPawn(moves, fr, to, flag);
				}

				int del = wt ? -8 : 8;
				int po = fr + del;
				if ((CPosition.emBitboard & (1ul << po)) > 0)
				{
					GenerateMovesPawn(moves, fr, po, 0);
					po += del;
					if (CPosition.IsRank(fr, 2) && ((CPosition.emBitboard & (1ul << po)) > 0))
						GenerateMovesPawn(moves, fr, po, 0);
				}
			}
			usbb = CPosition.bitBoard[Constants.pieceKnight | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				ulong bb = bbAttack[Constants.pieceKnight, fr] & ~CPosition.usBitboard;
				while (bb > 0)
				{
					int to = CBitboard.Pop(ref bb);
					AddMove(moves, fr, to, 0);
				}
			}
			usbb = CPosition.bitBoard[Constants.pieceBishop | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				GenerateMovesBishop(moves, fr, true);
			}
			usbb = CPosition.bitBoard[Constants.pieceRook | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				GenerateMovesRook(moves, fr, true);
			}
			usbb = CPosition.bitBoard[Constants.pieceQueen | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				GenerateMovesBishop(moves, fr, true);
				GenerateMovesRook(moves, fr, true);
			}
			usbb = CPosition.bitBoard[Constants.pieceKing | CPosition.usCol];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				ulong bb = bbAttack[Constants.pieceKing, fr] & ~CPosition.usBitboard;
				while (bb > 0)
				{
					int to = CBitboard.Pop(ref bb);
					AddMove(moves, fr, to, 0);
				}
				int cr = CChess.castleRights >> (wt ? 0 : 2);
				if ((cr & 1) > 0)
					if ((((0b11ul << (wt ? 61 : 5)) | CPosition.emBitboard) == CPosition.emBitboard) && !IsSquareAttacked(fr, CPosition.enCol) && !IsSquareAttacked(fr + 1, CPosition.enCol) && !IsSquareAttacked(fr + 2, CPosition.enCol))
						AddMove(moves, fr, fr + 2, Constants.moveflagCastleKing);
				if ((cr & 2) > 0)
					if ((((0b111ul << (wt ? 57 : 1)) | CPosition.emBitboard) == CPosition.emBitboard) && !IsSquareAttacked(fr, CPosition.enCol) && !IsSquareAttacked(fr - 1, CPosition.enCol) && !IsSquareAttacked(fr - 2, CPosition.enCol))
						AddMove(moves, fr, fr - 2, Constants.moveflagCastleQueen);
			}
			return moves;
		}

	}
}
