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

	internal static class Constants
	{
		internal const int elo = 2000;
		internal const int shiftDepth = 0;//6
		internal const int piecePawn = 0x01;
		internal const int pieceKnight = 0x02;
		internal const int pieceBishop = 0x03;
		internal const int pieceRook = 0x04;
		internal const int pieceQueen = 0x05;
		internal const int pieceKing = 0x06;
		internal const int colEmpty = 0;
		internal const int colWhite = 8;
		internal const int colBlack = 0;
		internal const int maskMove = 0x3f;
		internal const int maskColor = 8;
		internal const int maskRank = 7;
		internal const int moveflagPassing = 0x02 << 16;
		internal const int moveflagCastleKing = 0x04 << 16;
		internal const int moveflagCastleQueen = 0x08 << 16;
		internal const int maskCastle = moveflagCastleKing | moveflagCastleQueen;
		internal const int moveflagPromoteQueen = pieceQueen << 12;
		internal const int moveflagPromoteRook = pieceRook << 12;
		internal const int moveflagPromoteBishop = pieceBishop << 12;
		internal const int moveflagPromoteKnight = pieceKnight << 12;
		internal const int maskPromotion = moveflagPromoteQueen | moveflagPromoteRook | moveflagPromoteBishop | moveflagPromoteKnight;
		internal const int maskSpecial = maskCastle | maskPromotion;
		internal const Bitboard bbPromotion = 0xff000000000000fful;
		internal const Bitboard bbLight = 0xaa55aa55aa55aa55ul;
		internal const Bitboard bbDark  = 0x55aa55aa55aa55aaul;
		internal const int MAX_MOVES = 200;
		internal const int MAX_PLY = 100;
		internal const int CHECKMATE_MAX = 0x7ff0;
		internal const int CHECKMATE_NEAR = 0x7000;
		internal const int CHECKMATE_INFINITY = 0x7fff;
	}
}
