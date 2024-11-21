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
		public int bst = 0;

		public void Add(Move move)
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

		public bool Remove(Move m)
		{
			for (int n = 0; n < count; n++)
				if (table[n].move == m)
				{
					table[n]=table[--count];
					return true;
				}
			return false;
		}

		public void RemoveAt(int i)
		{
			table[i] = table[--count];
		}

		public void CopyTo(MList to)
		{
			Array.Copy(table, to.table, count);
			to.count = count;
			to.bst = bst;
		}

        public MList Copy()
        {
			MList copy = new MList();
            Array.Copy(table, copy.table, count);
            copy.count = count;
			copy.bst = bst;
			return copy;
        }

		public void Best(Move m)
		{
            for (int n = bst; n < count; n++)
                if (table[n].move == m)
                {
					(table[n], table[bst]) = (table[bst],table[n]);
					bst++;
                    return;
                }
        }

    }

}
