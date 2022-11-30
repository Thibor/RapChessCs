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
		exact
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
		internal readonly CRec[] table = new CRec[CTranspositionTable.clusterSize + 2];
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
			used=0;
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

		/*public static MList GetMoves(Hash hash)
		{
			MList moves = new MList();
			int index = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				CRec rec = table[index + n];
				if ((rec.hash == hash) && (rec.move != 0))
				{
					moves.Add(rec.move);
					break;
				}
			}
			return moves;
		}*/

		public static void GetRecExact(ulong hash, out Move move)
		{
			int iStart = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				CRec rec = table[iStart + n];
				if ((rec.hash == hash) && (rec.type == RecType.exact))
				{
					move = rec.move;
					return;
				}
				if (rec.type == RecType.invalid) break;
			}
			move = 0;
		}

		public static void GetRec(ulong hash, int halfMove, int depth, out CRec rec)
		{
			int iStart = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				rec = table[iStart + n];
				//if ((rec.hash == hash)&&(rec.halfMove+rec.depth>=halfMove+depth))
				//if ((rec.hash == hash) && (rec.halfMove >= halfMove))
				//if ((rec.hash == hash) && (rec.depth >= depth))
				if ((rec.hash == hash) && (rec.halfMove >= halfMove) && (rec.depth >= depth))
					return;
				/*if ((rec.depth < depth) || (rec.halfMove < halfMove))
					break;
				else */
				/*if (rec.hash == hash)
					return;*/
			}
			rec = empty;
		}

		public static void GetRec(ulong hash, out CRec rec)
		{
			int iStart = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				rec = table[iStart + n];
				if ((rec.hash == hash)||(rec.type==RecType.invalid))
					return;
			}
			rec = empty;
		}

		public static void SetRec(CRec rec)
		{
			int iStart = (int)(rec.hash & hashMask);
			ref CRec rep = ref table[iStart];
			for (int n = 0; n < clusterSize; n++)
			{
				ref CRec cur = ref table[iStart + n];
				if (cur.type == RecType.invalid)
				{
					used++;
					cur = rec;
					return;
				}
				if (cur.hash == rec.hash)
				{
					if (rec.move == 0)
						rec.move = cur.move;
					cur = rec;
					return;
				}
				int c1 = rep.halfMove < rec.halfMove ? 2 : 0;
				int c2 = (cur.halfMove == rec.halfMove) || (cur.type == RecType.exact) ? -2 : 0;
				int c3 = cur.depth < rec.depth ? 1 : 0;
				if (c1 + c2 + c3 > 0)
					rep = cur;
			}
			rep = rec;
			/*if (table[iStart + clusterSize - 1].type == RecType.invalid)
				used++;
			for (int i = clusterSize - 1; i > 0; i--)
				table[iStart + i] = table[iStart + i - 1];
			table[iStart] = rec;*/
		}

	}
}