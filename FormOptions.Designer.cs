namespace RapChessGui
{
	partial class FormOptions
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cbRotateBoard = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.SuspendLayout();
			// 
			// cbRotateBoard
			// 
			this.cbRotateBoard.AutoSize = true;
			this.cbRotateBoard.Location = new System.Drawing.Point(44, 33);
			this.cbRotateBoard.Name = "cbRotateBoard";
			this.cbRotateBoard.Size = new System.Drawing.Size(88, 17);
			this.cbRotateBoard.TabIndex = 0;
			this.cbRotateBoard.Text = "Rotate board";
			this.cbRotateBoard.UseVisualStyleBackColor = true;
			this.cbRotateBoard.CheckedChanged += new System.EventHandler(this.CbRotateBoard_CheckedChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(44, 65);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(79, 24);
			this.button1.TabIndex = 1;
			this.button1.Text = "Board color";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// FormOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(168, 118);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.cbRotateBoard);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "FormOptions";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbRotateBoard;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ColorDialog colorDialog1;
	}
}