namespace NSRapchess
{
	struct CUndo
	{
		public int captured;
		public int passing;
		public int castle;
		public byte move50;
		public ulong hash;
	}
}
