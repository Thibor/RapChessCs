using System;

using Color = System.Int32;
using Move = System.Int32;
using Square = System.Int32;
using Bitboard = System.UInt64;
using Hash = System.UInt64;

namespace NSRapchess

{

	public enum RecType : byte
	{
		invalid,
		alpha,
		beta,
		score
	}

	public struct CRec
	{
		public ulong hash;//8
		public int value;//4
		public int move;//4
		public byte depth;//1
		public ushort halfMove;//2
		public RecType type;//1

		public CRec(ulong hash, int depth, int move, int value, RecType type, ushort halfMove)
		{
			this.hash = hash;
			this.depth = (byte)depth;
			this.move = move;
			this.value = value;
			this.type = type;
			this.halfMove = halfMove;
		}

	}

	public class CRecList
	{
		internal readonly CRec[] table = new CRec[CTranspositionTable.clusterSize];
		internal int count = 0;

		public void Add(CRec rec)
		{
			table[count++] = rec;
		}

		public void RemoveAt(int i)
		{
			count--;
			(table[i], table[count]) = (table[count], table[i]);
		}

		public void CopyTo(CRecList to)
		{
			Array.Copy(this.table, to.table, count);
			to.count = count;
		}

	}

	internal class CTranspositionTable
	{
		static ulong used = 0;
		public const int DEFAULT_SIZE_MB = 20;
		static ulong hashMask;
		static CRec[] table;
		static CRec empty = new CRec();
		readonly static int clusterShift = 2;
		public static int clusterSize = 1 << clusterShift;
		readonly static int clusterMask = clusterSize - 1;

		static CTranspositionTable()
		{
			Resize(DEFAULT_SIZE_MB);
		}

		public static void Resize(int hashSizeMBytes)
		{
			int sizeRec = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CRec));
			int mb = 1 << 20;
			int length = 0x100;
			while ((length * sizeRec) < (hashSizeMBytes * mb))
				length <<= 1;
			table = new CRec[length--];
			hashMask = (ulong)(length & ~clusterMask);
			used = 0;
		}

		public static void Clear()
		{
			used = 0;
			Array.Clear(table, 0, table.Length);
		}

		public static int Permill()
		{
			return (int)(used * 1000ul / (ulong)table.Length);
		}

		public static CRecList GetRecList(Hash hash)
		{
			CRecList moves = new CRecList();
			int index = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				CRec rec = table[index + n];
				if (rec.hash == hash)
					moves.Add(rec);
			}
			return moves;
		}

		public static void GetRecScore(ulong hash, out CRec rec)
		{
			int iStart = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				rec = table[iStart + n];
				if ((rec.hash == hash) && (rec.type == RecType.score))
					return;
				if (rec.type == RecType.invalid) break;
			}
			rec = empty;
		}

		public static void GetRec(ulong hash, int depth, out CRec rec)
		{
			int iStart = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				rec = table[iStart + n];
				if (rec.depth < depth)
					break;
				else if (rec.hash == hash)
					return;
			}
			rec = empty;
		}

		public static void GetRec(ulong hash, out CRec rec)
		{
			int iStart = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				rec = table[iStart + n];
				if ((rec.hash == hash) || (rec.type == RecType.invalid))
					return;
			}
			rec = empty;
		}

		public static void SetRec(CRec rec)
		{
			int iStart = (int)(rec.hash & hashMask);
			if (table[iStart + clusterSize - 1].type == RecType.invalid)
				used++;
			for (int i = clusterSize - 1; i > 0; i--)
				table[iStart + i] = table[iStart + i - 1];
			table[iStart] = rec;
		}

	}
}

/*public static void SetRec1(CRec rec)
{
	int iStart = (int)(rec.hash & hashMask);
	CRec last = table[iStart + clusterSize - 1];
	if (last.type == RecType.invalid)
		used++;
	if (last.halfMove + last.depth <= rec.halfMove + rec.depth)
	{
		for (int i = clusterSize - 1; i > 0; i--)
			table[iStart + i] = table[iStart + i - 1];
		table[iStart] = rec;
	}
}

public static void SetRec2(CRec rec)
{
	int iStart = (int)(rec.hash & hashMask);
	CRec last = table[iStart + clusterSize - 1];
	for (int n = 0; n < clusterSize; n++)
	{
		ref CRec or = ref table[iStart + n];
		if ((or.hash == rec.hash) && (or.move == rec.move))
		{
			or = rec;
			break;
		}
		if ((or.depth < rec.depth) || (or.halfMove < rec.halfMove) || ((or.depth == rec.depth) && (or.type <= rec.type)))
		{
			if (last.type == RecType.invalid)
				used++;
			for (int i = clusterSize - 1; i > n; i--)
				table[iStart + i] = table[iStart + i - 1];
			or = rec;
			break;
		}
	}
}*/

/*public static MList GetMoves(Hash hash)
{
	MList moves = new MList();
	int index = (int)(hash & hashMask);
	for (int n = 0; n < clusterSize; n++)
	{
		CRec rec = table[index + n];
		if ((rec.hash == hash) && (rec.move != 0))
			moves.Add(rec.move);
	}
	return moves;
}*/



/*public static bool IsDoubled(Hash hash)
{
	MList moves = GetMoves(hash);
	for (int n = 0; n < moves.count; n++)
		for (int i = n + 1; i < moves.count; i++)
			if (moves.table[n].move == moves.table[i].move)
				return true;
	return false;
}*/


/*public static void GetRec(ulong hash, out CRec rec, ref int start)
{
	int index = (int)(hash & (ulong)table.Length);
	while (start < clusterSize)
	{
		rec = table[index + start++];
		if (rec.hash == hash)
			return;
		if (rec.type == RecType.invalid) break;
	}
	rec = empty;
}*/

/*public static void GetRec(ulong hash, out CRec rec)
{
	//int index = (int)(hash & hashMask);
	int index = (int)(hash % (ulong)_table.Length);
	for (int n = 0; n < clusterSize; n++)
	{
		rec = _table[index + n];
		if (rec.hash == hash)
			return;
	}
	rec = empty;
}

public static void SetRec(CRec rec)
{
	int index = (int)(rec.hash & hashMask);
	//int index = (int)(rec.hash % tabLength);
	for (int n = 0; n < clusterSize; n++)
	{
		ref CRec org = ref _table[index + n];
		if (org.type == RecType.invalid)
		{
			used++;
			org = rec;
			return;
		}
		if (org.age - org.ply > rec.age - rec.ply)
			continue;
		if (org.type < rec.type)
		{
			org = rec;
			return;
		}
		if (org.age - org.ply + org.depth > rec.age - rec.ply + rec.depth)
			continue;
		if (org.type <= rec.type)
		{
			org = rec;
			return;
		}
		if ((org.age + org.depth) <= (rec.age + rec.depth))
			org = rec;
	}
}*/


/*public static void GetRec(ulong hash, out CRec rec)
{
	CRec bst = new CRec();
	int index = (int)(hash & hashMask);
	for (int n = 0; n < clusterSize; n++)
	{
		rec = _table[index + n];
		if ((rec.hash == hash)&&(rec.type>bst.type))
			bst=rec;
		if (rec.type == RecType.invalid)break;
	}
	rec = bst;
}*/

/*
 * 		public static void SetRec(CRec rec)
		{
			int iStart = (int)(rec.hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				int nCur = iStart + n;
				if (IsBetter(rec, _table[nCur]))
				{
					for (int i = clusterSize - 1; i > n; i--)
					{
						CRec r = _table[iStart + i - 1];
						if ((r.hash == rec.hash) && (r.move == rec.move))
						{
							_table[iStart + i - 1] = rec;
							for (int k = i; k < clusterSize - 1; k++)
								_table[iStart + k] = _table[iStart + k + 1];
							_table[iStart + clusterSize - 1] = empty;
							return;
						}
						_table[iStart + i] = r;
					}
					_table[nCur] = rec;
					return;
				}
			}
		}

//ref CRec bst = ref _table[iStart];
/*for (int n = clusterSize - 1; n >= 0; n--)
{
	ref CRec cur = ref _table[iStart + n];
	if(cur.hash == rec.hash)
	{
		cur = rec;
		return;
	}
	if(n>0)
			cur = _table[iStart + n - 1];
}
if (invalid)
	used++;
_table[iStart] = rec;
return;*/

/*
 * 			int iStart = (int)(rec.hash & hashMask);
	ref CRec bst =ref rec;
	for (int n = 0; n < clusterSize; n++)
	{
		ref CRec cur = ref _table[iStart + n];
		if (cur.hash == rec.hash)
		{
			cur = rec;
			return;
		}
		if (cur.type == RecType.invalid)
		{
			used++;
			cur = rec;
			return;
		}
		if (IsBetter(bst, cur))
			bst =ref cur;
	}
	bst = rec;
 */