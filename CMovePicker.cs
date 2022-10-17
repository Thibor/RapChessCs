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
using Hash = System.UInt64;


namespace NSRapchess
{

	internal class CMovePicker
	{
		int index;
		int phase = 0;
		readonly MList pickerList = new MList();
		readonly MList recList = new MList();
		MList startMoves = new MList();
		readonly MList usedMoves = new MList();

		public CMovePicker()
		{
			startMoves = CMovesGenerator.GenerateMovesAttack();
			for (int n = 0; n < startMoves.count; n++)
			{
				int m = startMoves.table[n].move;
				int s = GetScore(m);
				if (s > 0) pickerList.Add(new MoveStack(m, s));
			}
			phase = 2;
		}

		public CMovePicker(Hash hash, MList moves, Move killer1, Move killer2)
		{
			recList = CTranspositionTable.GetMoves(hash);
			recList.Add(killer1);
			recList.Add(killer2);
			startMoves = moves;
		}

		public CMovePicker(Hash hash, Move killer1, Move killer2)
		{
			recList = CTranspositionTable.GetMoves(hash);
			recList.Add(killer1);
			recList.Add(killer2);
		}

		public bool NextMove(out Move move)
		{
			//send rec.move
			if (phase == 0)
			{
				while (index < recList.count)
				{
					move = recList.table[index++].move;
					if ((usedMoves.Find(move) == -1) && CMovesGenerator.IsHashMoveValid(move))
					{
						usedMoves.Add(move);
						return true;
					}
				}
				if (startMoves.count == 0)
				{
					startMoves = CMovesGenerator.GenerateMovesAll();
					Debug.Assert(startMoves.table[0].move > 0);
					if (startMoves.table[0].move < 0)
						startMoves = CMovesGenerator.GenerateMovesAll();
				}
				for (int n = 0; n < usedMoves.count; n++)
					startMoves.Remove(usedMoves.table[n].move);
				index = 0;
				phase = 1;
			}
			//create moves list
			if (phase == 1)
			{
				for (int n = 0; n < startMoves.count; n++)
				{
					int m = startMoves.table[n].move;
					int s = GetScore(m);
					pickerList.Add(new MoveStack(m, s));
				}
				index = 0;
				phase = 2;
			}
			if (index >= pickerList.count)
			{
				move = 0;
				return false;
			}
			int bstI = index;
			int bstS = int.MinValue;
			for (int n = index; n < pickerList.count; n++)
				if (pickerList.table[n].score > bstS)
				{
					bstI = n;
					bstS = pickerList.table[n].score;
				}
			move = pickerList.table[bstI].move;
			(pickerList.table[bstI], pickerList.table[index]) = (pickerList.table[index], pickerList.table[bstI]);
			index++;
			return true;
		}

		int GetScore(int move)
		{
			int fr = move & 0x3f;
			int to = (move >> 6) & 0x3f;
			int pr = (move >> 12) & 7;
			int pieceFr = CPosition.board[fr];
			int pieceTo = CPosition.board[to];
			int scoreMg = -CScore.bonMaterialMg[pieceFr, fr];
			int scoreEg = -CScore.bonMaterialEg[pieceFr, fr];
			if (pr == 0)
			{
				scoreMg += CScore.bonMaterialMg[pieceFr, to];
				scoreEg += CScore.bonMaterialEg[pieceFr, to];
			}
			else
			{
				scoreMg += CScore.bonMaterialMg[CPosition.usCol | pr, to];
				scoreEg += CScore.bonMaterialEg[CPosition.usCol | pr, to];
			}
			if (pieceTo > 0)
			{
				scoreMg += CScore.bonMaterialMg[pieceTo, to];
				scoreEg += CScore.bonMaterialEg[pieceTo, to];
			}
			return CScore.MgEgToScore(scoreMg, scoreEg);
		}

	}
}
