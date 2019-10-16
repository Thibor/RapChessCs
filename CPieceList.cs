using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RapChessGui
{
	static class CPieceBoard
	{
		public static bool animated = true;
		public static CPiece[] list = new CPiece[64];
		public const int field = 64;
		public const int margin = 32;
		public const int size = field * 8 + margin * 2;
		public static Bitmap bitmap = new Bitmap(size, size);
		public static Color color;

		public static void SaveToIni()
		{
			CIniFile.Write("colorR",color.R.ToString());
			CIniFile.Write("colorG", color.G.ToString());
			CIniFile.Write("colorB", color.B.ToString());
		}


		public static void LoadFromIni()
		{
			int r = Convert.ToInt32(CIniFile.Read("colorR", "64"));
			int g = Convert.ToInt32(CIniFile.Read("colorG", "0"));
			int b = Convert.ToInt32(CIniFile.Read("colorB", "0"));
			color = Color.FromArgb(r,g,b);
		}

		public static void Prepare()
		{
			Graphics g = Graphics.FromImage(bitmap);
			SolidBrush brush1 = new SolidBrush(color);
			SolidBrush brush2 = new SolidBrush(Color.FromArgb(0x60, 0x00, 0x00, 0x00));
			SolidBrush brush3 = new SolidBrush(Color.FromArgb(0x60, 0xff, 0xff, 0xff));
			g.FillRectangle(brush1, 0, 0, size, size);
			for (int y = 0; y < 8; y++)
			{
				int y2 = margin + y * field;
				for (int x = 0; x < 8; x++)
				{
					int x2 = margin + x * field;
					bool bgColor = ((y ^ x) & 1) == 1;
					if (bgColor)
					{
						g.FillRectangle(brush2, x2, y2, field, field);
					}
					else
					{
						g.FillRectangle(brush3, x2, y2, field, field);
					}
				}
			}
		}

		static void MakeMove(int sou, int des)
		{
			list[des] = list[sou];
			list[sou] = null;
		}

		public static void MakeMove(int gm)
		{
			animated = true;
			int flags = gm & 0xFF0000;
			int sou = gm & 0xFF;
			int des = (gm >> 8) & 0xFF;
			int xs = (sou & 0xf) - 4;
			int ys = (sou >> 4) - 4;
			int xd = (des & 0xf) - 4;
			int yd = (des >> 4) - 4;
			sou = ys * 8 + xs;
			des = yd * 8 + xd;
			var pd = list[des];
			if (pd != null)
				list[sou].desImage = pd.image;
			MakeMove(sou, des);
			if ((flags & CEngine.moveflagCastleKing) > 0)
			{
				MakeMove(sou + 3, sou + 1);
			}
			if ((flags & CEngine.moveflagCastleQueen) > 0)
			{
				MakeMove(sou - 4, sou - 1);
			}
		}

		public static void Render()
		{
			animated = false;
			for (int n = 0; n < 64; n++)
			{
				var p = list[n];
				if (p != null)
					if (p.Render())
						animated = true;
			}
		}
	}

	class CPiece
	{
		bool visible = false;
		public int desImage = -1;
		public int image = -1;
		public int index = -1;
		public Point curXY = new Point();
		Point souXY = new Point();
		public Point desXY = new Point();
		DateTime dt;
		double time = 200;

		public bool Render()
		{
			if ((curXY.X == desXY.X) && (curXY.Y == desXY.Y))
				return false;
			double dif = (DateTime.Now - dt).TotalMilliseconds / time;
			if (dif >= 1)
			{
				curXY.X = desXY.X;
				curXY.Y = desXY.Y;
				SetImage();
			}
			else
			{
				curXY.X = Convert.ToInt32(souXY.X * (1 - dif) + desXY.X * dif);
				curXY.Y = Convert.ToInt32(souXY.Y * (1 - dif) + desXY.Y * dif);
			}
			return true;
		}

		public void SetImage()
		{
			desImage = -1;
			int i = CEngine.arrField[index];
			int f = CEngine.g_board[i];
			image = (f & 7) - 1;
			if ((f & CEngine.colorBlack) > 0)
				image += 6;
		}

		public void SetDes(int i, int x, int y)
		{
			if (!visible)
			{
				visible = true;
				curXY.X = x;
				curXY.Y = y;
			}
			if ((x == desXY.X) && (y == desXY.Y))
				return;
			index = i;
			souXY.X = curXY.X;
			souXY.Y = curXY.Y;
			dt = DateTime.Now;
			desXY.X = x;
			desXY.Y = y;
		}
	}

	class CPieceList
	{
		public void Fill()
		{
			for (int n = 0; n < 64; n++)
			{
				CPieceBoard.list[n] = null;
				int i = CEngine.arrField[n];
				int f = CEngine.g_board[i];
				if ((f & CEngine.colorEmpty) > 0)
					continue;
				CPiece piece = new CPiece();
				piece.index = n;
				piece.SetImage();
				CPieceBoard.list[n] = piece;
			}
			CPieceBoard.animated = true;
		}

	}
}
