using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Color = System.Int32;
using Move = System.Int32;
using Bitboard = System.UInt64;
using Square = System.Int32;

namespace NSRapchess
{

	struct TPieceMove
	{
		public int maskPiece;
		public ulong maskMove;
	}

	internal static class MoveGenerator
	{
		static TPieceMove[,] arrVector = new TPieceMove[64, 64];
		public static ulong[,] bbAttack = new ulong[16, 64];
		static List<int>[] arrDelta = new List<int>[16];
		static int[] arrDelRook = new int[] { 1, -1, 8, -8 };
		static int[] arrDelBishop = new int[] { 7, -7, 9, -9 };
		static int[] arrDelKnight = new int[] { 15, -15, 17, -17, 6, -6, 10, -10 };

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
			if ((~Position.emBitboard & pm.maskMove) > 0)
				return false;
			return (pm.maskPiece & (1 << Position.board[fr])) > 0;
		}

		public static bool IsSquareAttacked(Square target, Color enCol)
		{
			Color usCol = enCol ^ Constants.maskColor;
			if ((Position.bitBoard[enCol | Constants.piecePawn] & bbAttack[usCol | Constants.piecePawn, target]) > 0)
				return true;
			if ((Position.bitBoard[enCol | Constants.pieceKnight] & bbAttack[Constants.pieceKnight, target]) > 0)
				return true;
			if ((Position.bitBoard[enCol | Constants.pieceKing] & bbAttack[Constants.pieceKing, target]) > 0)
				return true;
			ulong bbEn = (Position.bitBoard[enCol | Constants.pieceBishop] | Position.bitBoard[enCol | Constants.pieceRook] | Position.bitBoard[enCol | Constants.pieceQueen]) & bbAttack[Constants.pieceQueen, target];
			while (bbEn > 0)
			{
				int fr = Bit.Pop(ref bbEn);
				if (IsSquareAttackedFromTo(fr, target))
					return true;
			}
			return false;
		}

		public static void Init()
		{
			for (int piece = 0; piece < 16; piece++)
			{
				arrDelta[piece] = new List<int>();
				List<int> list = arrDelta[piece];
				switch (piece)
				{
					case Constants.piecePawn | Constants.colWhite:
						list.Add(-7);
						list.Add(-9);
						break;
					case Constants.piecePawn | Constants.colBlack:
						list.Add(7);
						list.Add(9);
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
					CBitBoard.Add(ref bbAttack[Constants.pieceKnight, iw], x - 1, y - 2);
					CBitBoard.Add(ref bbAttack[Constants.pieceKnight, iw], x - 1, y + 2);
					CBitBoard.Add(ref bbAttack[Constants.pieceKnight, iw], x + 1, y - 2);
					CBitBoard.Add(ref bbAttack[Constants.pieceKnight, iw], x + 1, y + 2);
					CBitBoard.Add(ref bbAttack[Constants.pieceKnight, iw], x - 2, y - 1);
					CBitBoard.Add(ref bbAttack[Constants.pieceKnight, iw], x - 2, y + 1);
					CBitBoard.Add(ref bbAttack[Constants.pieceKnight, iw], x + 2, y - 1);
					CBitBoard.Add(ref bbAttack[Constants.pieceKnight, iw], x + 2, y + 1);

					CBitBoard.Add(ref bbAttack[Constants.piecePawn | Constants.colWhite, iw], x - 1, y - 1);
					CBitBoard.Add(ref bbAttack[Constants.piecePawn | Constants.colWhite, iw], x + 1, y - 1);
					CBitBoard.Add(ref bbAttack[Constants.piecePawn, iw], x - 1, y + 1);
					CBitBoard.Add(ref bbAttack[Constants.piecePawn, iw], x + 1, y + 1);

					CBitBoard.Add(ref bbAttack[Constants.pieceKing, iw], x - 1, y - 1);
					CBitBoard.Add(ref bbAttack[Constants.pieceKing, iw], x - 1, y + 1);
					CBitBoard.Add(ref bbAttack[Constants.pieceKing, iw], x - 1, y);
					CBitBoard.Add(ref bbAttack[Constants.pieceKing, iw], x + 1, y - 1);
					CBitBoard.Add(ref bbAttack[Constants.pieceKing, iw], x + 1, y + 1);
					CBitBoard.Add(ref bbAttack[Constants.pieceKing, iw], x + 1, y);
					CBitBoard.Add(ref bbAttack[Constants.pieceKing, iw], x, y - 1);
					CBitBoard.Add(ref bbAttack[Constants.pieceKing, iw], x, y + 1);
					for (int n = 1; n < 8; n++)
					{
						CBitBoard.Add(ref bbAttack[Constants.pieceRook, iw], x + n, y);
						CBitBoard.Add(ref bbAttack[Constants.pieceRook, iw], x - n, y);
						CBitBoard.Add(ref bbAttack[Constants.pieceRook, iw], x, y + n);
						CBitBoard.Add(ref bbAttack[Constants.pieceRook, iw], x, y - n);
						CBitBoard.Add(ref bbAttack[Constants.pieceBishop, iw], x + n, y + n);
						CBitBoard.Add(ref bbAttack[Constants.pieceBishop, iw], x + n, y - n);
						CBitBoard.Add(ref bbAttack[Constants.pieceBishop, iw], x - n, y + n);
						CBitBoard.Add(ref bbAttack[Constants.pieceBishop, iw], x - n, y - n);
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

		public static bool IsHashMoveValid(int move)
		{
			int fr = move & Constants.maskMove;
			int to = (move >> 6) & Constants.maskMove;
			int piece = Position.board[fr];
			int rank = piece & Constants.maskRank;
			if (!Position.IsSquareOur(fr) || Position.IsSquareOur(to))
				return false;
			if (rank == Constants.piecePawn)
			{
				int del = Position.usCol == Constants.colWhite ? -8 : 8;
				if ((to == fr + del) && Position.IsSquareEmpty(to))
					return true;
				if ((to == fr + del + 1) && Position.IsSquareEnemy(to))
					return true;
				if ((to == fr + del - 1) && Position.IsSquareEnemy(to))
					return true;
				if ((to == fr + del +del) && Position.IsSquareEmpty(to) && Position.IsSquareDouble(fr))
					return true;
			}
			else
				return IsSquareAttackedFromTo(fr, to);
			return false;
		}
	}
}
