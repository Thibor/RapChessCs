namespace RapChessCs
{
	class CSynStop
	{
		private bool value;
		private readonly object locker = new object();

		public bool GetStop()
		{
			lock (locker)
			{
				return value;
			}
		}

		public void SetStop(bool v)
		{
			lock (locker)
			{
				value = v;
			}
		}

	}
}
