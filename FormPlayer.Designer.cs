namespace RapChessGui
{
	partial class FormPlayer
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
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.panel1 = new System.Windows.Forms.Panel();
			this.bDelete = new System.Windows.Forms.Button();
			this.bCreate = new System.Windows.Forms.Button();
			this.bUpdate = new System.Windows.Forms.Button();
			this.gbMode = new System.Windows.Forms.GroupBox();
			this.nudDepth = new System.Windows.Forms.NumericUpDown();
			this.nudTime = new System.Windows.Forms.NumericUpDown();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.tbParameters = new System.Windows.Forms.TextBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.cbEngList = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tbUserName = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.panel1.SuspendLayout();
			this.gbMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDepth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTime)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.bDelete);
			this.panel1.Controls.Add(this.bCreate);
			this.panel1.Controls.Add(this.bUpdate);
			this.panel1.Controls.Add(this.gbMode);
			this.panel1.Controls.Add(this.groupBox3);
			this.panel1.Controls.Add(this.groupBox4);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(489, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(311, 450);
			this.panel1.TabIndex = 0;
			// 
			// bDelete
			// 
			this.bDelete.Dock = System.Windows.Forms.DockStyle.Top;
			this.bDelete.Location = new System.Drawing.Point(0, 265);
			this.bDelete.Name = "bDelete";
			this.bDelete.Size = new System.Drawing.Size(311, 33);
			this.bDelete.TabIndex = 19;
			this.bDelete.Text = "Delete";
			this.bDelete.UseVisualStyleBackColor = true;
			this.bDelete.Click += new System.EventHandler(this.bDelete_Click);
			// 
			// bCreate
			// 
			this.bCreate.Dock = System.Windows.Forms.DockStyle.Top;
			this.bCreate.Location = new System.Drawing.Point(0, 232);
			this.bCreate.Name = "bCreate";
			this.bCreate.Size = new System.Drawing.Size(311, 33);
			this.bCreate.TabIndex = 18;
			this.bCreate.Text = "Create";
			this.bCreate.UseVisualStyleBackColor = true;
			this.bCreate.Click += new System.EventHandler(this.bCreate_Click);
			// 
			// bUpdate
			// 
			this.bUpdate.Dock = System.Windows.Forms.DockStyle.Top;
			this.bUpdate.Location = new System.Drawing.Point(0, 205);
			this.bUpdate.Name = "bUpdate";
			this.bUpdate.Size = new System.Drawing.Size(311, 27);
			this.bUpdate.TabIndex = 17;
			this.bUpdate.Text = "Update";
			this.bUpdate.UseVisualStyleBackColor = true;
			this.bUpdate.Click += new System.EventHandler(this.bUpdate_Click);
			// 
			// gbMode
			// 
			this.gbMode.Controls.Add(this.nudDepth);
			this.gbMode.Controls.Add(this.nudTime);
			this.gbMode.Controls.Add(this.radioButton2);
			this.gbMode.Controls.Add(this.radioButton1);
			this.gbMode.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbMode.Location = new System.Drawing.Point(0, 135);
			this.gbMode.Name = "gbMode";
			this.gbMode.Size = new System.Drawing.Size(311, 70);
			this.gbMode.TabIndex = 16;
			this.gbMode.TabStop = false;
			this.gbMode.Text = "Mode";
			// 
			// nudDepth
			// 
			this.nudDepth.Location = new System.Drawing.Point(83, 39);
			this.nudDepth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudDepth.Name = "nudDepth";
			this.nudDepth.Size = new System.Drawing.Size(222, 20);
			this.nudDepth.TabIndex = 3;
			this.nudDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.nudDepth.ThousandsSeparator = true;
			this.nudDepth.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// nudTime
			// 
			this.nudTime.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.nudTime.Location = new System.Drawing.Point(83, 16);
			this.nudTime.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.nudTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudTime.Name = "nudTime";
			this.nudTime.Size = new System.Drawing.Size(222, 20);
			this.nudTime.TabIndex = 2;
			this.nudTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.nudTime.ThousandsSeparator = true;
			this.nudTime.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(6, 42);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(54, 17);
			this.radioButton2.TabIndex = 1;
			this.radioButton2.Text = "Depth";
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Checked = true;
			this.radioButton1.Location = new System.Drawing.Point(6, 19);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(48, 17);
			this.radioButton1.TabIndex = 0;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "Time";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.tbParameters);
			this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox3.Location = new System.Drawing.Point(0, 90);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(311, 45);
			this.groupBox3.TabIndex = 15;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Parameters";
			// 
			// tbParameters
			// 
			this.tbParameters.Dock = System.Windows.Forms.DockStyle.Top;
			this.tbParameters.Location = new System.Drawing.Point(3, 16);
			this.tbParameters.Name = "tbParameters";
			this.tbParameters.Size = new System.Drawing.Size(305, 20);
			this.tbParameters.TabIndex = 0;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.cbEngList);
			this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox4.Location = new System.Drawing.Point(0, 45);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(311, 45);
			this.groupBox4.TabIndex = 11;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Engin";
			// 
			// cbEngList
			// 
			this.cbEngList.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbEngList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbEngList.FormattingEnabled = true;
			this.cbEngList.Items.AddRange(new object[] {
            "Human"});
			this.cbEngList.Location = new System.Drawing.Point(3, 16);
			this.cbEngList.Name = "cbEngList";
			this.cbEngList.Size = new System.Drawing.Size(305, 21);
			this.cbEngList.Sorted = true;
			this.cbEngList.TabIndex = 2;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tbUserName);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(311, 45);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Name";
			// 
			// tbUserName
			// 
			this.tbUserName.Dock = System.Windows.Forms.DockStyle.Top;
			this.tbUserName.Location = new System.Drawing.Point(3, 16);
			this.tbUserName.Name = "tbUserName";
			this.tbUserName.Size = new System.Drawing.Size(305, 20);
			this.tbUserName.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.listBox1);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(489, 450);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Players List";
			// 
			// listBox1
			// 
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(3, 16);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(483, 431);
			this.listBox1.Sorted = true;
			this.listBox1.TabIndex = 1;
			this.listBox1.SelectedValueChanged += new System.EventHandler(this.ListBox1_SelectedValueChanged);
			// 
			// FormPlayer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimizeBox = false;
			this.Name = "FormPlayer";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Players";
			this.TopMost = true;
			this.panel1.ResumeLayout(false);
			this.gbMode.ResumeLayout(false);
			this.gbMode.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDepth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTime)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox tbUserName;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.ComboBox cbEngList;
		private System.Windows.Forms.Button bDelete;
		private System.Windows.Forms.Button bCreate;
		private System.Windows.Forms.Button bUpdate;
		private System.Windows.Forms.GroupBox gbMode;
		private System.Windows.Forms.NumericUpDown nudDepth;
		private System.Windows.Forms.NumericUpDown nudTime;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox tbParameters;
	}
}