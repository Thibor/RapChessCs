namespace NSRapchess
{
	public class CSynStop
	{
		private bool value = true;
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
