using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapChessCs
{
	public static class CBitBoard
	{
		public static void Add(ref ulong board,int index)
		{
			board |= (ulong)(long)(1 << (63- index));
		}

		public static void Del(ref ulong board,int index)
		{
			board &= (ulong)~(1 << (63 - index));
		}

	}
}
