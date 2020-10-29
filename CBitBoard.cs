using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapChessCs
{
	public static class CBitBoard
	{
		public static void Add(ref ulong board, int index)
		{
			board |= (ulong)1 << index;
		}

		public static void Add(ref ulong board, int x, int y)
		{
			if ((x >= 0) && (y >= 0) && (x < 8) && (y < 8))
				Add(ref board, (y << 3) | x);
		}

		public static void Del(ref ulong board, int index)
		{
			board &= ~((ulong)1 << index);
		}

	}
}
