using System;
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
	static class CPosition
	{
		public static Color usCol = Constants.colWhite;
		public static Color enCol = Constants.colBlack;
		public static Square usKing;
		public static Square enKing;
		public static ulong[] bitBoard = new ulong[16];
		public static int[] board = new int[64];
		public static ulong usBitboard = 0;
		public static ulong enBitboard = 0;
		public static ulong emBitboard = 0;

		public static void SetColor(Color c)
		{
			usCol = c;
			enCol = c ^ Constants.maskColor;
			usKing = CBitboard.Read(bitBoard[usCol | Constants.pieceKing]);
			enKing = CBitboard.Read(bitBoard[enCol | Constants.pieceKing]);
			bitBoard[usCol] = bitBoard[Constants.piecePawn | usCol] | bitBoard[Constants.pieceKnight | usCol] | bitBoard[Constants.pieceBishop | usCol] | bitBoard[Constants.pieceRook | usCol] | bitBoard[Constants.pieceQueen | usCol] | bitBoard[Constants.pieceKing | usCol];
			bitBoard[enCol] = bitBoard[Constants.piecePawn | enCol] | bitBoard[Constants.pieceKnight | enCol] | bitBoard[Constants.pieceBishop | enCol] | bitBoard[Constants.pieceRook | enCol] | bitBoard[Constants.pieceQueen | enCol] | bitBoard[Constants.pieceKing | enCol];
			usBitboard = bitBoard[usCol];
			enBitboard = bitBoard[enCol];
			emBitboard = ~(usBitboard | enBitboard);
		}

		public static bool IsWhiteTurn()
		{
			return usCol == Constants.colWhite;
		}

		public static bool IsSquareOur(Square square)
		{
			return (usBitboard & (1ul << square)) > 0;
		}

		public static bool IsSquareEnemy(Square square)
		{
			return (enBitboard & (1ul << square)) > 0;
		}

		public static bool IsSquareEmpty(Square square)
		{
			return board[square] == 0;
		}

		public static bool IsRank(Square square, int rank)
		{
			return rank == 1 + (usCol == Constants.colWhite ? 7-(square >> 3) :  (square >> 3));
		}

		public static bool NotOnlyPawns()
		{
			return usBitboard != (bitBoard[usCol | Constants.pieceKing] | bitBoard[usCol | Constants.piecePawn]);
		}

		public static void ChangeColor()
		{
			SetColor(enCol);
		}

	}
}
