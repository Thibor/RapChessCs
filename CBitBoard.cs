using System.Runtime.CompilerServices;
using Color = System.Int32;
using Move = System.Int32;
using Bitboard = System.UInt64;
using Square = System.Int32;

namespace NSRapchess
{
	public static class CBitboard
	{

        /// <summary>
        /// The collection of indices for calculating the index of a single bit. 
        /// </summary>
        private static readonly int[] BitIndex = new int[64];

        public static Bitboard[] bb = new Bitboard[64];

        static CBitboard()
		{
            // Initialize bit index table. 
            for (int i = 0; i < 64; i++)
            {
                Bitboard b = 1ul << i;
                bb[i] = b;
                BitIndex[(b * 0x07EDD5E59A4E28C2UL) >> 58] = i;
            }
        }

		public static void Add(ref ulong board, int index)
		{
			board |= bb[index];
		}

		public static void Add(ref ulong board, int x, int y)
		{
			if ((x >= 0) && (y >= 0) && (x < 8) && (y < 8))
				Add(ref board,(y << 3) | x);
		}

		public static void AddR(ref ulong board, int x, int y)
		{
			x = 7 - x;
			y = 7 - y;
			if ((x >= 0) && (y >= 0) && (x < 8) && (y < 8))
				Add(ref board, (y << 3) | x);
		}

		public static void Del(ref ulong board, int index)
		{
			board &= ~bb[index];
		}

        /// <summary>
        /// Removes and returns the index of the least significant set bit in the 
        /// given bitboard.  
        /// </summary>
        /// <param name="bitboard">The bitboard to pop.</param>
        /// <returns>The index of the least significant set bit.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Pop(ref Bitboard bitboard)
        {
            Bitboard isolatedBit = bitboard & (0UL - bitboard);
            bitboard &= bitboard - 1;
            return BitIndex[(isolatedBit * 0x07EDD5E59A4E28C2UL) >> 58];
        }

        /// <summary>
        /// Returns the index of the bit in a bitboard with a single set bit. 
        /// </summary>
        /// <param name="bitboard">The bitboard to read.</param>
        /// <returns>The index of the single set bit.</returns>
        public static int Read(Bitboard bitboard)
        {
            return BitIndex[(bitboard * 0x07EDD5E59A4E28C2UL) >> 58];
        }

        /// <summary>
        /// Returns the index of the least significant set bit in the given 
        /// bitboard.
        /// </summary>
        /// <param name="bitboard">The bitboard to scan.</param>
        /// <returns>The index of the least significant set bit.</returns>
        public static int Scan(Bitboard bitboard)
        {
            return BitIndex[((bitboard & (0UL - bitboard)) * 0x07EDD5E59A4E28C2UL) >> 58];
        }

        /// <summary>
        /// Returns the given bitboard with only the least significant bit set.
        /// </summary>
        /// <param name="bitboard">The bitboard to isolate the least significant bit.</param>
        /// <returns>The given bitboard with only the least significant bit set.</returns>
        public static Bitboard Isolate(Bitboard bitboard)
        {
            return bitboard & (0UL - bitboard);
        }

        /// <summary>
        /// Returns the number of set bits in the given bitboard.
        /// </summary>
        /// <param name="bitboard">The bitboard to count.</param>
        /// <returns>The number of set bits.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(Bitboard bitboard)
        {
            bitboard -= (bitboard >> 1) & 0x5555555555555555UL;
            bitboard = (bitboard & 0x3333333333333333UL) + ((bitboard >> 2) & 0x3333333333333333UL);
            return (int)(((bitboard + (bitboard >> 4) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

    }
}
