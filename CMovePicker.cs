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
		int index =0;
		readonly MList moves = new MList();

		public CMovePicker()
		{
			moves = CMovesGenerator.GenerateMovesAttack();
		}

		public CMovePicker(MList ml,Hash hash,  Move killer1, Move killer2)
		{
			CTranspositionTable.GetRec(hash, out CRec rec);
			ml.Best(rec.move);
			ml.Best(killer1);
			ml.Best(killer2);
			ml.CopyTo(moves);
		}

		public bool NextMove(out Move move)
		{
			move = 0;
			if (index >= moves.count)
				return false;
			if(index<moves.bst)
			{
				move = moves.table[index++].move;
				return true;
			}
			if(index==moves.bst)
                for (int n = index; n < moves.count; n++)
                    moves.table[n].score = GetScore(moves.table[n].move);
			int bstI = index;
			int bstS = int.MinValue;
			for (int n = index; n < moves.count; n++)
				if (moves.table[n].score > bstS)
				{
					bstI = n;
					bstS = moves.table[n].score;
				}
			move = moves.table[bstI].move;
			(moves.table[bstI], moves.table[index]) = (moves.table[index],moves.table[bstI]);
			index++;
			return true;
		}

		int GetScore(int move)
		{
			int fr = move & 0x3f;
			int to = (move >> 6) & 0x3f;
			int pr = (move >> 12) & 7;
			int phase = CPosition.phase;
			int pieceFr = CPosition.board[fr];
			int pieceTo = CPosition.board[to];
			int score = -CEvaluate.bonPositionPhase[pieceFr, fr, phase];
			if (pr == 0)
				score += CEvaluate.bonPositionPhase[pieceFr, to, phase];
			else
				score += CEvaluate.bonPositionPhase[CPosition.usCol | pr, to, phase];
			if (pieceTo > 0)
				score += CEvaluate.bonPositionPhase[pieceTo, to, phase];
			return score;
		}


	}
}
