using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Color = System.Int32;
using Move = System.Int32;
using Square = System.Int32;
using Bitboard = System.UInt64;
using Hash = System.UInt64;

namespace NSRapchess
//move 
//6 bits from
//6 bits to
//4 bits promotion
//4 bits move flag
{
	public partial class CEngine
	{
		int curPv = 1;
		int inTime = 0;
		int inDepth = 0;
		ulong inNodes = 0;
		MList inMoves = null;
		public static int castleRights = 0xf;
		ulong hash = 0;
		ulong nodeCur = 0;
		int timeOut = 0;
		int depthOut = 0;
		ulong nodeOut = 0;
		int depthCur = 1;
		bool engineStop = false;
		public int undoIndex = 0;
		readonly ulong hashColor;
		readonly ulong[,] arrHashBoard = new ulong[64, 16];
		readonly int[] boardCastle = new int[64];
		readonly CUndo[] undoStack = new CUndo[0xfff];
		public static Move[,] killers = new Move[Constants.MAX_PLY, 2];
		Thread startThread;
		public static MList moveList = new MList();
		public static List<int> bstMoves = new List<int>();
		public Stopwatch stopwatch = Stopwatch.StartNew();
		private static readonly Random rnd = new Random(1);
		public CSynStop synStop = new CSynStop();
		public static bool optMatePruning = true;
		public static int optMultiPv = 1;

		public ushort MoveNumber
		{
			get
			{
				return (ushort)((CPosition.halfMove >> 1) + 1);
			}
			set
			{
				CPosition.halfMove = value;
				if (CPosition.halfMove > 0)
					CPosition.halfMove--;
				CPosition.halfMove <<= 1;
				if (!CPosition.IsWhiteTurn())
					CPosition.halfMove++;
			}
		}

		public CEngine()
		{
			hash = RandomUInt64();
			for (int n = 0; n < 64; n++)
			{
				CPosition.board[n] = 0;
				boardCastle[n] = 15;
				for (int p = 0; p < 16; p++)
					arrHashBoard[n, p] = RandomUInt64();
			}
			hashColor = RandomUInt64();
			boardCastle[0] = 7;
			boardCastle[4] = 3;
			boardCastle[7] = 11;
			boardCastle[56] = 13;
			boardCastle[60] = 12;
			boardCastle[63] = 14;
			CMovesGenerator.Init();
			CEvaluate.Init();
			SetFen();
		}
		bool GetStop()
		{
			if (engineStop || (depthCur < 2))
				return false;
			if (((timeOut > 0) && (stopwatch.Elapsed.TotalMilliseconds > timeOut)) || ((depthOut > 0) && (depthCur > depthOut)) || ((nodeOut > 0) && (nodeCur > nodeOut)))
				return true;
			return synStop.GetStop();
		}

		public void MakeMoves(string moves)
		{
			string[] am = moves.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string m in am)
			{
				int emo = UmoToEmo(m);
				if (emo > 0)
					MakeMove(emo);
			}
		}

		public void SetFen(string fen = "")
		{
			if (String.IsNullOrEmpty(fen))
				fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
			synStop.SetStop(false);
			CPosition.phase = 0;
			for (int n = 0; n < 64; n++)
				CPosition.board[n] = Constants.colEmpty;
			for (int n = 0; n < 16; n++)
				CPosition.bitBoard[n] = 0;
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
					CPosition.phase++;
					bool isWhite = Char.IsUpper(c);
					int piece = isWhite ? Constants.colWhite : Constants.colBlack;
					int index = (y << 3) | x;
					switch (Char.ToLower(c))
					{
						case 'p':
							piece |= Constants.piecePawn;
							break;
						case 'b':
							piece |= Constants.pieceBishop;
							break;
						case 'n':
							piece |= Constants.pieceKnight;
							break;
						case 'r':
							piece |= Constants.pieceRook;
							break;
						case 'q':
							piece |= Constants.pieceQueen;
							break;
						case 'k':
							piece |= Constants.pieceKing;
							break;
					}
					CPosition.board[index] = piece;
					CBitboard.Add(ref CPosition.bitBoard[piece & 0xf], index);
					x++;
				}
			}
			bool whiteTurn = chunks[1] == "w";
			CPosition.SetColor(whiteTurn ? Constants.colWhite : Constants.colBlack);
			castleRights = 0;
			if (chunks[2].IndexOf('K') != -1)
				castleRights |= 1;
			if (chunks[2].IndexOf('Q') != -1)
				castleRights |= 2;
			if (chunks[2].IndexOf('k') != -1)
				castleRights |= 4;
			if (chunks[2].IndexOf('q') != -1)
				castleRights |= 8;
			CPosition.Passant = chunks.Length < 4 ? "-" : chunks[3];
			CPosition.move50 = chunks.Length < 5 ? (byte)0 : byte.Parse(chunks[4]);
			MoveNumber = chunks.Length < 6 ? (ushort)1 : ushort.Parse(chunks[5]);
			undoIndex = 0;
		}

		public string GetFenBase()
		{
			string result = "";
			string[] arr = { " ", "p", "n", "b", "r", "q", "k", " " };
			for (int y = 0; y < 8; y++)
			{
				if (y != 0)
					result += '/';
				int empty = 0;
				for (int x = 0; x < 8; x++)
				{
					int piece = CPosition.board[(y << 3) + x];
					if (piece == Constants.colEmpty)
						empty++;
					else
					{
						if (empty != 0)
							result += empty;
						empty = 0;
						string pieceChar = arr[(piece & 0x7)];
						result += ((piece & Constants.colWhite) != 0) ? pieceChar.ToUpper() : pieceChar;
					}
				}
				if (empty != 0)
				{
					result += empty;
				}
			}
			return result;
		}

		public string GetEpd()
		{
			string result = GetFenBase();
			result += CPosition.IsWhiteTurn() ? " w " : " b ";
			if (castleRights == 0)
				result += "-";
			else
			{
				if ((castleRights & 1) != 0)
					result += 'K';
				if ((castleRights & 2) != 0)
					result += 'Q';
				if ((castleRights & 4) != 0)
					result += 'k';
				if ((castleRights & 8) != 0)
					result += 'q';
			}
			return $"{result} {CPosition.Passant}";
		}

		public string GetFen()
		{
			return $"{GetEpd()} {CPosition.move50} {MoveNumber}";
		}

		private static ulong RandomUInt64()
		{
			byte[] bytes = new byte[8];
			rnd.NextBytes(bytes);
			return BitConverter.ToUInt64(bytes, 0);
		}

		string EmoToUmo(int emo)
		{
			string result = SquareToStr(emo & Constants.maskMove) + SquareToStr((emo >> 6) & Constants.maskMove);
			int promotion = emo & Constants.maskPromotion;
			if (promotion > 0)
			{
				if (promotion == Constants.moveflagPromoteQueen) result += 'q';
				else if (promotion == Constants.moveflagPromoteRook) result += 'r';
				else if (promotion == Constants.moveflagPromoteBishop) result += 'b';
				else result += 'n';
			}
			return result;
		}

		public int UmoToEmo(string umo)
		{
			MList moves = CMovesGenerator.GenerateMovesAll();
			for (int i = 0; i < moves.count; i++)
				if (EmoToUmo(moves.table[i].move) == umo)
					return moves.table[i].move;
			return 0;
		}

		public static string SquareToStr(int square)
		{
			if (square < 0)
				return "-";
			int x = square & 0x7;
			int y = square >> 3;
			string xs = "abcdefgh";
			string ys = "87654321";
			return $"{xs[x]}{ys[y]}";
		}

		public static int StrToSquare(string s)
		{
			if (s == "-")
				return -1;
			string xs = "abcdefgh";
			string ys = "87654321";
			int x = xs.IndexOf(s[0]);
			int y = ys.IndexOf(s[1]);
			return (y << 3) | x;
		}

		bool IsRepetition()
		{
			int pos = undoIndex - 2;
			while (pos >= 0)
			{
				if (undoStack[pos].hash == hash)
					return true;
				pos -= 2;
				if (pos < undoIndex - CPosition.move50)
					return false;
			}
			return false;
		}

		MList GenerateMovesLegal()
		{
			MList moves = CMovesGenerator.GenerateMovesAll();
			for (int n = moves.count - 1; n >= 0; n--)
			{
				int emo = moves.table[n].move;
				MakeMove(emo);
				if (CMovesGenerator.IsSquareAttacked(CPosition.enRS.kingPosition, CPosition.usCol))
					moves.RemoveAt(n);
				UnmakeMove(emo);
			}
			return moves;
		}

		public void MakeMove(int move)
		{
			int fr = move & Constants.maskMove;
			int to = (move >> 6) & Constants.maskMove;
			int piece = CPosition.board[fr];
			int rank = piece & 7;
			int capi = to;
			int captured = CPosition.board[to];
			if ((move & Constants.moveflagCastleKing) > 0)
			{
				CPosition.board[to - 1] = CPosition.board[to + 1];
				CPosition.board[to + 1] = Constants.colEmpty;
				int secPiece = (piece & Constants.colWhite) | Constants.pieceRook;
				CBitboard.Del(ref CPosition.bitBoard[secPiece], to + 1);
				CBitboard.Add(ref CPosition.bitBoard[secPiece], to - 1);
			}
			else if ((move & Constants.moveflagCastleQueen) > 0)
			{
				CPosition.board[to + 1] = CPosition.board[to - 2];
				CPosition.board[to - 2] = Constants.colEmpty;
				int secPiece = (piece & Constants.colWhite) | Constants.pieceRook;
				CBitboard.Del(ref CPosition.bitBoard[secPiece], to - 2);
				CBitboard.Add(ref CPosition.bitBoard[secPiece], to + 1);
			}
			else if ((move & Constants.moveflagPassing) > 0)
			{
				capi = CPosition.IsWhiteTurn() ? to + 8 : to - 8;
				captured = CPosition.board[capi];
				CPosition.board[capi] = Constants.colEmpty;
			}
			ref CUndo undo = ref undoStack[undoIndex++];
			undo.captured = captured;
			undo.hash = hash;
			undo.passing = CPosition.passant;
			undo.castle = castleRights;
			undo.move50 = CPosition.move50;
			hash ^= arrHashBoard[fr, piece];
			CPosition.passant = -1;
			if (captured != Constants.colEmpty)
			{
				CPosition.move50 = 0;
				CPosition.phase--;
				CBitboard.Del(ref CPosition.bitBoard[captured], capi);
			}
			else if (rank == Constants.piecePawn)
			{
				if (to == (fr + 16))
					CPosition.passant = fr + 8;
				if (to == (fr - 16))
					CPosition.passant = fr - 8;
				CPosition.move50 = 0;
			}
			else
				CPosition.move50++;
			int newPiece = ((move & Constants.maskPromotion) > 0) ? (piece & Constants.maskColor) | ((move >> 12) & Constants.maskRank) : piece;
			CPosition.board[to] = newPiece;
			CBitboard.Add(ref CPosition.bitBoard[newPiece], to);
			hash ^= arrHashBoard[to, newPiece];
			CPosition.board[fr] = Constants.colEmpty;
			CBitboard.Del(ref CPosition.bitBoard[piece], fr);
			castleRights &= boardCastle[fr] & boardCastle[to];
			CPosition.halfMove++;
			hash ^= hashColor;
			CPosition.ChangeColor();
		}

		void UnmakeMove(int move)
		{
			int fr = move & Constants.maskMove;
			int to = (move >> 6) & Constants.maskMove;
			int piece = CPosition.board[to];
			int capI = to;
			CUndo undo = undoStack[--undoIndex];
			CPosition.passant = undo.passing;
			castleRights = undo.castle;
			CPosition.move50 = undo.move50;
			hash = undo.hash;
			int capP = undo.captured;
			CBitboard.Del(ref CPosition.bitBoard[piece], to);
			if ((move & Constants.moveflagCastleKing) > 0)
			{
				CPosition.board[to + 1] = CPosition.board[to - 1];
				CPosition.board[to - 1] = Constants.colEmpty;
				int secPiece = (piece & Constants.colWhite) | Constants.pieceRook;
				CBitboard.Del(ref CPosition.bitBoard[secPiece], to - 1);
				CBitboard.Add(ref CPosition.bitBoard[secPiece], to + 1);
			}
			else if ((move & Constants.moveflagCastleQueen) > 0)
			{
				CPosition.board[to - 2] = CPosition.board[to + 1];
				CPosition.board[to + 1] = Constants.colEmpty;
				int secPiece = (piece & Constants.colWhite) | Constants.pieceRook;
				CBitboard.Del(ref CPosition.bitBoard[secPiece], to + 1);
				CBitboard.Add(ref CPosition.bitBoard[secPiece], to - 2);
			}
			if ((move & Constants.maskPromotion) > 0)
			{
				int secPiece = (piece & Constants.maskColor) | Constants.piecePawn;
				CPosition.board[fr] = secPiece;
				CBitboard.Add(ref CPosition.bitBoard[secPiece], fr);
			}
			else
			{
				CPosition.board[fr] = CPosition.board[to];
				CBitboard.Add(ref CPosition.bitBoard[piece], fr);
			}
			if ((move & Constants.moveflagPassing) > 0)
			{
				capI = CPosition.IsWhiteTurn() ? to - 8 : to + 8;
				CPosition.board[to] = Constants.colEmpty;
				CBitboard.Del(ref CPosition.bitBoard[piece], to);
			}
			CPosition.board[capI] = capP;
			if (capP != Constants.colEmpty)
			{
				CPosition.phase++;
				CBitboard.Add(ref CPosition.bitBoard[capP], capI);
			}
			CPosition.halfMove--;
			CPosition.ChangeColor();
		}

		bool CanReturnTT(CRec rec, int beta)
		{
			if ((rec.type == RecType.beta) && (rec.value >= beta)) return true;
			if ((rec.type == RecType.alpha) && (rec.value < beta)) return true;
			return false;
		}

		int Quiesce(NodeType nt, int ply, int alpha, int beta)
		{
			if ((++nodeCur & 0xffff) == 0)
				engineStop = GetStop();
			if (engineStop)
				return -Constants.CHECKMATE_INFINITY;
			if (nt != NodeType.Pv)
			{
				CTranspositionTable.GetRec(hash, out CRec rec);
				if (CanReturnTT(rec, beta))
					return rec.value;
			}
			int score = CEvaluate.Score();
			if (score >= beta)
				return beta;
			if (score > alpha)
				alpha = score;
			int oldAlpha = alpha;
			int bestMove = 0;
			int bestValue = alpha;
			int legalMoves = 0;
			CMovePicker movePicker = new CMovePicker();
			while (movePicker.NextMove(out Move move))
			{
				MakeMove(move);
				if (CPosition.IsLegal())
				{
					score = -Quiesce(nt, ply + 1, -beta, -alpha);
					legalMoves++;
				}else
					score = -Constants.CHECKMATE_INFINITY;
				UnmakeMove(move);
				if (score > bestValue)
				{
					bestValue = score;
					bestMove = move;
				}
				if (score >= beta)
					break;
				if (score > alpha)
					alpha = score;
			}
			if (!engineStop && (legalMoves > 0))
			{
				RecType rt = bestValue <= oldAlpha ? RecType.alpha : bestValue >= beta ? RecType.beta : RecType.exact;
				CTranspositionTable.SetRec(hash, 0, bestMove, bestValue, rt);
			}
			return bestValue;
		}

		int Search(NodeType nt, int ply, int depth, int alpha, int beta)
		{
			int oldAlpha = alpha;
			int bestMove = 0;
			int bestValue = alpha;
			bool pvNode = nt != NodeType.NonPv;
			bool usChecked = CPosition.IsCheck();
			if ((depth <= 0) && !usChecked)
				return Quiesce(nt, ply, alpha, beta);
			if ((++nodeCur & 0xffff) == 0)
				engineStop = GetStop();
			if (engineStop)
				return -Constants.CHECKMATE_INFINITY;

			if ((ply > 0) && CPosition.usRS.insufficient && CPosition.enRS.insufficient)
				return 0;

			// Mate pruning
			if (optMatePruning)
			{
				int ma = -Constants.CHECKMATE_MAX + ply;
				int mb = Constants.CHECKMATE_MAX - ply - 1;
				if (ma < alpha)
					ma = alpha;
				if (mb > beta)
					mb = beta;
				if (ma >= mb)
					return ma;
			}

			// Null pruning
			if ((ply > 1) && !usChecked &&  (depth > 2) && CPosition.NotOnlyPawns())
			{
				hash ^= hashColor;
				CPosition.ChangeColor();
				int score = -Search(NodeType.NonPv, ply + 1, 1, -beta, -beta + 1);
				hash ^= hashColor;
				CPosition.ChangeColor();
				if (score >= beta)
					return beta;
			}

			if (ply > 0)
			{
				CTranspositionTable.GetRec(hash, depth, out CRec rec);
				if (pvNode ? rec.type == RecType.exact : CanReturnTT(rec, beta))
				{
					if (rec.value >= beta
						&& (rec.move != 0)
						&& !CPosition.IsCaptureOrPromotion(rec.move)
						&& (killers[ply, 0]) != rec.move)
					{
						killers[ply, 1] = killers[ply, 0];
						killers[ply, 0] = rec.move;
					}
					return rec.value;
				}
			}

			int legalMoves = 0;
			CMovePicker movePicker = ply == 0 ? new CMovePicker(hash, moveList, killers[ply, 0], killers[ply, 1]) : new CMovePicker(hash, killers[ply, 0], killers[ply, 1]);
			while (movePicker.NextMove(out Move move))
			{
				int score = -Constants.CHECKMATE_INFINITY;
				bool reducible = (ply > 0) && (legalMoves>1) && !usChecked && (alpha > -Constants.CHECKMATE_NEAR) && (!CEvaluate.IsPassed(CPosition.usCol, (move >> 6) & Constants.maskMove));
				MakeMove(move);
				if (CPosition.IsLegal())
				{
					if ((CPosition.move50 >= 100) || (IsRepetition()))
						score = 0;
					else
					{
						score = alpha + 1;
						if (legalMoves > 0)
						{
							if (reducible && !CPosition.IsCheck())
								score = -Search(NodeType.NonPv, ply + 1, depth - 2, -alpha - 1, -alpha);
							if (score>alpha)
								score = -Search(NodeType.NonPv, ply + 1, depth - 1, -alpha - 1, -alpha);
						}
						if ((legalMoves==0)||((score > alpha)&&(score<beta)))
							score = -Search(NodeType.Pv, ply + 1, depth - 1, -beta, -alpha);
					}
					legalMoves++;
				}
				UnmakeMove(move);
				if (score > bestValue)
				{
					bestValue = score;
					bestMove = move;
				}
				if (score >= beta)
				{
					if (killers[ply, 0] != move)
					{
						killers[ply, 1] = killers[ply, 0];
						killers[ply, 0] = move;
					}
					break;
				}
				if (score > alpha)
				{
					alpha = score;
					if (ply == 0)
					{
						string bsFm = EmoToUmo(move);
						string pv = GetBstMoves(move);
						string scFm = score > Constants.CHECKMATE_NEAR ? $"mate {(Constants.CHECKMATE_MAX - score) >> 1}" : ((score < -Constants.CHECKMATE_NEAR) ? $"mate {(-Constants.CHECKMATE_MAX - score + 2) >> 1}" : $"cp {score}");
						double t = stopwatch.Elapsed.TotalMilliseconds;
						double nps = t > 0 ? nodeCur / t * 1000 : 0;
						string mpv = optMultiPv > 1 ? $" multipv {curPv}" : String.Empty;
						Console.WriteLine($"info time {Convert.ToInt64(t)} nodes {nodeCur} nps {Convert.ToInt64(nps)} depth {depthCur} seldepth {bstMoves.Count} currmove {bsFm} currmovenumber {legalMoves} hashfull {CTranspositionTable.Permill()} score {scFm}{mpv} pv {pv}");
					}
				}
			}
			if (!engineStop)
			{
				if (legalMoves == 0)
					if (usChecked)
						bestValue = -Constants.CHECKMATE_MAX + ply + 1;
					else
						bestValue = 0;
				if (legalMoves > 0)
				{
					RecType rt = bestValue <= oldAlpha ? RecType.alpha : bestValue >= beta ? RecType.beta : RecType.exact;
					CTranspositionTable.SetRec(hash, depth, bestMove, bestValue, rt);
				}
			}
			return bestValue;
		}

		public string GetBstMoves(Move move)
		{
			string result = String.Empty;
			bstMoves.Clear();
			int count = 1;
			bstMoves.Add(move);
			MakeMove(move);
			do
			{
				CTranspositionTable.GetRec(hash, out CRec rec);
				move = rec.move;
				if (!CMovesGenerator.IsHashMoveValid(move))
					break;
				bstMoves.Add(move);
				MakeMove(move);
				if (!CPosition.IsLegal())
					break;
				count++;
			} while ((CPosition.move50 < 100) && !IsRepetition());
			for (int n = bstMoves.Count - 1; n >= 0; n--)
				UnmakeMove(bstMoves[n]);
			for (int n = 0; n < count; n++)
				result = $"{result} {EmoToUmo(bstMoves[n])}";
			return result.Trim();
		}

		public void Start(MList mo, int depth, int time, ulong nodes)
		{
			engineStop = false;
			timeOut = time;
			depthOut = depth;
			nodeOut = nodes;
			depthCur = 1;
			nodeCur = 0;
			curPv = 1;
			CTranspositionTable.NextGeneration();
			mo.CopyTo(moveList);
			string bsFm = String.Empty;
			string bsPm = String.Empty;
			bstMoves.Clear();
			do
			{
				Console.WriteLine($"info depth {depthCur}");
				int score = Search(NodeType.Root, 0, depthCur, -Constants.CHECKMATE_MAX, Constants.CHECKMATE_MAX);
				double ms = stopwatch.Elapsed.TotalMilliseconds;
				double nps = ms > 0 ? (nodeCur * 1000.0) / ms : 0;
				Console.WriteLine($"info time {Convert.ToInt64(ms)} nodes {nodeCur} nps {Convert.ToInt64(nps)}");
				if (curPv == 1)
				{
					if (bstMoves.Count > 0)
						bsFm = EmoToUmo(bstMoves[0]);
					if (bstMoves.Count > 1)
						bsPm = $" ponder {EmoToUmo(bstMoves[1])}";
				}
				if ((++curPv > optMultiPv) || (moveList.count < 2))
				{
					curPv = 1;
					mo.CopyTo(moveList);
				}
				else if (bstMoves.Count > 0)
					moveList.Remove(bstMoves[0]);
				if ((++depthCur > depthOut) && (depthOut > 0))
					break;
				if ((score > Constants.CHECKMATE_NEAR) || (score < -Constants.CHECKMATE_NEAR))
					break;
			} while (!engineStop);
			Console.WriteLine($"bestmove {bsFm}{bsPm}");
			if (Program.bench.start)
			{
				Program.bench.nodes += nodeCur;
				Program.bench.Next();
			}
		}

		public void UciGo()
		{
			synStop.SetStop(false);
			stopwatch.Restart();
			MList mo = GenerateMovesLegal();
			if (mo.count == 0)
			{
				Console.WriteLine($"info string no moves");
				return;
			}
			if (mo.count == 1)
			{
				Console.WriteLine($"bestmove {EmoToUmo(mo.table[0].move)}");
				return;
			}
			int time = Program.uci.GetInt("movetime");
			int depth = Program.uci.GetInt("depth");
			ulong node = (ulong)Program.uci.GetInt("nodes");
			int infinite = Program.uci.GetIndex("infinite");
			if ((time == 0) && (depth == 0) && (node == 0) && (infinite == 0))
			{
				double ct = CPosition.IsWhiteTurn() ? Program.uci.GetInt("wtime") : Program.uci.GetInt("btime");
				double inc = CPosition.IsWhiteTurn() ? Program.uci.GetInt("winc") : Program.uci.GetInt("binc");
				double mg = Program.uci.GetInt("movestogo", 32);
				double mod = mo.count / mg;
				if (mod > 1)
					mod = 1;
				time = Convert.ToInt32(((ct - 1000.0) * mod + inc * mg * mod) / mg);
				if (time < 1)
					time = 1;
			}
			StartThread(mo, depth, time, node);
		}

		public void Thread()
		{
			Start(inMoves, inDepth, inTime, inNodes);
		}

		public void StartThread(MList moves, int depth, int time, ulong nodes)
		{
			inMoves = moves;
			inDepth = depth;
			inTime = time;
			inNodes = nodes;
			startThread = new Thread(Thread);
			startThread.Start();
		}

	}
}