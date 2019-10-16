using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RapChessGui
{
	public partial class FormOptions : Form
	{
		public FormOptions()
		{
			InitializeComponent();
		}

		private void CbRotateBoard_CheckedChanged(object sender, EventArgs e)
		{
			CData.rotateBoard = cbRotateBoard.Checked;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			colorDialog1.Color = CPieceBoard.color;
			if(colorDialog1.ShowDialog() != DialogResult.Cancel)
			{
				CPieceBoard.color = colorDialog1.Color;
			}
		}
	}
}
