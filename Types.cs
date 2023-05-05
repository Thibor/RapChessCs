using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Move = System.Int32;
using Score = System.Int32;

namespace NSRapchess
{
	public class CDataOut
	{
		public string moveBst = String.Empty;
		public string movePonder = String.Empty;

		public void Restart()
		{
			moveBst = String.Empty;
			movePonder = String.Empty;
		}

	}

	public class COptions
	{
		public bool ponder = true;

		public COptions()
		{
			ponder = true;
		}

	}

	public class CDataIn
	{
		public bool infinite = false;
		public bool ponder = false;
		public bool post = true;
		public int time = 0;
		public int depth = 0;
		public ulong nodes = 0;

		public CDataIn()
		{
			Reset();
		}

		public void Reset()
		{
			infinite = false;
			ponder = false;
			post = true;
		}

	}

	public enum NodeType : byte
	{
		Root,
		Pv,
		NonPv
	}

	public struct MoveStack
	{
		internal Move move;
		internal int score;

		public MoveStack(Move move, Score score)
		{
			this.move = move;
			this.score = score;
		}

	};

	public class MList
	{
		internal readonly MoveStack[] table = new MoveStack[Constants.MAX_MOVES];
		internal int count = 0;

		public void Add(int move)
		{
			table[count++].move = move;
		}

		public void Add(MoveStack ms)
		{
			table[count++] = ms;
		}

		public int Find(Move m)
		{
			for (int n = 0; n < count; n++)
				if (table[n].move == m)
					return n;
			return -1;
		}

		public void Remove(Move m)
		{
			for (int n = 0; n < count; n++)
				if (table[n].move == m)
				{
					count--;
					(table[n], table[count]) = (table[count], table[n]);
					return;
				}
		}

		public void RemoveAt(int i)
		{
			count--;
			(table[i], table[count]) = (table[count], table[i]);
		}

		public void CopyTo(MList to)
		{
			Array.Copy(this.table, to.table, count);
			to.count = count;
		}

	}

}
