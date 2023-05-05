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
			insufficient = CEvaluate.GetInsufficient(usCol);
		}

	}

	internal class CEvaluate
	{
		private static readonly Random rnd = new Random();
		readonly public static Bitboard[,] bbRank = new ulong[8, 2];
		readonly static Bitboard[,] bbOutpost = new ulong[64, 2];
		readonly static Bitboard[,] bbSupported = new ulong[64, 2];
		readonly static Bitboard[] bbIsolated = new ulong[64];
		readonly static Bitboard[] bbDoubled = new ulong[64];
		readonly static Bitboard[] bbConnected = new ulong[64];
		readonly public static Bitboard[,] bbPassed = new ulong[64, 2];
		readonly static Bitboard[,] bbKingSafety = new ulong[64, 2];
		readonly static Bitboard[] bbAttackBishop = new ulong[64];
		readonly static Bitboard[] bbAttackRook = new ulong[64];
		readonly static Bitboard[] bbAttackKing = new ulong[64];
		public static double[] arrCenter = new double[64];
		public static int[,] arrDistance = new int[64, 64];

		static readonly short[] mgTable = new short[6 * 64]
{ //PAWN MG
          100,  100,  100,  100,  100,  100,  100,  100,
		  176,  214,  147,  194,  189,  214,  132,   77,
		   82,   88,  106,  113,  150,  146,  110,   73,
		   67,   93,   83,   95,   97,   92,   99,   63,
		   55,   74,   80,   89,   94,   86,   90,   55,
		   55,   70,   68,   69,   76,   81,  101,   66,
		   52,   84,   66,   60,   69,   99,  117,   60,
		  100,  100,  100,  100,  100,  100,  100,  100,
          //KNIGHT MG
          116,  228,  271,  270,  338,  213,  278,  191,
		  225,  247,  353,  331,  321,  360,  300,  281,
		  258,  354,  343,  362,  389,  428,  375,  347,
		  300,  332,  325,  360,  349,  379,  339,  333,
		  298,  322,  325,  321,  337,  332,  332,  303,
		  287,  297,  316,  319,  327,  320,  327,  294,
		  276,  259,  300,  304,  308,  322,  296,  292,
		  208,  290,  257,  274,  296,  284,  293,  284,
          //BISHOP MG
          292,  338,  254,  283,  299,  294,  337,  323,
		  316,  342,  319,  319,  360,  385,  343,  295,
		  342,  377,  373,  374,  368,  392,  385,  363,
		  332,  338,  356,  384,  370,  380,  337,  341,
		  327,  354,  353,  366,  373,  346,  345,  341,
		  335,  350,  351,  347,  352,  361,  350,  344,
		  333,  354,  354,  339,  344,  353,  367,  333,
		  309,  341,  342,  325,  334,  332,  302,  313,
          //ROOK MG
          493,  511,  487,  515,  514,  483,  485,  495,
		  493,  498,  529,  534,  546,  544,  483,  508,
		  465,  490,  499,  497,  483,  519,  531,  480,
		  448,  464,  476,  495,  484,  506,  467,  455,
		  442,  451,  468,  470,  476,  472,  498,  454,
		  441,  461,  468,  465,  478,  481,  478,  452,
		  443,  472,  467,  476,  483,  500,  487,  423,
		  459,  463,  470,  479,  480,  480,  446,  458,
          //QUEEN MG
          865,  902,  922,  911,  964,  948,  933,  928,
		  886,  865,  903,  921,  888,  951,  923,  940,
		  902,  901,  907,  919,  936,  978,  965,  966,
		  881,  885,  897,  894,  898,  929,  906,  915,
		  907,  884,  899,  896,  904,  906,  912,  911,
		  895,  916,  900,  902,  904,  912,  924,  917,
		  874,  899,  918,  908,  915,  924,  911,  906,
		  906,  899,  906,  918,  898,  890,  878,  858,
          //KING MG
          -11,   70,   55,   31,  -37,  -16,   22,   22,
		   37,   24,   25,   36,   16,    8,  -12,  -31,
		   33,   26,   42,   11,   11,   40,   35,   -2,
			0,   -9,    1,  -21,  -20,  -22,  -15,  -60,
		  -25,   16,  -27,  -67,  -81,  -58,  -40,  -62,
			7,   -2,  -37,  -77,  -79,  -60,  -23,  -26,
		   12,   15,  -13,  -72,  -56,  -28,   15,   17,
		   -6,   44,   29,  -58,    8,  -25,   34,   28,
};

		static readonly short[] egTable = new short[6 * 64]
		{ //PAWN EG
          100,  100,  100,  100,  100,  100,  100,  100,
		  277,  270,  252,  229,  240,  233,  264,  285,
		  190,  197,  182,  168,  155,  150,  180,  181,
		  128,  117,  108,  102,   93,  100,  110,  110,
		  107,  101,   89,   85,   86,   83,   92,   91,
		   96,   96,   85,   92,   88,   83,   85,   82,
		  107,   99,   97,   97,  100,   89,   89,   84,
		  100,  100,  100,  100,  100,  100,  100,  100,
          //KNIGHT EG
          229,  236,  269,  250,  257,  249,  219,  188,
		  252,  274,  263,  281,  273,  258,  260,  229,
		  253,  264,  290,  289,  278,  275,  263,  243,
		  267,  280,  299,  301,  299,  293,  285,  264,
		  263,  273,  293,  301,  296,  293,  284,  261,
		  258,  276,  278,  290,  287,  274,  260,  255,
		  241,  259,  270,  277,  276,  262,  260,  237,
		  253,  233,  258,  264,  261,  260,  234,  215,
          //BISHOP EG
          288,  278,  287,  292,  293,  290,  287,  277,
		  289,  294,  301,  288,  296,  289,  294,  281,
		  292,  289,  296,  292,  296,  300,  296,  293,
		  293,  302,  305,  305,  306,  302,  296,  297,
		  289,  293,  304,  308,  298,  301,  291,  288,
		  285,  294,  304,  303,  306,  294,  290,  280,
		  285,  284,  291,  299,  300,  290,  284,  271,
		  277,  292,  286,  295,  294,  288,  290,  285,
          //ROOK EG
          506,  500,  508,  502,  504,  507,  505,  503,
		  505,  506,  502,  502,  491,  497,  506,  501,
		  504,  503,  499,  500,  500,  495,  496,  496,
		  503,  502,  510,  500,  502,  504,  500,  505,
		  505,  509,  509,  506,  504,  503,  496,  495,
		  500,  503,  500,  505,  498,  498,  499,  489,
		  496,  495,  502,  505,  498,  498,  491,  499,
		  492,  497,  498,  496,  493,  493,  497,  480,
          //QUEEN EG
          918,  937,  943,  945,  934,  926,  924,  942,
		  907,  945,  946,  951,  982,  933,  928,  912,
		  896,  921,  926,  967,  963,  937,  924,  915,
		  926,  944,  939,  962,  983,  957,  981,  950,
		  893,  949,  942,  970,  952,  956,  953,  936,
		  911,  892,  933,  928,  934,  942,  934,  924,
		  907,  898,  883,  903,  903,  893,  886,  888,
		  886,  887,  890,  872,  916,  890,  906,  879,
          //KING EG
          -74,  -43,  -23,  -25,  -11,   10,    1,  -12,
		  -18,    6,    4,    9,    7,   26,   14,    8,
		   -3,    6,   10,    6,    8,   24,   27,    3,
		  -16,    8,   13,   20,   14,   19,   10,   -3,
		  -25,  -14,   13,   20,   24,   15,    1,  -15,
		  -27,  -10,    9,   20,   23,   14,    2,  -12,
		  -32,  -17,    4,   14,   15,    5,  -10,  -22,
		  -55,  -40,  -23,   -6,  -20,   -8,  -28,  -47,
		};

		//                 opponent                            friendly                  
		//	-     P     N     B     R     Q     K     -     -     P      N     B     R     Q     K   
		public static short[] MobilityValuesPawn = new short[]
		{   0,    0,   28,   36,   17,   31,   89,    0,    0,   10,    9,   11,    3,   -1,   -5 };  //P
		public static short[] MobilityValuesKnight = new short[]
		{   1,   -4,    0,   20,   19,   18,   34,    0,    0,    1,    3,    0,    2,    4,    3 };  // N
		public static short[] MobilityValuesBishop = new short[]
		{    2,   -1,   22,    0,   15,   30,   67,   0,    0,   -8,    3,   54,    4,   -1,   -3 };  // B
		public static short[] MobilityValuesRook = new short[]
		{    2,   -1,    6,   15,    0,   31,   36,   0,    0,  -11,    3,   -2,    4,    2,   -1 };  // R
		public static short[] MobilityValuesQueen = new short[]
		{    3,   -3,   -3,    4,    1,    0,   75,   0,    0,   -3,    5,    6,    1,  -99,   -4 };  // Q
		public static short[] MobilityValuesKing = new short[]
		{   0,   30,    3,   12,    5,  -99,    0,    0,    0,    6,    4,    7,   -8,    6,    0 };  // K

		readonly static int[,] orgPositionMg = new int[16, 64];
		readonly static int[,] orgPositionEg = new int[16, 64];
		readonly static int[,] bonPositionMg = new int[16, 64];
		readonly static int[,] bonPositionEg = new int[16, 64];
		public static int[,,] bonPositionPhase = new int[16, 64, 33];

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
					int ib = ((7 - y) << 3) | x;
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
					orgPositionMg[Constants.piecePawn | 8, iw] = mgTable[iw];
					orgPositionMg[Constants.pieceKnight | 8, iw] = mgTable[iw + 64];
					orgPositionMg[Constants.pieceBishop | 8, iw] = mgTable[iw + 64 * 2];
					orgPositionMg[Constants.pieceRook | 8, iw] = mgTable[iw + 64 * 3];
					orgPositionMg[Constants.pieceQueen | 8, iw] = mgTable[iw + 64 * 4];
					orgPositionMg[Constants.pieceKing | 8, iw] = mgTable[iw + 64 * 5];
					orgPositionEg[Constants.piecePawn | 8, iw] = egTable[iw];
					orgPositionEg[Constants.pieceKnight | 8, iw] = egTable[iw + 64];
					orgPositionEg[Constants.pieceBishop | 8, iw] = egTable[iw + 64 * 2];
					orgPositionEg[Constants.pieceRook | 8, iw] = egTable[iw + 64 * 3];
					orgPositionEg[Constants.pieceQueen | 8, iw] = egTable[iw + 64 * 4];
					orgPositionEg[Constants.pieceKing | 8, iw] = egTable[iw + 64 * 5];
					orgPositionMg[Constants.piecePawn, iw] = mgTable[ib];
					orgPositionMg[Constants.pieceKnight, iw] = mgTable[ib + 64];
					orgPositionMg[Constants.pieceBishop, iw] = mgTable[ib + 64 * 2];
					orgPositionMg[Constants.pieceRook, iw] = mgTable[ib + 64 * 3];
					orgPositionMg[Constants.pieceQueen, iw] = mgTable[ib + 64 * 4];
					orgPositionMg[Constants.pieceKing, iw] = mgTable[ib + 64 * 5];
					orgPositionEg[Constants.piecePawn, iw] = egTable[ib];
					orgPositionEg[Constants.pieceKnight, iw] = egTable[ib + 64];
					orgPositionEg[Constants.pieceBishop, iw] = egTable[ib + 64 * 2];
					orgPositionEg[Constants.pieceRook, iw] = egTable[ib + 64 * 3];
					orgPositionEg[Constants.pieceQueen, iw] = egTable[ib + 64 * 4];
					orgPositionEg[Constants.pieceKing, iw] = egTable[ib + 64 * 5];
				}
			SetLevel(100);
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

		public static int MgEgToScore(int mg, int eg, int phase)
		{
			return (mg * phase + eg * (32 - phase)) / 32;
		}

		static int SliderBishop(Color enCol, int fr, short[] arr)
		{
			int score = 0;
			ulong bbFr = 1ul << fr;
			ulong bb = bbFr;
			int to = fr;
			while ((bb & CMovesGenerator.bbLastSE) == 0)
			{
				bb >>= 9;
				to -= 9;
				score += arr[CPosition.board[to] ^ enCol];
				if ((bb & CPosition.emBitboard) == 0)
					break;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastSW) == 0)
			{
				bb >>= 7;
				to -= 7;
				score += arr[CPosition.board[to] ^ enCol];
				if ((bb & CPosition.emBitboard) == 0)
					break;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastNW) == 0)
			{
				bb <<= 9;
				to += 9;
				score += arr[CPosition.board[to] ^ enCol];
				if ((bb & CPosition.emBitboard) == 0)
					break;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastNE) == 0)
			{
				bb <<= 7;
				to += 7;
				score += arr[CPosition.board[to] ^ enCol];
				if ((bb & CPosition.emBitboard) == 0)
					break;
			}
			return score;
		}

		static int SliderRook(Color enCol, int fr, short[] arr)
		{
			int score = 0;
			ulong bbFr = 1ul << fr;
			ulong bb = bbFr;
			int to = fr;
			while ((bb & CMovesGenerator.bbLastE) == 0)
			{
				bb >>= 1;
				to--;
				score += arr[CPosition.board[to] ^ enCol];
				if ((bb & CPosition.emBitboard) == 0)
					break;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastW) == 0)
			{
				bb <<= 1;
				to++;
				score += arr[CPosition.board[to] ^ enCol];
				if ((bb & CPosition.emBitboard) == 0)
					break;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastS) == 0)
			{
				bb >>= 8;
				to -= 8;
				score += arr[CPosition.board[to] ^ enCol];
				if ((bb & CPosition.emBitboard) == 0)
					break;
			}
			bb = bbFr;
			to = fr;
			while ((bb & CMovesGenerator.bbLastN) == 0)
			{
				bb <<= 8;
				to += 8;
				score += arr[CPosition.board[to] ^ enCol];
				if ((bb & CPosition.emBitboard) == 0)
					break;
			}
			return score;
		}

		public static void ScorePawn(RScore rs, ref int mg, ref int eg)
		{
			int mob = 0;
			int piece = Constants.piecePawn | rs.usCol;
			ulong usbb = CPosition.bitBoard[piece];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				ulong bb = CMovesGenerator.bbAttack[piece, fr];
				while (bb > 0)
				{
					int to = CBitboard.Pop(ref bb);
					mob += MobilityValuesPawn[CPosition.board[to] ^ rs.enCol];
				}
				mg += bonPositionMg[piece, fr];
				eg += bonPositionEg[piece, fr];
			}
			mg += mob;
			eg += mob;
		}

		public static void ScoreKnight(RScore rs, ref int mg, ref int eg)
		{
			int mob = 0;
			int piece = Constants.pieceKnight | rs.usCol;
			ulong usbb = CPosition.bitBoard[piece];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				ulong bb = CMovesGenerator.bbAttack[piece, fr];
				while (bb > 0)
				{
					int to = CBitboard.Pop(ref bb);
					mob += MobilityValuesKnight[CPosition.board[to] ^ rs.enCol];
				}
				mg += bonPositionMg[piece, fr];
				eg += bonPositionEg[piece, fr];
			}
			mg += mob;
			eg += mob;
		}

		public static void ScoreBishop(RScore rs, ref int mg, ref int eg)
		{
			int mob = 0;
			int piece = Constants.pieceBishop | rs.usCol;
			ulong usbb = CPosition.bitBoard[piece];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				mob += SliderBishop(rs.enCol, fr, MobilityValuesBishop);
				mg += bonPositionMg[piece, fr];
				eg += bonPositionEg[piece, fr];
			}
			mg += mob;
			eg += mob;
		}

		public static void ScoreRook(RScore rs, ref int mg, ref int eg)
		{
			int mob = 0;
			int piece = Constants.pieceRook | rs.usCol;
			ulong usbb = CPosition.bitBoard[piece];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				mob += SliderRook(rs.enCol, fr, MobilityValuesRook);
				mg += bonPositionMg[piece, fr];
				eg += bonPositionEg[piece, fr];
			}
			mg += mob;
			eg += mob;
		}

		public static void ScoreQueen(RScore rs, ref int mg, ref int eg)
		{
			int mob = 0;
			int piece = Constants.pieceQueen | rs.usCol;
			ulong usbb = CPosition.bitBoard[piece];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				mob += SliderBishop(rs.enCol, fr, MobilityValuesQueen);
				mob += SliderRook(rs.enCol, fr, MobilityValuesQueen);
				mg += bonPositionMg[piece, fr];
				eg += bonPositionEg[piece, fr];
			}
			mg += mob;
			eg += mob;
		}

		public static void ScoreKing(RScore rs, ref int mg, ref int eg)
		{
			int mob = 0;
			int piece = Constants.pieceKing | rs.usCol;
			ulong usbb = CPosition.bitBoard[piece];
			while (usbb > 0)
			{
				int fr = CBitboard.Pop(ref usbb);
				ulong bb = CMovesGenerator.bbAttack[piece, fr];
				while (bb > 0)
				{
					int to = CBitboard.Pop(ref bb);
					mob += MobilityValuesKing[CPosition.board[to] ^ rs.enCol];
				}
				mg += bonPositionMg[piece, fr];
				eg += bonPositionEg[piece, fr];
			}
			mg += mob;
			eg += mob;
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
			int scoreUs = Score(CPosition.usRS);
			int scoreEn = Score(CPosition.enRS);
			if (CPosition.usRS.insufficient && CPosition.enRS.insufficient)
				return 0;
			return scoreUs - scoreEn;
		}

		public static void SetLevel(int level)
		{
			for (int r = 0; r < 16; r++)
				for (int f = 0; f < 64; f++)
				{
					int valO = (orgPositionMg[r, f]*level)/100;
					int valD = ((rnd.Next(800) - 400)*(100-level))/100;
					bonPositionMg[r, f] = valO + valD;
					bonPositionEg[r, f] = orgPositionEg[r, f];
					for (int p = 0; p < 33; p++)
					{
						int mg = bonPositionMg[r, f];
						int eg = bonPositionEg[r, f];
						bonPositionPhase[r, f, p] = MgEgToScore(mg, eg, p);
					}
				}
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
			Console.WriteLine($"Total ({scoreMg} {scoreEg})");
			Console.WriteLine($"Score ({Score()})");
		}

	}
}
