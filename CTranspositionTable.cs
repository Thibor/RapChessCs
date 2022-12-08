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
		public byte age;//1
		public RecType type;//1

		public CRec(ulong hash, int depth, int move, int value, RecType type, byte age)
		{
			this.hash = hash;
			this.depth = (byte)depth;
			this.move = move;
			this.value = value;
			this.type = type;
			this.age = age;
		}

	}

	internal class CTranspositionTable
	{
		static byte generation = 0;
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
			generation = 0;
			Array.Clear(table, 0, table.Length);
		}

		public static void NextGeneration()
		{
			if (++generation == 0)
				Clear();
		}

		public static int Permill()
		{
			return (int)(used * 1000ul / (ulong)table.Length);
		}

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
				if ((rec.hash == hash) && (rec.age >= halfMove) && (rec.depth >= depth))
					return;
			}
			rec = empty;
		}

		public static void GetRec(ulong hash, int depth, out CRec rec)
		{
			int iStart = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				rec = table[iStart + n];
				if (rec.hash == hash)
				{
					if (rec.depth < depth)
						rec = empty;
					else
						table[iStart + n].age = generation;
					return;
				}
			}
			rec = empty;
		}

		public static void GetRec(ulong hash, out CRec rec)
		{
			int iStart = (int)(hash & hashMask);
			for (int n = 0; n < clusterSize; n++)
			{
				rec = table[iStart + n];
				if (rec.hash == hash)
					return;
			}
			rec = empty;
		}

		public static void SetRec(ulong hash, int depth, int bestMove, int bestValue, RecType rt)
		{
			CRec rec = new CRec(hash, depth, bestMove, bestValue, rt, generation);
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
				if (((cur.age == generation || cur.type == RecType.exact) ? 1 : 0)
				   - ((rep.age == generation) ? 1 : 0)
				   - ((cur.depth < rep.depth) ? 1 : 0) < 0)
					rep = cur;
			}
			rep = rec;
		}

	}
}