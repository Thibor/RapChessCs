using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapChessGui
{
	class CTrainer
	{
		public int rotate = 0;
		public int games = 0;
		public int win = 0;
		public int draw = 0;
		public int loose = 0;
		public int time = 1000;
		public CUser user = new CUser("Teacher");

		public int Total()
		{
			return win + draw + loose;
		}

		public int Result()
		{
			int t = Total();
			if (t == 0)
				return 100;
			return ((win * 2 + draw) * 100) / (t * 2);
		}

	}
}
