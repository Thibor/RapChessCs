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
		public static int passant = -1;
		public static int phase = 32;
		public static ushort halfMove = 0;
		public static byte move50 = 0;
		public static Color usCol = Constants.colWhite;
		public static Color enCol = Constants.colBlack;
		public static ulong[] bitBoard = new ulong[16];
		public static int[] board = new int[64];
		public static ulong usBitboard = 0;
		public static ulong enBitboard = 0;
		public static ulong emBitboard = 0;
		public static RScore usRS;
		public static RScore enRS;

		public static string Passant
		{
			get
			{
				return CSearch.SquareToStr(passant);
			}
			set
			{
				passant = CSearch.StrToSquare(value);
			}
		}

		public static void SetColor(Color c)
		{
			usCol = c;
			enCol = c ^ Constants.maskColor;
			bitBoard[usCol] = bitBoard[Constants.piecePawn | usCol] | bitBoard[Constants.pieceKnight | usCol] | bitBoard[Constants.pieceBishop | usCol] | bitBoard[Constants.pieceRook | usCol] | bitBoard[Constants.pieceQueen | usCol] | bitBoard[Constants.pieceKing | usCol];
			bitBoard[enCol] = bitBoard[Constants.piecePawn | enCol] | bitBoard[Constants.pieceKnight | enCol] | bitBoard[Constants.pieceBishop | enCol] | bitBoard[Constants.pieceRook | enCol] | bitBoard[Constants.pieceQueen | enCol] | bitBoard[Constants.pieceKing | enCol];
			usBitboard = bitBoard[usCol];
			enBitboard = bitBoard[enCol];
			emBitboard = ~(usBitboard | enBitboard);
			usRS = new RScore(CPosition.usCol);
			enRS = new RScore(CPosition.enCol);
		}

		public static bool IsCapture(Move move)
		{
			return board[(move >> 6) & 0x3f] != 0;
		}


		public static bool IsCaptureOrPromotion(Move move)
		{
			return ((move & Constants.maskPromotion) > 0) || (board[(move >> 6) & 0x3f] != 0);
		}

		public static bool IsSpecialCapture(Move move)
		{
			return ((move & Constants.maskSpecial) > 0) || (board[(move >> 6) & 0x3f] != 0);
		}

		public static bool IsPassed(Move move)
		{
			int piece = (move >> 12) & 0xf;
			if ((piece & 0x7) != Constants.piecePawn)
				return false;
			int to = (move >> 6) & 0x3f;
			return (CPosition.bitBoard[(piece ^ Constants.maskColor) | Constants.piecePawn] & CEvaluate.bbPassed[to, piece >> 3]) == 0;
		}

		public static bool IsCheck()
		{
			return CMovesGenerator.IsSquareAttacked(usRS.kingPosition, enCol);
		}

		public static bool IsLegal()
		{
			return !CMovesGenerator.IsSquareAttacked(enRS.kingPosition, usCol);
		}

		public static bool IsPawnOnRank7()
		{
			return (bitBoard[usCol | Constants.piecePawn] & CEvaluate.bbRank[6, usCol >> 3]) > 0;
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
